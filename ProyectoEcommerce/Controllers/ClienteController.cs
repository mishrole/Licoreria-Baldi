using ProyectoEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoEcommerce.Controllers
{
    public class ClienteController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        List<Cliente> ListCliente()
        {

            List<Cliente> aClientes = new List<Cliente>();
            SqlCommand cmd = new SqlCommand("SP_LISTARCLIENTES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aClientes.Add(new Cliente()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        direccion = dr[2].ToString(),
                        pais = dr[3].ToString(),
                        telefono = dr[4].ToString()
                    });
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aClientes;
        }

        List<ClienteO> ListClienteO()
        {

            List<ClienteO> aClientes = new List<ClienteO>();
            SqlCommand cmd = new SqlCommand("SP_CLIENTES", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aClientes.Add(new ClienteO()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        direccion = dr[2].ToString(),
                        pais = int.Parse(dr[3].ToString()),
                        telefono = dr[4].ToString()
                    });
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aClientes;
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

        /*public ActionResult Index()
        {
            return View();
        }*/

        public ActionResult listadoClientes()
        {
            return View(ListCliente());
        }

        public ActionResult nuevoCliente()
        {
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre");
            ViewBag.codigo = codigoActual();
            return View(new ClienteO());
        }

        [HttpPost]
        public ActionResult nuevoCliente(ClienteO objetoCliente)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoCliente);
            }
            ViewBag.codigo = codigoActual();

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOCLIENTE", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoCliente.codigo);
                cmd.Parameters.AddWithValue("nom", objetoCliente.nombre);
                cmd.Parameters.AddWithValue("dir", objetoCliente.direccion);
                cmd.Parameters.AddWithValue("pais", objetoCliente.pais);
                cmd.Parameters.AddWithValue("tel", objetoCliente.telefono);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " CLIENTE REGISTRADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre");
            return View(objetoCliente);
        }

        public ActionResult modificaCliente(int id)
        {
            ClienteO objetoCliente = ListClienteO().Where(c => c.codigo == id).FirstOrDefault();
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre", objetoCliente.pais);
            return View(objetoCliente);
        }

        [HttpPost]
        public ActionResult modificaCliente(ClienteO objetoCliente)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoCliente);
            }

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOCLIENTE", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoCliente.codigo);
                cmd.Parameters.AddWithValue("nom", objetoCliente.nombre);
                cmd.Parameters.AddWithValue("dir", objetoCliente.direccion);
                cmd.Parameters.AddWithValue("pais", objetoCliente.pais);
                cmd.Parameters.AddWithValue("tel", objetoCliente.telefono);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " CLIENTE MODIFICADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.pais = new SelectList(ListPais(), "codigo", "nombre", objetoCliente.pais);
            return View(objetoCliente);
        }

        public ActionResult detalleCliente(int id)
        {
            Cliente objetoCliente = ListCliente().Where(c => c.codigo == id).FirstOrDefault();
            return View(objetoCliente);
        }

        public ActionResult eliminaCliente(int id)
        {
            Cliente objetoCliente = ListCliente().Where(c => c.codigo == id).FirstOrDefault();
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_ELIMINACLIENTE", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoCliente.codigo);
                int estado = cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            return RedirectToAction("listadoClientes");
        }

        public int codigoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_SIGCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "CLIENTE");
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