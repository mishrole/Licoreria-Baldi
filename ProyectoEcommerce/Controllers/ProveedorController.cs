using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoEcommerce.Models;

namespace ProyectoEcommerce.Controllers
{
    public class ProveedorController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        // Proveedores
        List<Proveedor> ListProveedor()
        {
            List<Proveedor> aProveedores = new List<Proveedor>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPROVEEDORES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aProveedores.Add(new Proveedor() {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        nomcontacto = dr[2].ToString(),
                        carcontacto = dr[3].ToString(),
                        direccion = dr[4].ToString(),
                        pais = dr[5].ToString(),
                        telefono = dr[6].ToString()
                    });
                }

                dr.Close();
                cn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aProveedores;
        }

        List<ProveedorO> ListProveedorO()
        {
            List<ProveedorO> aProveedores = new List<ProveedorO>();
            SqlCommand cmd = new SqlCommand("SP_PROVEEDORES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aProveedores.Add(new ProveedorO()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        nomcontacto = dr[2].ToString(),
                        carcontacto = dr[3].ToString(),
                        direccion = dr[4].ToString(),
                        pais = int.Parse(dr[5].ToString()),
                        telefono = dr[6].ToString()
                    });
                }

                dr.Close();
                cn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aProveedores;
        }

        List<Pais> ListPais()
        {
            List<Pais> aPaises = new List<Pais>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPAISES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read()) 
            {
                aPaises.Add(new Pais()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aPaises;
        }

        /*
        public ActionResult Index()
        {
            return View();
        }*/

        public ActionResult listadoProveedores()
        {
            return View(ListProveedor());
        }

        public ActionResult nuevoProveedor()
        {
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre");
            ViewBag.codigo = codigoActual();
            return View(new ProveedorO());
        }
        [HttpPost]
        public ActionResult nuevoProveedor(ProveedorO objetoProveedor)
        {
           if(!ModelState.IsValid)
           {
                return View(objetoProveedor);
           }
            ViewBag.codigo = codigoActual();

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOPROVEEDOR", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoProveedor.codigo);
                cmd.Parameters.AddWithValue("@nom", objetoProveedor.nombre);
                cmd.Parameters.AddWithValue("@contacto", objetoProveedor.nomcontacto);
                cmd.Parameters.AddWithValue("@cargo", objetoProveedor.carcontacto);
                cmd.Parameters.AddWithValue("@dir", objetoProveedor.direccion);
                cmd.Parameters.AddWithValue("@pais", objetoProveedor.pais);
                cmd.Parameters.AddWithValue("@tel", objetoProveedor.telefono);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " PROVEEDOR REGISTRADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre");
            return View(objetoProveedor);
        }

        public ActionResult modificaProveedor(int id)
        {
            ProveedorO objetoProveedor = ListProveedorO().Where(c => c.codigo == id).FirstOrDefault();
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre", objetoProveedor.pais);
            return View(objetoProveedor);
        }

        [HttpPost]
        public ActionResult modificaProveedor(ProveedorO objetoProveedor)
        {
            if (!ModelState.IsValid)
            {
                return View(objetoProveedor);
            }

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOPROVEEDOR", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoProveedor.codigo);
                cmd.Parameters.AddWithValue("@nom", objetoProveedor.nombre);
                cmd.Parameters.AddWithValue("@contacto", objetoProveedor.nomcontacto);
                cmd.Parameters.AddWithValue("@cargo", objetoProveedor.carcontacto);
                cmd.Parameters.AddWithValue("@dir", objetoProveedor.direccion);
                cmd.Parameters.AddWithValue("@pais", objetoProveedor.pais);
                cmd.Parameters.AddWithValue("@tel", objetoProveedor.telefono);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " PROVEEDOR MODIFICADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre", objetoProveedor.pais);
            return View(objetoProveedor);
        }

        public ActionResult detalleProveedor (int id) 
        { 
            Proveedor objetoProveedor = ListProveedor().Where(c => c.codigo == id).FirstOrDefault();
            return View(objetoProveedor);
        }

        public ActionResult eliminaProveedor(int id)
        {
            Proveedor objetoProveedor = ListProveedor().Where(c => c.codigo == id).FirstOrDefault();
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_ELIMINAPROVEEDOR", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoProveedor.codigo);
                int estado = cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            return RedirectToAction("listadoProveedores");
        }

        public int codigoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_SIGCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "PROVEEDOR");
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
    }
}