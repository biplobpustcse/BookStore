using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoreController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
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
