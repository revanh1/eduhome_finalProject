using EduHome.Core.Entities;

namespace EduHome.App.ViewModels
{
    public class AboutVM
    {
        public IEnumerable<Notice> Notices { get; set; }
        public IEnumerable<Teacher> Teachers { get; set; }
        public Service Service { get; set; }
    }
}
