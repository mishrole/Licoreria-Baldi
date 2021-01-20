using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class UsuarioO
    {
        [DisplayName("CÓDIGO")]
        public int codigo { get; set; }
        [DisplayName("USUARIO")]
        public String usuario { get; set; }
        [DisplayName("CONTRASEÑA")]
        public String contraseña { get; set; }
        [DisplayName("NOMBRE")]
        public String nombre { get; set; }
        [DisplayName("EMAIL")]
        public String email { get; set; }
        [DisplayName("DNI")]
        public String dni { get; set; }
        [DisplayName("TELÉFONO")]
        public String telefono { get; set; }
        [DisplayName("TOKEN")]
        public String token { get; set; }
        [DisplayName("ROL")]
        public int rol { get; set; }
    }
}