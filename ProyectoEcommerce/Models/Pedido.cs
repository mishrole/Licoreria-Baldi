using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class Pedido
    {
        [DisplayName("PEDIDO")]
        public int pedido { get; set; }
        [DisplayName("CLIENTE")]
        public String cliente { get; set; }
        [DisplayName("FECHA DE PEDIDO")]
        public String fechapedido { get; set; }
        [DisplayName("FECHA DE ENTREGA")]
        public String fechaentrega { get; set; }
        [DisplayName("DISTRITO")]
        public String distrito { get; set; }
        [DisplayName("DIRECCIÓN")]
        public String direccion { get; set; }
        [DisplayName("REFERENCIA")]
        public String referencia { get; set; }
        [DisplayName("MONTO")]
        public String monto { get; set; }
        [DisplayName("EMPLEADO")]
        public String empleado { get; set; }
    }
}