using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoEcommerce.Models
{
    public class EmailReset
    {
        [EmailAddress]
        [Required]
        [DisplayName("Email")]
        public string email { get; set; }
    }
}