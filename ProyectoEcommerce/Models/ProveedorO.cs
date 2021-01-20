using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class ProveedorO
    {
        [DisplayName("CÓDIGO")]
        public int codigo { get; set; }
        [DisplayName("NOMBRE DE PROVEEDOR")]
        public String nombre { get; set; }
        [DisplayName("DIRECCIÓN")]
        public String direccion { get; set; }
        [DisplayName("NOMBRE DE CONTACTO")]
        public String nomcontacto { get; set; }
        [DisplayName("CARGO DE CONTACTO")]
        public String carcontacto { get; set; }
        [DisplayName("PAÍS")]
        public int pais { get; set; }
        [DisplayName("TELÉFONO")]
        public String telefono { get; set; }
    }
}