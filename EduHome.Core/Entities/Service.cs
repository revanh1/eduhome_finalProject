using EduHome.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
    public class Service:BaseModel
    {
        [Required]
        public string AboutText { get; set; }
        [Required]  
        public string AboutTitle { get; set; }
        public string? AboutImage { get; set; }
        public string VideoLink { get; set; }
        public string HeaderLogo { get; set; }
        public string FooterLogo { get; set; }
        [Required]
        public string Number1 { get; set; }
        [Required]
        public string Number2 { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }

        [NotMapped]
        public IFormFile? file { get; set; }
        public List<Social>? Socials { get; set; }
    }
}
