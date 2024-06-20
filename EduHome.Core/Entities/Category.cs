using EduHome.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
	public class Category:BaseModel
	{
		[Required]
		public string Name { get; set; }
		public List<CourseCategory>? courseCategories { get; set; }
		public List<BlogCategory>? blogCategories { get; set; }

    }
}
