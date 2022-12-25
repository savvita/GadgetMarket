//using Microsoft.AspNetCore.Mvc;
//using GadgetMarket.Model;
//using System.Security.Claims;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.AspNetCore.Authorization;
//using System.Text.Json.Serialization;
//using GadgetMarket.Web.Model;
//using GadgetMarket.Exceptions;

//namespace GadgetMarket.Web.Controllers
//{
//    [ApiController]
//    [Route("users2")]
//    public class User2Controller : ControllerBase
//    {
//        //todo: crete custom config object use IoC to inject here
//        private readonly IUsersRepository usersRepository;

//        public User2Controller(IUsersRepository usersRepository)
//        {
//            this.usersRepository = usersRepository;
//        }

//        //TODO check this
//        //GET /users2/
//        [HttpGet("")]
//        public async Task<List<User>> GetAllUsers()
//        {
//            return await usersRepository.GetAllUsersAsync();
//        }

//        [HttpGet("{id:int}")]
//        [Authorize]
//        public async Task<User> GetUserById(int id)
//        {
//            if (this.User.Identity == null)
//            {
//                throw new ForbiddenException();
//            }

//            //TODO admin can get other user

//            if (int.TryParse(this.User.Identity.Name, out int userId))
//            {
//                if (id != userId)
//                {
//                    throw new ForbiddenException();
//                }

//                var user = await usersRepository.GetUserByIdAsync(userId);
//                if (user == null)
//                {
//                    throw new UserNotFoundException(userId);
//                }

//                return user;
//            }

//            throw new Exception("bad request");
//        }

//        [HttpPost("signup")]
//        // POST /users2/signup
//        public async Task<UserTokenModel> SignUp([FromBody] string email)
//        {
//            User user = new User
//            {
//                Email = email
//            };

//            user = await usersRepository.CreateUserAsync(user);

//            var jwt = GetToken(user.Id);

//            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

//            return new UserTokenModel
//            {
//                Id = user.Id,
//                UserEmail = email,
//                AccessToken = encodedJwt
//            };
//        }

//        public class UserTokenModel
//        {

//            [JsonPropertyName("id")]
//            public int Id { get; set; }

//            [JsonPropertyName("user_email")]
//            public string UserEmail { get; set; }

//            [JsonPropertyName("access_token")]
//            public string AccessToken { get; set; }
//        }

//        ////TODO check this
//        //[HttpPost("")]
//        //[Authorize]
//        //// POST /users2
//        //public async Task<IResult> UpdateUser(User user)
//        //{
//        //    if (this.User.Identity == null)
//        //    {
//        //        return Results.Forbid();
//        //    }

//        //    if (int.TryParse(this.User.Identity.Name, out int userId))
//        //    {
//        //        if (userId != user.Id)
//        //        {
//        //            return Results.Forbid();
//        //        }

//        //        using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
//        //        {
//        //            try
//        //            {
//        //                int rows = await connection.ExecuteAsync("update Users set [Email] = @Email where Id = @Id", user);
//        //                return rows > 0 ? Results.Ok() : Results.NotFound();
//        //            }
//        //            catch
//        //            {
//        //                return Results.StatusCode(500);
//        //            }
//        //        }
//        //    }
//        //    return Results.StatusCode(500);
//        //}

//        ////TODO check this
//        //[HttpDelete("{id:int}")]
//        //[Authorize]
//        //// DELETE /users2/22
//        //public async Task<IResult> RemoveUserById(int id)
//        //{
//        //    if (this.User.Identity == null)
//        //    {
//        //        return Results.Forbid();
//        //    }

//        //    //TODO admin can remove other user

//        //    if (int.TryParse(this.User.Identity.Name, out int userId))
//        //    {
//        //        if (id != userId)
//        //        {
//        //            return Results.Forbid();
//        //        }

//        //        using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
//        //        {
//        //            try
//        //            {
//        //                int rows = await connection.ExecuteAsync("delete Users where Id = @Id", new { Id = id });
//        //                return rows > 0 ? Results.Ok() : Results.NotFound();
//        //            }
//        //            catch
//        //            {
//        //                return Results.StatusCode(500);
//        //            }
//        //        }
//        //    }
//        //    return Results.StatusCode(500);
//        //}

//        //[HttpPost("singin")]
//        //public async Task<IResult> SignIn([FromBody]string email)
//        //{
//        //    User? user = null;

//        //    using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
//        //    {
//        //        try
//        //        {
//        //            user = await connection.QueryFirstOrDefaultAsync<User>("select * from Users where [Email] = @Email;", new { Email = email });
//        //        }
//        //        catch
//        //        {
//        //            return Results.StatusCode(500);
//        //        }
//        //    }

//        //    if (user != null)
//        //    {
//        //        var jwt = GetToken(user.Id);

//        //        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

//        //        var response = new
//        //        {
//        //            id = user.Id,
//        //            user_email = email,
//        //            access_token = encodedJwt
//        //        };

//        //        return Results.Ok(response);
//        //    }
//        //    else
//        //    {
//        //        return Results.BadRequest("Invalid email");
//        //    }
//        //}


//        private JwtSecurityToken GetToken(int userId)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString())
//            };

//            ClaimsIdentity identity = new ClaimsIdentity(claims, "Token");

//            var now = DateTime.UtcNow;

//            return new JwtSecurityToken(
//                issuer: Auth.ISSUER,
//                audience: Auth.AUDIENCE,
//                notBefore: now,
//                claims: identity.Claims,
//                expires: now.Add(TimeSpan.FromMinutes(Auth.LIFETIME)),
//                signingCredentials: new SigningCredentials(Auth.GetSecurityKey(), SecurityAlgorithms.HmacSha256));
//        }
//    }

//}
