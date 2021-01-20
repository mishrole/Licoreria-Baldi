using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProyectoEcommerce.Models;

namespace ProyectoEcommerce.Controllers
{
    public class AccesoController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);
        string urlDominio = "http://localhost:63351";

        public ActionResult RecuperarCuenta()
        {
            return View(new EmailReset());
        }

        [HttpPost]
        public ActionResult RecuperarCuenta(EmailReset objetoEmailReset)
        {

            if (!ModelState.IsValid)
            {
                return View(objetoEmailReset);
            }

            String token = GetSha256(Guid.NewGuid().ToString());

            if (objetoEmailReset != null)
            {

                var usuario = ListLogin().Where(u => u.email == objetoEmailReset.email).FirstOrDefault();

                if (usuario == null)
                {
                    ViewBag.mensaje = "NO EXISTE UNA CUENTA ASOCIADA A ESE CORREO ELECTRÓNICO";
                    ViewBag.tipo = "error";
                    return View();
                }

                cn.Open();
                ViewBag.mensaje = "";
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try
                {

                    SqlCommand cmd = new SqlCommand("SP_ACTUALIZATOKEN", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@email", usuario.email);
                    cmd.Parameters.AddWithValue("@token", token);

                    int estado = cmd.ExecuteNonQuery();
                    tr.Commit();

                    // Luego de actualizar el token, envía mensaje
                    EnviarEmail(objetoEmailReset.email, token);

                    ViewBag.mensaje = estado.ToString() + " MENSAJE ENVIADO A " + usuario.email;
                    ViewBag.tipo = "success";
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = ex.Message;
                    ViewBag.tipo = "error";
                    tr.Rollback();
                }
                cn.Close();
            }

            return View();

        }

        public ActionResult Reset(string token)
        {
            ResetPass resetPass = new ResetPass();
            resetPass.token = token;
            return View(resetPass);
        }
        [HttpPost]

        public ActionResult Reset(ResetPass objetoResetPass)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoResetPass);
            }

            if (objetoResetPass != null)
            {
                var usuario = ListLogin().Where(t => t.token == objetoResetPass.token).FirstOrDefault();

                if(usuario == null)
                {
                    ViewBag.mensaje = "OCURRIÓ UN ERROR AL PROCESAR EL CAMBIO DE CONTRASEÑA";
                    ViewBag.tipo = "error";
                    return View();
                }

                cn.Open();
                ViewBag.mensaje = "";
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    usuario.contraseña = objetoResetPass.contraseña;

                    SqlCommand cmd = new SqlCommand("SP_ACTUALIZAPASS", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@pass", usuario.contraseña);
                    cmd.Parameters.AddWithValue("@token", usuario.token);

                    int estado = cmd.ExecuteNonQuery();
                    tr.Commit();

                    ViewBag.mensaje = "CONTRASEÑA MODIFICADA CON ÉXITO";
                    ViewBag.tipo = "success";

                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = ex.Message;
                    ViewBag.tipo = "error";
                    return View();
                }

                cn.Close();

            }


            return View();
        }

        public ActionResult NuevaCuenta()
        {
            return View(new UsuarioO());
        }

        [HttpPost]
        public ActionResult NuevaCuenta(UsuarioO objetoUsuario)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoUsuario);
            }

            if(verificaUsuario(objetoUsuario.usuario) == 1)
            {
                ViewBag.mensaje = "EL NOMBRE DE USUARIO YA SE ENCUENTRA EN USO";
                ViewBag.tipo = "error";
                return View(objetoUsuario);
            }
            else
            {
                cn.Open();
                ViewBag.mensaje = "";
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    SqlCommand cmd = new SqlCommand("SP_NUEVACUENTA", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@login", objetoUsuario.usuario);
                    cmd.Parameters.AddWithValue("@pass", objetoUsuario.contraseña);
                    cmd.Parameters.AddWithValue("@nombre", objetoUsuario.nombre);
                    cmd.Parameters.AddWithValue("@email", objetoUsuario.email);
                    cmd.Parameters.AddWithValue("@dni", objetoUsuario.dni);
                    cmd.Parameters.AddWithValue("@tel", objetoUsuario.telefono);

                    int estado = cmd.ExecuteNonQuery();
                    tr.Commit();

                    ViewBag.mensaje = estado.ToString() + " CUENTA REGISTRADA CON ÉXITO";
                    ViewBag.tipo = "success";
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = ex.Message;
                    ViewBag.tipo = "error";
                    tr.Rollback();
                }

                cn.Close();
                // Si el registro es exitoso, inicia sesión directamente
                return Login(objetoUsuario.usuario, objetoUsuario.contraseña);

            }

        }

        List<UsuarioO> ListLogin()
        {
            List<UsuarioO> aUsuarios = new List<UsuarioO>();
            SqlCommand cmd = new SqlCommand("SP_USUARIOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    aUsuarios.Add(new UsuarioO()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        usuario = dr[1].ToString(),
                        contraseña = dr[2].ToString(),
                        nombre = dr[3].ToString(),
                        email = dr[4].ToString(),
                        dni = dr[5].ToString(),
                        telefono = dr[6].ToString(),
                        token = dr[7].ToString(),
                        rol = int.Parse(dr[8].ToString())
                    });
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aUsuarios;
        }

        // Si encuentra, retorna 1. Caso contrario, retorna 0
        public int verificaUsuario(String login)
        {
            int estado = 0;
            SqlCommand cmd = new SqlCommand("SP_BUSCARLOGINUSUARIO", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@login", login);
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                estado = int.Parse(dr[0].ToString());
            }

            dr.Close();
            cn.Close();

            return estado;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string pass)
        {
            try
            {
                ViewBag.mensaje = "";
                var usuario = ListLogin().Where(u => u.usuario == user).FirstOrDefault();

                if(usuario == null || usuario.contraseña != pass)
                {
                    ViewBag.mensaje = "Credenciales de acceso incorrectas";
                    ViewBag.tipo = "error";
                    return View();
                }

                Session["user"] = usuario;
                Session["codigousuario"] = usuario.codigo;
                Session["nombrelogin"] = usuario.nombre;
                Session["rolelogin"] = usuario.rol.ToString();

                return RedirectToAction("Index", "Licoreria");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                ViewBag.tipo = "error";
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "Acceso");
        }

        #region HELPERS
        private string GetSha256(string data)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(data));
            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }
            return sb.ToString();
        }

        private void EnviarEmail(string destino, string token)
        {
            string origin = "mishroletestmail@gmail.com";
            string pass = "Zhg85V*R1C04";
            string url = urlDominio+"/Acceso/Reset/?token="+token;

            MailMessage mensajeEmail = new MailMessage(origin, destino,
                "Nueva contraseña en Baldí",
                "<h2>Cambio de contraseña</h2>" +
                "<p>Hemos recibido tu pedido para recuperar la contraseña de su cuenta: <b>" + destino +"</b></p>" +
                "<p>Para realizar el cambio de contraseña, haz click en el siguiente enlace: <a style='font-weight:bolder' href='" + url + "'>Cambiar contraseña</a></p>" +
                "<p>Si no puedes abrir el link, copia y pega la siguiente dirección en tu navegador: <a href='"+ url+"'>"+ url +"</a></p>" +
                "<p>Gracias por usar <b><a href='"+ urlDominio +"'>Baldí</a></b></p>");

            mensajeEmail.IsBodyHtml = true;
            SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com");
            clienteSmtp.EnableSsl = true;
            clienteSmtp.UseDefaultCredentials = false;
            clienteSmtp.Port = 587;
            clienteSmtp.Credentials = new System.Net.NetworkCredential(origin, pass);
            clienteSmtp.Send(mensajeEmail);
            clienteSmtp.Dispose();


        }

        #endregion
    }
}