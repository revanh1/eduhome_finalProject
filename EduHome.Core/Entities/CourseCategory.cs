using EduHome.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
	public class CourseCategory:BaseModel
	{
		public int CategoryId { get; set; }
		public int CourseId { get; set; }	
		public Category? Category { get; set; }
		public Course? Course { get; set; }
	}
}
