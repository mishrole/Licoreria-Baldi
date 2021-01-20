using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class EmpleadoO
    {
        [DisplayName("CÓDIGO")]
        public int codigo { get; set; }
        [DisplayName("NOMBRE DE EMPLEADO")]
        public String nombre { get; set; }
        [DisplayName("FECHA DE NACIMIENTO")]
        public String nacimiento { get; set; }
        [DisplayName("DIRECCIÓN")]
        public String direccion { get; set; }
        [DisplayName("DISTRITO")]
        public int distrito { get; set; }
        [DisplayName("TELÉFONO")]
        public String telefono { get; set; }
        [DisplayName("CARGO")]
        public int cargo { get; set; }
        [DisplayName("FECHA DE CONTRATACIÓN")]
        public String contratacion { get; set; }
        [DisplayName("FOTO")]
        public String foto { get; set; }
    }
}