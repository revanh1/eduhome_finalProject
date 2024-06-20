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
    public class Slider:BaseModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public string? Image { get; set; }
        public string Link { get; set; }
        [NotMapped]
        public IFormFile? file { get; set; }
        public bool isActive { get; set; }


    }
}
