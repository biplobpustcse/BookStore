using BookStore.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Api.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AuthService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            this.configuration = _configuration;
        }
        public async Task<(int, string)> Registeration(Registration model, string role)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return (0, "User already exists");

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name
            };
            var createUserResult = await userManager.CreateAsync(user,model.Password);
            if (!createUserResult.Succeeded)
                return (0, string.Join("; ", createUserResult.Errors.Select(e => e.Description)));

            if(!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            await userManager.AddToRoleAsync(user, role);

            return (1, "User created successfully!");
        }

        public async Task<(int, string)> Login(Login model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
                return (0, "Invalid username");
            if (!await userManager.CheckPasswordAsync(user, model.Password))
                return (0, "Invalid password");

            var userRoles =await userManager.GetRolesAsync(user);

            var authClaims= new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = GenerateToken(authClaims);
            return (1, token);
        }

        #region private method
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]));

            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["Jwt:ValidIssuer"],
                Audience= configuration["Jwt:ValidAudience"],
                Expires= DateTime.UtcNow.AddHours(Convert.ToInt32(configuration["Jwt:ExpireHour"])),
                SigningCredentials = new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256),
                Subject=new ClaimsIdentity(claims)
            };
            var tokenHeader = new JwtSecurityTokenHandler();
            var token = tokenHeader.CreateToken(tokenDiscriptor);
            return tokenHeader.WriteToken(token);
        } 
        #endregion
    }
}
