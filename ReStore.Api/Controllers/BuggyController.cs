using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BuggyController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("not-found")]
        public ActionResult GetNotFound()
        {
            return NotFound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ProblemDetails { Title = "This is a bad request" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("unauthorized")]
        public ActionResult GetUnauthorized()
        {
            return Unauthorized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("validation-error")]
        public ActionResult GetValidationError()
        {
            ModelState.AddModelError("Problem1", "This is the first error");
            ModelState.AddModelError("Problem2", "This is the second error");

            return ValidationProblem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("server-error")]
        public ActionResult GetServerErrror()
        {
            throw new Exception("This is a server error");
        }
    }
}