using BookStore.Data.Models;

namespace BookStore.Api.Services
{
    public interface IAuthService
    {
        Task<(int, string)> Registeration(Registration model, string role);
        Task<(int, string)> Login(Login model);
    }
}
