using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoEcommerce.Models;

namespace ProyectoEcommerce.Controllers
{
    public class UsuarioController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        List<Usuario> ListUsuario()
        {
            List<Usuario> aUsuarios = new List<Usuario>();
            SqlCommand cmd = new SqlCommand("SP_LISTARUSUARIOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aUsuarios.Add(new Usuario()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        usuario = dr[1].ToString(),
                        contraseña = dr[2].ToString(),
                        nombre = dr[3].ToString(),
                        email = dr[4].ToString(),
                        dni = dr[5].ToString(),
                        telefono = dr[6].ToString(),
                        token = dr[7].ToString(),
                        rol = dr[8].ToString()
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

        List<UsuarioO> ListUsuarioO()
        {
            List<UsuarioO> aUsuarios = new List<UsuarioO>();
            SqlCommand cmd = new SqlCommand("SP_USUARIOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
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

        List<Usuario> ListUsuariosxRolxLoginoEmail(int idrol, string cadena)
        {
            List<Usuario> aUsuarios = new List<Usuario>();
            SqlCommand cmd = new SqlCommand("[SP_LISTARUSUARIOSXROLXLOGINOEMAIL]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@rol", idrol);
            cmd.Parameters.AddWithValue("@texto", cadena);
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aUsuarios.Add(new Usuario()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        usuario = dr[1].ToString(),
                        contraseña = dr[2].ToString(),
                        nombre = dr[3].ToString(),
                        email = dr[4].ToString(),
                        dni = dr[5].ToString(),
                        telefono = dr[6].ToString(),
                        token = dr[7].ToString(),
                        rol = dr[8].ToString()
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

        List<Rol> ListRol()
        {
            List<Rol> aRoles = new List<Rol>();
            SqlCommand cmd = new SqlCommand("SP_LISTARROLES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aRoles.Add(new Rol()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aRoles;
        }

        public ActionResult listadoUsuarios(int idrol = 0, string cadena = "")
        {
            ViewBag.rol = idrol;
            ViewBag.cadena = cadena;
            ViewBag.roles = new SelectList(ListRol(), "codigo", "nombre");

            return View(ListUsuariosxRolxLoginoEmail(idrol, cadena));
        }

        public ActionResult nuevoUsuario()
        {
            ViewBag.roles = new SelectList(ListRol(), "codigo", "nombre");
            ViewBag.codigo = codigoActual();
            return View(new UsuarioO());
        }

        [HttpPost]
        public ActionResult nuevoUsuario(UsuarioO objetoUsuarioO)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoUsuarioO);
            }
            ViewBag.codigo = codigoActual();

            if(verificaUsuario(objetoUsuarioO.usuario) == 1)
            {
                ViewBag.mensaje = "EL NOMBRE DE USUARIO YA SE ENCUENTRA EN USO";
                ViewBag.tipo = "error";
                ViewBag.rol = new SelectList(ListRol(), "codigo", "nombre");
                return View(objetoUsuarioO);
            }
            else
            {
                cn.Open();
                ViewBag.mensaje = "";
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    SqlCommand cmd = new SqlCommand("SP_NUEVOUSUARIO", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@login", objetoUsuarioO.usuario);
                    cmd.Parameters.AddWithValue("@pass", objetoUsuarioO.contraseña);
                    cmd.Parameters.AddWithValue("@nom", objetoUsuarioO.nombre);
                    cmd.Parameters.AddWithValue("@email", objetoUsuarioO.email);
                    cmd.Parameters.AddWithValue("@dni", objetoUsuarioO.dni);
                    cmd.Parameters.AddWithValue("@tel", objetoUsuarioO.telefono);
                    cmd.Parameters.AddWithValue("@rol", objetoUsuarioO.rol);

                    int estado = cmd.ExecuteNonQuery();
                    tr.Commit();

                    ViewBag.mensaje = estado.ToString() + " USUARIO REGISTRADO";
                    ViewBag.tipo = "success";
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = ex.Message + ex.InnerException + ex.Source + ex.StackTrace;
                    ViewBag.tipo = "error";
                    tr.Rollback();
                }

                cn.Close();
                ViewBag.rol = new SelectList(ListRol(), "codigo", "nombre");
                return View(objetoUsuarioO);
            }
        }

        public ActionResult modificaUsuario(int id)
        {
            UsuarioO objetoUsuario = ListUsuarioO().Where(c => c.codigo == id).FirstOrDefault();
            ViewBag.rol = new SelectList(ListRol(), "codigo", "nombre", objetoUsuario.rol);
            return View(objetoUsuario);
        }

        [HttpPost]
        public ActionResult modificaUsuario(UsuarioO objetoUsuarioO)
        {
            if (!ModelState.IsValid)
            {
                return View(objetoUsuarioO);
            }

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_ACTUALIZAUSUARIO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoUsuarioO.codigo);
                cmd.Parameters.AddWithValue("@pass", objetoUsuarioO.contraseña);
                cmd.Parameters.AddWithValue("@nom", objetoUsuarioO.nombre);
                cmd.Parameters.AddWithValue("@email", objetoUsuarioO.email);
                cmd.Parameters.AddWithValue("@dni", objetoUsuarioO.dni);
                cmd.Parameters.AddWithValue("@tel", objetoUsuarioO.telefono);
                cmd.Parameters.AddWithValue("@rol", objetoUsuarioO.rol);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " USUARIO MODIFICADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message + ex.InnerException + ex.Source + ex.StackTrace;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.rol = new SelectList(ListRol(), "codigo", "nombre", objetoUsuarioO.rol);
            return View(objetoUsuarioO);
        }

        public ActionResult detalleUsuario(int id)
        {
            Usuario objetoUsuario = ListUsuario().Where(c => c.codigo == id).FirstOrDefault();
            return View(objetoUsuario);
        }

        public ActionResult eliminaUsuario(int id)
        {
            Usuario objetoUsuario = ListUsuario().Where(c => c.codigo == id).FirstOrDefault();
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_ELIMINAUSUARIO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoUsuario.codigo);
                int estado = cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            return RedirectToAction("listadoUsuarios");
        }

        public int codigoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_SIGCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "USUARIO");
            cn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                ultimo = int.Parse(dr[0].ToString());

            }

            dr.Close();
            cn.Close();

            return ultimo;
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
    }
}