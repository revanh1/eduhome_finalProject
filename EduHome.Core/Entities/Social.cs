using EduHome.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
    public class Social:BaseModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Link { get; set; }
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }   
    }
}
