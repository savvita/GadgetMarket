using Dapper;
using GadgetMarket.Exceptions;
using GadgetMarket.Model;
using System.Data;
using System.Data.SqlClient;

namespace GadgetMarket.Repositories
{

    public class UsersRepository : IUsersRepository
    {
        private DbConfig configuration;

        public UsersRepository(DbConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return (await connection.QueryAsync<User>("select * from Users")).ToList();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<User>("select * from Users where Id = @Id", new { Id = id });
        }

        public async Task<User> CreateUserAsync(User user)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            var id = await connection.QueryFirstOrDefaultAsync<int>(
                "if not exists(select Id from Users where Email = @Email) " +
                "begin " +
                "insert into Users values(@Email); " +
                "select SCOPE_IDENTITY(); " +
                "end " +
                "else select 0;", user);

            if (id == 0)
            {
                throw new UserEmailConflictException();
            }

            user.Id = id;
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Users set [Email] = @Email where Id = @Id", user);

            if (rows == 0)
            {
                throw new UserNotFoundException(user.Id);
            }

            return user;
        }

        public async Task<bool> RemoveUserByIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            await connection.ExecuteAsync("delete Gadget where OwnerId = @OwnerId", new { OwnerId = id });

            int rows = await connection.ExecuteAsync("delete Users where Id = @Id", new { Id = id });

            if (rows == 0)
            {
                throw new UserNotFoundException(id);
            }

            return true;
        }

        public async Task<User?> SignInAsync(User user)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<User>("select * from Users where [Email] = @Email;", new { user.Email });
        }
    }
}
