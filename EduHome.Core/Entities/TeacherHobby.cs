using EduHome.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
    public class TeacherHobby:BaseModel
    {
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public int HobbyId { get; set; }
        public Teacher? Teacher { get; set; }
        public Hobby? Hobby { get; set; }
    }
}
