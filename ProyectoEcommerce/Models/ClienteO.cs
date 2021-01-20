using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class ClienteO
    {
        [DisplayName("CÓDIGO")]
        public int codigo { get; set; }

        [DisplayName("NOMBRE DE CLIENTE")]
        public String nombre { get; set; }

        [DisplayName("DIRECCIÓN")]
        public String direccion { get; set; }

        [DisplayName("PAÍS")]
        public int pais { get; set; }

        [DisplayName("TELÉFONO")]
        public String telefono { get; set; }
    }
}