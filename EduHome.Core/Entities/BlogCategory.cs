using EduHome.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
	public class BlogCategory:BaseModel
	{
		public int CategoryId { get; set; }
		public int BlogId { get; set; }	
		public Category? Category { get; set; }
		public Blog? Blog { get; set; }
	}
}
