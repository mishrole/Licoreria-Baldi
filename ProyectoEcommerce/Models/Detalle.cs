using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class Detalle
    {
        [DisplayName("PEDIDO")]
        public int pedido { get; set; }
        [DisplayName("PRODUCTO")]
        public int producto { get; set; }
        [DisplayName("PRECIO")]
        public Double precio { get; set; }
        [DisplayName("CANTIDAD")]
        public int cantidad { get; set; }
    }
}