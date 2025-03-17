using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Get()
        {
            var fruits = await Task.FromResult(new string[] { "History", "Funny", "Secienc Fiction" });
            return Ok(fruits);
        }

        [HttpGet]
        [Route("GetAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmin()
        {
            var fruits = await Task.FromResult(new string[] { "History", "Funny", "Secienc Fiction" });
            return Ok(fruits);
        }

        [HttpGet]
        [Route("test")]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            var fruits = await Task.FromResult(new string[] { "History", "Funny", "Secienc Fiction" });
            return Ok(fruits);
        }
    }
}
