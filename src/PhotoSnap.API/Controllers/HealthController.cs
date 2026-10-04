using Microsoft.AspNetCore.Mvc;

namespace PhotoSnap.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
   [HttpGet]
   public IActionResult Ping()
   {
      return Ok("Pong!");
   }

   [HttpGet("/Environment")]
   public IActionResult Environment()
   {
      throw new NotImplementedException(); 
   }
}