using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class Item
    {
        [DisplayName("CÓDIGO")]
        public int codigo { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public String descripcion { get; set; }
        [DisplayName("PRECIO")]
        public Double precio { get; set; }
        [DisplayName("CANTIDAD")]
        public int cantidad { get; set; }
        [DisplayName("SUBTOTAL")]
        public Double subtotal { get { return cantidad * precio; } }
        [DisplayName("FOTO")]
        public String foto { get; set; }
    }
}