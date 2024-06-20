using EduHome.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduHome.Core.Entities
{
	public class Course:BaseModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string AboutText { get; set; }
		[Required]
		public string Apply { get; set; }
		[Required]
		public string Certificiation { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }
		[Required]
		public double ClassDuration { get; set; }
		[Required] 
		public string SkillLevel { get; set; }
		[Required]
		public int CourseLanguageId { get; set; }
		[Required]
		public int StudentCount { get; set; }
		[Required]
		public double CourseFee { get; set; }
		[Required]
		public int CourseAssestsId { get; set; }
		public CourseAssests? courseAssests { get; set; }
		public CourseLanguage? CourseLanguage { get; set; }
		public List<CourseTag>? courseTags { get; set; }
		public List<CourseCategory>? courseCategories { get; set; }
		[NotMapped]
		public List<int> CategoryIds { get; set; }
		[NotMapped]
		public List<int> TagIds { get; set; }
		public string? Image { get; set; }
		[NotMapped]
		public IFormFile? file { get; set; }
	}
}
