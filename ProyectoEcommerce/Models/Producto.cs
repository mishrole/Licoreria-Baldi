using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class Producto
    {
        [DisplayName("CÓDIGO")]
        public int codigo { get; set; }
        [DisplayName("PRODUCTO")]
        public String nombre { get; set; }
        [DisplayName("PROVEEDOR")]
        public String proveedor { get; set; }
        [DisplayName("CATEGORÍA")]
        public String categoria { get; set; }
        [DisplayName("CANTIDAD POR UNIDAD")]
        public String canxunidad { get; set; }
        [DisplayName("PRECIO")]
        public Double precio { get; set; }
        [DisplayName("STOCK ACTUAL")]
        public int existencia { get; set; }
        [DisplayName("EN PEDIDO")]
        public int pedido { get; set; }
        [DisplayName("FOTO")]
        public String foto { get; set; }
    }
}