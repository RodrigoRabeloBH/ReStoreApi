using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ReStore.Api.Controllers
{
    [AllowAnonymous]
    public class FallbackController : Controller
    {
        public IActionResult Index()
        {
            var diretory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

            return PhysicalFile(diretory, "text/HTML");
        }
    }
}
