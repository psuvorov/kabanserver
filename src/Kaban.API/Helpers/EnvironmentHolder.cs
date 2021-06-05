using Kaban.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Kaban.API.Helpers
{
    public class EnvironmentHolder : IEnvironmentHolder
    {
        private readonly IWebHostEnvironment _env;

        public EnvironmentHolder(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string GetRootPath()
        {
            return _env.WebRootPath;
        }
    }
}