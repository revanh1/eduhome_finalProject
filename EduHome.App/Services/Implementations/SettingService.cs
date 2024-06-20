using EduHome.App.Context;
using EduHome.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;

namespace EduHome.App.Services.Implementations
{
    public class SettingService
    {
        public readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingService(EduHomeDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<Service> GetSetting()
        {
            Service? service = await _context.Services
                .Include(x=>x.Socials)
                   .FirstOrDefaultAsync();
            return service;
        }
    }
}
