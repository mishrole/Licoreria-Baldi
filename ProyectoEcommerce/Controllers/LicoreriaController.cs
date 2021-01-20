using ProyectoEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace ProyectoEcommerce.Controllers
{
    public class LicoreriaController : Controller
    {

        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        // Información del cliente

        List<UsuarioO> ListUsuario()
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

        // Productos
        List<Producto> ListProducto()
        {
            List<Producto> aProductos = new List<Producto>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPRODUCTOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aProductos.Add(new Producto()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        proveedor = dr[2].ToString(),
                        categoria = dr[3].ToString(),
                        canxunidad = dr[4].ToString(),
                        precio = double.Parse(dr[5].ToString()),
                        existencia = int.Parse(dr[6].ToString()),
                        pedido = int.Parse(dr[7].ToString()),
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

            return aProductos;
        }

        List<Producto> ListProductoxCatxOrd(int cat, int ord)
        {
            List<Producto> aProductos = new List<Producto>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPRODUCTOSXCATXORD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@cat", cat);
            cmd.Parameters.AddWithValue("@ord", ord);
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aProductos.Add(new Producto()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        proveedor = dr[2].ToString(),
                        categoria = dr[3].ToString(),
                        canxunidad = dr[4].ToString(),
                        precio = double.Parse(dr[5].ToString()),
                        existencia = int.Parse(dr[6].ToString()),
                        pedido = int.Parse(dr[7].ToString()),
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

            return aProductos;
        }

        // Distritos
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

        // Categorías
        List<Categoria> ListCategoria()
        {
            List<Categoria> aCategorias = new List<Categoria>();
            SqlCommand cmd = new SqlCommand("SP_LISTARCATEGORIAS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aCategorias.Add(new Categoria()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aCategorias;
        }

        // Orden
        List<Orden> ListOrden()
        {
            List<Orden> aOrden = new List<Orden>();
            SqlCommand cmd = new SqlCommand("SP_LISTARORDEN", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aOrden.Add(new Orden()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aOrden;
        }

        // Pedido con datos en String
        List<Pedido> ListPedido()
        {
            List<Pedido> aPedidos = new List<Pedido>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPEDIDOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aPedidos.Add(new Pedido()
                {
                    pedido = int.Parse(dr[0].ToString()),
                    cliente = dr[1].ToString(),
                    fechapedido = dr[2].ToString(),
                    distrito = dr[3].ToString(),
                    direccion = dr[4].ToString(),
                    referencia = dr[5].ToString(),
                    monto = dr[6].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aPedidos;
        }

        public ActionResult Index()
        {
            return View();
        }

        // Venta de Producto
        /*
        public ActionResult carritoCompras()
        {
            if(Session["carrito"] == null)
            {
                Session["carrito"] = new List<Item>();
            }

            return View(ListProducto());
        }
        */
        public ActionResult carritoCompras(int cat = 0, int ord = 0)
        {
            if (Session["carrito"] == null)
            {
                Session["carrito"] = new List<Item>();
            }

            ViewBag.cat = cat;
            ViewBag.ord = ord;
            ViewBag.categorias = new SelectList(ListCategoria(), "codigo", "nombre");
            ViewBag.orden = new SelectList(ListOrden(), "codigo", "nombre");

            return View(ListProductoxCatxOrd(cat, ord));
        }

        public ActionResult seleccionaProducto(int id)
        {
            Producto objetoProducto = ListProducto().Where(c => c.codigo == id).FirstOrDefault();
            return View(objetoProducto);
        }

        // Enviar el producto seleccionado al carrito
        public ActionResult agregaProducto(int id, int cant = 1)
        {
            var producto = ListProducto().Where(c => c.codigo == id).FirstOrDefault();
            // Enviar información a la clase Item
            Item objetoItem = new Item()
            {
                codigo = producto.codigo,
                descripcion = producto.categoria + " " +producto.nombre + " " + producto.canxunidad,
                precio = producto.precio,
                cantidad = cant,
                foto = producto.foto
            };

            var carrito = (List<Item>)Session["carrito"];
            carrito.Add(objetoItem);
            Session["carrito"] = carrito;

            // Redireccionar
            return RedirectToAction("carritoCompras");
        }

        // Calcular total
        public ActionResult comprarProducto()
        {
            if(Session["carrito"] == null)
            {
                return RedirectToAction("carritoCompras");
            }

            var carrito = (List<Item>) Session["carrito"];
            ViewBag.total = carrito.Sum( sub => sub.subtotal);
            return View(carrito);
        }

        // Eliminar producto de la compra
        public ActionResult eliminaProductoCarrito(int? id = 0)
        {
            if(id == null)
            {
                return RedirectToAction("carritoCompras");
            }

            var carrito = (List<Item>)Session["carrito"];
            var item = carrito.Where(c => c.codigo == id).FirstOrDefault();
            carrito.Remove(item);


            Session["carrito"] = carrito;
            return RedirectToAction("comprarProducto");
        }

        // Finalizar la compra
        public ActionResult FinalizarCompra()
        {
            if (Session["carrito"] == null)
            {
                return RedirectToAction("carritoCompras");
            }

            var carrito = (List<Item>)Session["carrito"];

            if (carrito.Sum(sub => sub.subtotal) < 1)
            {
                return RedirectToAction("carritoCompras");
            }

            var usuarioLogueado = (int)Session["codigousuario"];
            var cliente = ListUsuario().Where(u => u.codigo == usuarioLogueado).FirstOrDefault();

            ViewBag.cliente = cliente.codigo;
            ViewBag.nombre = cliente.nombre;
            ViewBag.dni = cliente.dni;
            ViewBag.email = cliente.email;
            ViewBag.telefono = cliente.telefono;
            ViewBag.pedido = codigoActual();
            ViewBag.fecha = DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.distrito = new SelectList(ListDistrito(), "codigo", "nombre");
            return View(new PedidoO());
        }

        [HttpPost]
        public ActionResult FinalizarCompra(PedidoO objetoPedido)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            /* Pedido */
            var usuarioLogueado = (int)Session["codigousuario"];
            List<Item> carrito = (List<Item>)Session["carrito"];
            var monto = carrito.Sum(sub => sub.subtotal);
            var codigoPedido = codigoActual();
            var cliente = ListUsuario().Where(u => u.codigo == usuarioLogueado).FirstOrDefault();
            var distrito = ListDistrito().Where(d => d.codigo == objetoPedido.distrito).FirstOrDefault();

            cn.Open();
            ViewBag.mensaje = "";
            
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            int estadoPedido;
            int estadoDetalle;

            try
            {
                SqlCommand cmdPedido = new SqlCommand("SP_NUEVOPEDIDO", cn, tr);
                cmdPedido.CommandType = CommandType.StoredProcedure;
                cmdPedido.Parameters.AddWithValue("@cliente", usuarioLogueado);
                cmdPedido.Parameters.AddWithValue("@distrito", objetoPedido.distrito);
                cmdPedido.Parameters.AddWithValue("@direccion", objetoPedido.direccion);
                cmdPedido.Parameters.AddWithValue("@referencia", objetoPedido.referencia);
                cmdPedido.Parameters.AddWithValue("@monto", monto);
                estadoPedido = cmdPedido.ExecuteNonQuery();

                try
                {
                    /* Detalle de Pedido */
                    foreach (var item in carrito)
                    {
                        SqlCommand cmdDetalle = new SqlCommand("SP_DETALLEPEDIDO", cn, tr);
                        cmdDetalle.CommandType = CommandType.StoredProcedure;
                        cmdDetalle.Parameters.AddWithValue("@pedido", codigoPedido);
                        cmdDetalle.Parameters.AddWithValue("@producto", item.codigo);
                        cmdDetalle.Parameters.AddWithValue("@precio", item.precio);
                        cmdDetalle.Parameters.AddWithValue("@cantidad", item.cantidad);
                        estadoDetalle = cmdDetalle.ExecuteNonQuery();
                    }
                }
                catch (Exception exD)
                {
                    ViewBag.mensaje = exD.Message;
                    //tr.Rollback(); Si colocamos doble rollback, se cae por duplicidad
                    // Ya es controlado por el rollback del siguiente catch
                }

                tr.Commit();

                if(estadoPedido > 0)
                {
                    Pedido pedidoActual = new Pedido();
                    pedidoActual.pedido = codigoPedido;
                    pedidoActual.fechapedido = ""+DateTime.Now.ToString("yyyy-MM-dd");
                    pedidoActual.distrito = distrito.nombre;
                    pedidoActual.direccion = objetoPedido.direccion;
                    pedidoActual.referencia = objetoPedido.referencia;
                    pedidoActual.monto = "S/. "+monto;
                    // Se envía email con datos del pedido
                    EnviarConfirmacion(cliente, pedidoActual, carrito);
                }

                /*var pedido = ListPedido().Where(p => p.pedido == codigoPedido).FirstOrDefault();

                if(pedido != null)
                {
                    var cliente = ListUsuario().Where(u => u.codigo == usuarioLogueado).FirstOrDefault();
                    EnviarConfirmacion(cliente.email, pedido);
                }*/

                ViewBag.mensaje = estadoPedido.ToString() + " PEDIDO REALIZADO CON ÉXITO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }finally
            {
                cn.Close();
                // Una vez realizada la compra, el carrito se limpia
                Session["carrito"] = null;
            }

            return RedirectToAction("PedidoConfirmado");
        }

        public ActionResult PedidoConfirmado()
        {
            return View();
        }
        /*
        public int pedidoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_ACTCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "PEDIDO");
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
        */
        public int codigoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_SIGCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "PEDIDO");
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

        #region HELPERS
        private void EnviarConfirmacion(UsuarioO objetoCliente, Pedido objetoPedido, List<Item> carrito)
        {
            string origin = "mishroletestmail@gmail.com";
            string pass = "Zhg85V*R1C04";
            // Strings para formar la tabla con toda la información del pedido
            string parte1 = "<body><table cellpadding='0' cellspacing='0' width='640' align='center' border='0'><tr><td><table cellpadding='0' cellspacing='0' width='640' align='left' border='0'><tr><td><h3 align='center'><strong>Hola " + objetoCliente.nombre + "</strong></h3><p align='center'>Tu Pedido en Baldí ha sido confirmado</p></td></tr></table>";
            string parte2 = "<table cellpadding='0' cellspacing='0' width='640' align='left' border='0'><tr><td><br><p style='color:gray;'><strong>INFORMACIÓN DEL PEDIDO</strong></p><hr style='color:gray;'></td></tr></table><table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>ID del Pedido: </strong>" + objetoPedido.pedido + "</p></td></tr></table>";
            string parte3 = "<table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Fecha del Pedido: </strong>" + objetoPedido.fechapedido + "</p></td></tr></table><table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Nombre de cliente: </strong> " + objetoCliente.nombre + "</p></td></tr></table>";
            string parte4 = "<table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Facturado a: <a href='" + objetoCliente.email + "'>" + objetoCliente.email + "</a></strong></p></td></tr></table>";
            string parte5 = "<table cellpadding='0' cellspacing='0' width='640' align='left' border='0'><tr><td><br><p style='color:gray;'><strong>DATOS DE LA ENTREGA</strong></p><hr style='color:gray;'></td></tr></table><table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Entrega estimada: </strong>24 horas</p></td></tr></table>";
            string parte6 = "<table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Distrito: </strong>" + objetoPedido.distrito + "</p></td></tr></table><table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Dirección: </strong>" + objetoPedido.direccion + "</p></td></tr></table><table cellpadding='0' cellspacing='0' width='320' align='left' border='0'><tr><td align='left'><p><strong>Referencia: </strong>" + objetoPedido.referencia + "</p></td></tr></table>";
            string parte7 = "<table cellpadding='0' cellspacing='0' width='640' align='left' border='0'><tr><td><br><p style='color:gray;'><strong>ESTE ES TU PEDIDO</strong></p><hr style='color:gray;'></td></tr></table><table cellpadding='0' cellspacing='0' width='640' align='left' border='0'><tr><td><strong>DESCRIPCIÓN</strong></td><td><strong>CANT</strong></td><td><strong>PRECIO</strong></td></tr>";
            string parte9 = "</table><table cellpadding='0' cellspacing='0' width='640' align='left' border='0'><tr><td><br><br><h3 align='center'><strong>TOTAL DEL PEDIDO: " + objetoPedido.monto + "</strong></h3></td></tr></table></td></tr></table>";
            string parte10 = "<table cellpadding='0' cellspacing='0' width='640' align='center' border='0'><tr><td><br><br><h3 align='center'><strong>Te informaremos cuando tu pedido esté en camino</strong></h3></td></tr></table></td></tr></table></body>";
            
            // Listado de items
            //string parte8 = "<tr><td>Producto 1</td><td>1</td><td>S/. 15.00</td></tr>";
            string parte8 = "";
            

            foreach (var item in carrito)
            {
                parte8 += "<tr><td>" + item.descripcion + "</td><td>" + item.cantidad + "</td><td>S/. " + item.precio + "</td></tr>";
            }


            MailMessage mensajeEmail = new MailMessage(origin, objetoCliente.email,
                "Nuevo pedido en Baldí", parte1+parte2+parte3+parte4+parte5+parte6+parte7+parte8+parte9+parte10
                );;

            /*
                "<h1><strong>¡Gracias!</strong></h1></br>" +
                "<h2><strong>Hola " + objetoCliente.nombre + "</strong></h2>" +
                "<p>Tu pedido en Baldí ha sido confirmado</p></br></br>" +
                "<p>INFORMACIÓN DEL PEDIDO:</p><hr>" +
                "<p><strong>ID del Pedido: </strong>" + objetoPedido.pedido + "</p>" +
                "<p><strong>Fecha del Pedido: </strong> " + objetoPedido.fechapedido + "</p>" +
                "<p><strong>Facturado a: </strong> " + objetoCliente.email + "</p></br></br>" +
                "<p>ESTE ES TU PEDIDO:</p><hr>" +
                "<p><strong>" + carrito[0].codigo +"</strong></p>" +
                ""*/

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