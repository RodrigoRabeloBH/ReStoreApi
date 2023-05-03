using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace API.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase { }

}