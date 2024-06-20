using EduHome.Core.Entities;

namespace EduHome.App.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Notice> Notices { get; set; }
        public IEnumerable<Teacher> Teachers { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
        public Service Service { get; set; }
    }
}
