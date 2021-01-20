using ProyectoEcommerce.Filters;
using ProyectoEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoEcommerce.Controllers
{
    public class ProductoController : Controller
    {

        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        // Productos
        List<Producto> ListProducto()
        {
            List<Producto> aProductos = new List<Producto>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPRODUCTOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            String fotourl = "";

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    if(dr[8].ToString() == "")
                    {
                        fotourl = "~/fotos/nofoto.jpg";
                    } else
                    {
                        fotourl = dr[8].ToString();
                    }

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
                        foto = fotourl
                    }) ;
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

        List<ProductoO> ListProductoO()
        {
            List<ProductoO> aProductos = new List<ProductoO>();
            SqlCommand cmd = new SqlCommand("SP_PRODUCTOS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    aProductos.Add(new ProductoO()
                    {
                        codigo = int.Parse(dr[0].ToString()),
                        nombre = dr[1].ToString(),
                        proveedor = int.Parse(dr[2].ToString()),
                        categoria = int.Parse(dr[3].ToString()),
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

        List<ProveedorC> ListProveedor()
        {
            List<ProveedorC> aProveedores = new List<ProveedorC>();
            SqlCommand cmd = new SqlCommand("SP_LISTARPROVEEDORES_COMBO", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aProveedores.Add(new ProveedorC()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    nombre = dr[1].ToString()
                });
            }

            dr.Close();
            cn.Close();
            return aProveedores;

        }

        /*
        public ActionResult Index()
        {
            return View();
        }
        */

        //[AuthorizeUsuario(idOperacion:1)]
        public ActionResult listadoProductos()
        {
            return View(ListProducto());
        }

        //[AuthorizeUsuario(idOperacion:2)]
        public ActionResult nuevoProducto()
        {
            ViewBag.categoria = new SelectList(ListCategoria(), "codigo", "nombre");
            ViewBag.proveedor = new SelectList(ListProveedor(), "codigo", "nombre");
            ViewBag.codigo = codigoActual();
            return View(new ProductoO());
        }
        [HttpPost]
        public ActionResult nuevoProducto(ProductoO objetoProducto)
        {
            if(!ModelState.IsValid)
            {
                return View(objetoProducto);
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
                        fotourl = "~/fotos_productos/" + imgname;
                    }
                }
            }

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOPRODUCTO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoProducto.codigo);
                cmd.Parameters.AddWithValue("@nom", objetoProducto.nombre);
                cmd.Parameters.AddWithValue("@prov", objetoProducto.proveedor);
                cmd.Parameters.AddWithValue("@cat", objetoProducto.categoria);
                cmd.Parameters.AddWithValue("@cxu", objetoProducto.canxunidad);
                cmd.Parameters.AddWithValue("@pre", objetoProducto.precio);
                cmd.Parameters.AddWithValue("@uex", objetoProducto.existencia);
                cmd.Parameters.AddWithValue("@upe", objetoProducto.pedido);
                cmd.Parameters.AddWithValue("@fot", fotourl);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " PRODUCTO REGISTRADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.categoria = new SelectList(ListCategoria(), "codigo", "nombre");
            ViewBag.proveedor = new SelectList(ListProveedor(), "codigo", "nombre");
            return View(objetoProducto);

        }

        //[AuthorizeUsuario(idOperacion:3)]
        public ActionResult modificaProducto(int id)
        {
            ProductoO objetoProducto = ListProductoO().Where(c => c.codigo == id).FirstOrDefault();
            ViewBag.categoria = new SelectList(ListCategoria(), "codigo", "nombre", objetoProducto.categoria);
            ViewBag.proveedor = new SelectList(ListProveedor(), "codigo", "nombre", objetoProducto.proveedor);
            return View(objetoProducto);
        }

        [HttpPost]
        public ActionResult modificaProducto(ProductoO objetoProducto)
        {
            if (!ModelState.IsValid)
            {
                return View(objetoProducto);
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
                        fotourl = "~/fotos_productos/" + imgname;
                    }
                }
                else
                {
                    ProductoO fotoProducto = ListProductoO().Where(c => c.codigo == objetoProducto.codigo).FirstOrDefault();
                    fotourl = fotoProducto.foto;

                    if (fotourl == null)
                    {
                        fotourl = "~/fotos/nofoto.jpg";
                    }
                }
            }
            else
            {
                fotourl = "~/fotos/nofoto.jpg";
            }

            cn.Open();
            ViewBag.mensaje = "";
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_MANTENIMIENTOPRODUCTO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoProducto.codigo);
                cmd.Parameters.AddWithValue("@nom", objetoProducto.nombre);
                cmd.Parameters.AddWithValue("@prov", objetoProducto.proveedor);
                cmd.Parameters.AddWithValue("@cat", objetoProducto.categoria);
                cmd.Parameters.AddWithValue("@cxu", objetoProducto.canxunidad);
                cmd.Parameters.AddWithValue("@pre", objetoProducto.precio);
                cmd.Parameters.AddWithValue("@uex", objetoProducto.existencia);
                cmd.Parameters.AddWithValue("@upe", objetoProducto.pedido);
                cmd.Parameters.AddWithValue("@fot", fotourl);

                int estado = cmd.ExecuteNonQuery();
                tr.Commit();

                ViewBag.mensaje = estado.ToString() + " PRODUCTO MODIFICADO";
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            ViewBag.categoria = new SelectList(ListCategoria(), "codigo", "nombre");
            ViewBag.proveedor = new SelectList(ListProveedor(), "codigo", "nombre");
            return View(objetoProducto);
        }

        //[AuthorizeUsuario(idOperacion: 4)]
        public ActionResult detalleProducto(int id)
        {
            Producto objetoProducto = ListProducto().Where(c => c.codigo == id).FirstOrDefault();
            return View(objetoProducto);
        }

        //[AuthorizeUsuario(idOperacion: 5)]
        public ActionResult eliminaProducto(int id)
        {
            Producto objetoProducto = ListProducto().Where(c => c.codigo == id).FirstOrDefault();
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                SqlCommand cmd = new SqlCommand("SP_ELIMINAPRODUCTO", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", objetoProducto.codigo);
                int estado = cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }

            cn.Close();
            return RedirectToAction("listadoProductos");
        }

        public int codigoActual()
        {
            int ultimo = 0;
            SqlCommand cmd = new SqlCommand("SP_SIGCOD", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tabla", "PRODUCTO");
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