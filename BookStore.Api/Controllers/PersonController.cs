using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Get()
        {
            var fruits = await Task.FromResult(new string[] { "Biplob", "Hasan", "Rakib" });
            return Ok(fruits);
        }

        [HttpGet]
        [Route("GetAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmin()
        {
            var fruits = await Task.FromResult(new string[] { "Dhaka", "Rajshahi", "Jessore" });
            return Ok(fruits);
        }
    }
}
