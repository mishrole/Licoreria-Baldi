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
    public class PedidoController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);
        
        List<PedidoO> ListPedidos()
        {
            List<PedidoO> aPedidos = new List<PedidoO>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPEDIDOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aPedidos.Add(new PedidoO()
                    {
                        pedido = int.Parse(dr[0].ToString()),
                        cliente = int.Parse(dr[1].ToString()),
                        fechapedido = dr[2].ToString(),
                        fechaentrega = dr[3].ToString(),
                        distrito = int.Parse(dr[4].ToString()),
                        direccion = dr[5].ToString(),
                        referencia = dr[6].ToString(),
                        monto = Double.Parse(dr[7].ToString()),
                        empleado = int.Parse(dr[8].ToString())
                    });
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aPedidos;
        }

        List<Pedido> ListPedidosxUsuario()
        {
            var id = (int)Session["codigousuario"];
            List<Pedido> aPedidos = new List<Pedido>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPEDIDOSXUSUARIO", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@usuario", id);
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aPedidos.Add(new Pedido()
                    {
                        pedido = int.Parse(dr[0].ToString()),
                        cliente = dr[1].ToString(),
                        fechapedido = dr[2].ToString(),
                        fechaentrega = dr[3].ToString(),
                        distrito = dr[4].ToString(),
                        direccion = dr[5].ToString(),
                        referencia = dr[6].ToString(),
                        monto = dr[7].ToString()
                    });
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return aPedidos;
        }

        public ActionResult listadoPedidos()
        {
            return View(ListPedidosxUsuario());
        }
    }
}