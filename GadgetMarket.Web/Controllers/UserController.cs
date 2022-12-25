using Microsoft.AspNetCore.Mvc;
using GadgetMarket.Model;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using GadgetMarket.Web.Model;
using GadgetMarket.Exceptions;
using GadgetMarket.Repositories;

namespace GadgetMarket.Web.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private IUsersRepository usersRepository;

        public UserController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        //TODO check this
        [HttpGet("")]
        public async Task<List<User>> GetAllUsers()
        {
            return await usersRepository.GetAllUsersAsync();
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<User> GetUserById(int id)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            //TODO admin can get other user

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                if (id != userId)
                {
                    throw new ForbiddenException();
                }

                var user = await usersRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    throw new UserNotFoundException(id);
                }

                return user;
            }

            throw new BadRequestException();
        }

        [HttpPost("signup")]
        public async Task<UserTokenModel> SignUp([FromBody] string email)
        {
            User user = new User()
            {
                Email = email
            };

            user = await usersRepository.CreateUserAsync(user);

            var jwt = GetToken(user.Id);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new UserTokenModel
            {
                Id = user.Id,
                UserEmail = email,
                AccessToken = encodedJwt
            };
        }

        //TODO check this
        [HttpPut("")]
        [Authorize]
        public async Task<User> UpdateUser(User user)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                if (userId != user.Id)
                {
                    throw new ForbiddenException();
                }

                return await usersRepository.UpdateUserAsync(user);
            }

            throw new BadRequestException();
        }

        //TODO check this
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<bool> RemoveUserById(int id)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            //TODO admin can remove other user

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                if (id != userId)
                {
                    throw new ForbiddenException();
                }

                return await usersRepository.RemoveUserByIdAsync(id);
            }

            throw new BadRequestException();
        }

        [HttpPost("signin")]
        public async Task<UserTokenModel> SignIn([FromBody] string email)
        {
            User? user = await usersRepository.SignInAsync(new User()
            {
                Email = email
            });


            if (user == null)
            {
                throw new InvalidCredentialsException();
            }

            var jwt = GetToken(user.Id);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new UserTokenModel
            {
                Id = user.Id,
                UserEmail = email,
                AccessToken = encodedJwt
            };
        }


        private JwtSecurityToken GetToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Token");

            var now = DateTime.UtcNow;

            return new JwtSecurityToken(
                issuer: Auth.ISSUER,
                audience: Auth.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(Auth.LIFETIME)),
                signingCredentials: new SigningCredentials(Auth.GetSecurityKey(), SecurityAlgorithms.HmacSha256));
        }
    }
}
