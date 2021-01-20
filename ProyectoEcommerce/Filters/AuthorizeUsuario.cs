using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using ProyectoEcommerce.Models;

namespace ProyectoEcommerce.Filters
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeUsuario : AuthorizeAttribute
    {
        private UsuarioO usuario;
        private int idOperacion;

        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["CNX"].ConnectionString);

        List<RolOperacion> ListRolOperacion()
        {
            List<RolOperacion> aRolOperacion = new List<RolOperacion>();
            SqlCommand cmd = new SqlCommand("SP_BUSCAROPERACIONXROL", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                aRolOperacion.Add(new RolOperacion()
                {
                    codigo = int.Parse(dr[0].ToString()),
                    rol = int.Parse(dr[1].ToString()),
                    operacion = int.Parse(dr[2].ToString())
                });
            }

            dr.Close();
            cn.Close();
            return aRolOperacion;
        }

        public AuthorizeUsuario(int idOperacion = 0)
        {
            this.idOperacion = idOperacion;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            
            try
            {
                usuario = (UsuarioO)HttpContext.Current.Session["user"];
                var operaciones = ListRolOperacion().Where(c => c.operacion == idOperacion && c.rol == usuario.rol).FirstOrDefault();
                /*
                if(operaciones == null)
                {
                    filterContext.Result = new RedirectResult("~/Error/NoAutorizado");
                }*/
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("~/Error/NoAutorizado");
            }
            //
        }
    }
}