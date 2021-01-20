using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class ResetPass
    {
        public String token { get; set; }
        [Required]
        [DisplayName("NUEVA CONTRASEÑA")]
        public String contraseña { get; set; }
        [DisplayName("CONFIRMAR CONTRASEÑA")]
        [Compare("contraseña")]
        [Required]
        public String contraseñaV { get; set; }
    }
}