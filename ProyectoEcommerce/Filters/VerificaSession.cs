using ProyectoEcommerce.Models;
using ProyectoEcommerce.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoEcommerce.Filters
{
    public class VerificaSession : ActionFilterAttribute
    {
        private UsuarioO usuario;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                usuario = (UsuarioO)HttpContext.Current.Session["user"];
                if(usuario == null)
                {
                    if (filterContext.Controller is AccesoController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("/Acceso/Login");
                    }
                }
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }
        }

    }
}