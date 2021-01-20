using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoEcommerce.Models;

namespace ProyectoEcommerce.Controllers
{
    public class EmpleadoController : Controller
    {

        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        // Empleados
        List<Empleado> ListEmpleado()
        {

            List<Empleado> aEmpleados = new List<Empleado>();
            SqlCommand cmd = new SqlCommand("SP_LISTAREMPLEADOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aEmpleados.Add(new Empleado()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        nacimiento = DateTime.Parse(dr[2].ToString()).ToString("dd/MM/yyyy"),
                        direccion = dr[3].ToString(),
                        distrito = dr[4].ToString(),
                        telefono = dr[5].ToString(),
                        cargo = dr[6].ToString(),
                        contratacion = DateTime.Parse(dr[7].ToString()).ToString("dd/MM/yyyy"),
                        foto = dr[8].ToString()
                    });
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aEmpleados;
        }

        List<EmpleadoO> ListEmpleadoO()
        {
            List<EmpleadoO> aEmpleados = new List<EmpleadoO>();
            SqlCommand cmd = new SqlCommand("SP_EMPLEADOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aEmpleados.Add(new EmpleadoO()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString(),
                    nacimiento = DateTime.Parse(dr[2].ToString()).ToString("dd/MM/yyyy"),
                    direccion = dr[3].ToString(),
                    distrito = int.Parse(dr[4].ToString()),
                    telefono = dr[5].ToString(),
                    cargo = int.Parse(dr[6].ToString()),
                    contratacion = DateTime.Parse(dr[7].ToString()).ToString("dd/MM/yyyy"),
                    foto = dr[8].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aEmpleados;
        }

        List<Distrito> ListDistrito()
        {
            List<Distrito> aDistritos = new List<Distrito>();
            SqlCommand cmd = new SqlCommand("SP_LISTARDISTRITOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aDistritos.Add(new Distrito()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aDistritos;
        }

        List<Cargo> ListCargo()
        {
            List<Cargo> aCargos = new List<Cargo>();
            SqlCommand cmd = new SqlCommand("SP_LISTARCARGOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aCargos.Add(new Cargo()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aCargos;
        }

        /*
        public ActionResult Index()
        {
            return View();
        }
        */
        public ActionResult listadoEmpleados()
        {
            return View(ListEmpleado());
        }

        public ActionResult nuevoEmpleado()
        {
            ViewBag.distrito = new SelectList(ListDistrito(), "codigo", "nombre");
            ViewBag.cargo = new SelectList(ListCargo(), "codigo", "nombre");
            ViewBag.codigo = codigoActual();
            return View(new EmpleadoO());
        }
        [HttpPost]
        public ActionResult nuevoEmpleado(EmpleadoO objetoEmpleado)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoEmpleado);
            }

            ViewBag.codigo = codigoActual();

            string fotourl = "";

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file.ContentLength > 0)
                {
                    string imgname = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(imgname);

                    //FileInfo f = new FileInfo(imgname);
                    //string path = Path.GetFullPath(file.FileName);

                    if (extension == ".jpg" || extension == ".png")
                    {
                        //string imgpath = Path.Combine(Server.MapPath("~/fotos_empleados"), imgname);
                        //string fuente = Path.Combine(Directory.GetCurrentDirectory(), imgname);
                        //System.IO.File.Copy(f.FullName, imgpath, true);
                        fotourl = "~/fotos_empleados/" + imgname;
                    }
                }
            }


            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOEMPLEADO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoEmpleado.codigo);
                cmd.Parameters.AddWithValue("@nom", objetoEmpleado.nombre);
                cmd.Parameters.AddWithValue("@fecha", objetoEmpleado.nacimiento);
                cmd.Parameters.AddWithValue("@dir", objetoEmpleado.direccion);
                cmd.Parameters.AddWithValue("@distrito", objetoEmpleado.distrito);
                cmd.Parameters.AddWithValue("@tel", objetoEmpleado.telefono);
                cmd.Parameters.AddWithValue("@cargo", objetoEmpleado.cargo);
                cmd.Parameters.AddWithValue("@contrat", objetoEmpleado.contratacion);
                cmd.Parameters.AddWithValue("@foto", fotourl);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " EMPLEADO REGISTRADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.distrito = new SelectList(ListDistrito(), "codigo", "nombre");
            ViewBag.cargo = new SelectList(ListCargo(), "codigo", "nombre");
            return View(objetoEmpleado);

        }

        public ActionResult modificaEmpleado(int id)
        {
            EmpleadoO objetoEmpleado = ListEmpleadoO().Where(c => c.codigo == id).FirstOrDefault();
            ViewBag.distrito = new SelectList(ListDistrito(), "codigo", "nombre", objetoEmpleado.distrito);
            ViewBag.cargo = new SelectList(ListCargo(), "codigo", "nombre", objetoEmpleado.cargo);
            return View(objetoEmpleado);
        }

         [HttpPost]
         public ActionResult modificaEmpleado(EmpleadoO objetoEmpleado)
        {
            if (!ModelState.IsValid)
            {
                return View(objetoEmpleado);
            }

            string fotourl = "";

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file.ContentLength > 0)
                {
                    string imgname = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(imgname);

                    //FileInfo f = new FileInfo(imgname);
                    //string path = Path.GetFullPath(file.FileName);

                    if (extension == ".jpg" || extension == ".png")
                    {
                        //string imgpath = Path.Combine(Server.MapPath("~/fotos_empleados"), imgname);
                        //string fuente = Path.Combine(Directory.GetCurrentDirectory(), imgname);
                        //System.IO.File.Copy(f.FullName, imgpath, true);
                        fotourl = "~/fotos_empleados/" + imgname;
                    }
                }
            }

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOEMPLEADO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoEmpleado.codigo);
                cmd.Parameters.AddWithValue("@nom", objetoEmpleado.nombre);
                cmd.Parameters.AddWithValue("@fecha", objetoEmpleado.nacimiento);
                cmd.Parameters.AddWithValue("@dir", objetoEmpleado.direccion);
                cmd.Parameters.AddWithValue("@distrito", objetoEmpleado.distrito);
                cmd.Parameters.AddWithValue("@tel", objetoEmpleado.telefono);
                cmd.Parameters.AddWithValue("@cargo", objetoEmpleado.cargo);
                cmd.Parameters.AddWithValue("@contrat", objetoEmpleado.contratacion);
                cmd.Parameters.AddWithValue("@foto", fotourl);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " EMPLEADO MODIFICADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.distrito = new SelectList(ListDistrito(), "codigo", "nombre", objetoEmpleado.distrito);
            ViewBag.cargo = new SelectList(ListCargo(), "codigo", "nombre", objetoEmpleado.cargo);
            return View(objetoEmpleado);
        }

        public ActionResult detalleEmpleado(int id)
        {
            Empleado objetoEmpleado = ListEmpleado().Where(c => c.codigo == id).FirstOrDefault();
            return View(objetoEmpleado);
        }

        public ActionResult eliminaEmpleado(int id)
        {
            Empleado objetoEmpleado = ListEmpleado().Where(c => c.codigo == id).FirstOrDefault();
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_ELIMINAEMPLEADO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoEmpleado.codigo);
                int estado = cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            return RedirectToAction("listadoEmpleados");

        }

        public int codigoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_SIGCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "EMPLEADO");
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
        /*
        public ActionResult listadoEmpleadoxPag(int p = 0)
        {
            List<Empleado> arregloEmpleado = ListEmpleado();
            int filasxPag = 10;
            int n = arregloEmpleado.Count;
            int pag = n % filasxPag > 0 ? n / filasxPag + 1 : n / filasxPag;

            ViewBag.pag = pag;
            ViewBag.p = p;
            return View(arregloEmpleado.Skip(p * filasxPag).Take(filasxPag));
        }
        */

    }
}