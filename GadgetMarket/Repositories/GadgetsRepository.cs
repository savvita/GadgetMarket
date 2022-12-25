using Dapper;
using GadgetMarket.Exceptions;
using GadgetMarket.Model;
using System.Data;
using System.Data.SqlClient;

namespace GadgetMarket.Repositories
{
    public class GadgetsRepository : IGadgetsRepository
    {
        private DbConfig configuration;

        public GadgetsRepository(DbConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Gadget> CreateGadgetAsync(Gadget gadget)
        {
            if (gadget.Name.Equals(String.Empty) || gadget.Price <= 0)
            {
                throw new BadRequestException();
            }

            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var id = (await connection.QueryFirstAsync<int>("insert into Gadget values (@Name, @Price, @CategoryId, @OwnerId); select SCOPE_IDENTITY()",
                gadget));
            gadget.Id = id;
            return gadget;
        }

        public async Task<List<Gadget>> GetAllGadgetsAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var gadgets = (await connection.QueryAsync<Gadget, Category, Gadget>(
                "select Gadget.*, Categories.* from Gadget left join Categories on CategoryId = Categories.Id",
                (g, c) =>
                {
                    return new Gadget()
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Price = g.Price,
                        CategoryId = g.CategoryId,
                        Category = c
                    };
                })).ToList();

            return gadgets;
        }

        public async Task<Gadget?> GetGadgetByIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var gadget = (await connection.QueryFirstOrDefaultAsync<Gadget>("select * from Gadget where Id = @Id", new { Id = id }));

            if (gadget != null)
            {
                var category = await connection.QueryFirstOrDefaultAsync<Category>("select * from Categories where Id = @CategoryId;",
                    new { CategoryId = gadget.CategoryId });

                gadget.Category = category;

                return gadget;
            }

            return null;
        }

        public async Task<User?> GetGadgetOwnerByGadgetIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            var ownerId = await connection.QueryFirstOrDefaultAsync<int>("select ownerId from Gadget where Id = @GadgetId;",
                            new { GadgetId = id });
            return await connection.QueryFirstOrDefaultAsync<User>("select * from Users where Id = @Id;", new { Id = ownerId });
        }

        public async Task<GadgetsPageModel> GetGadgetsAsync(int page, int perPage)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var count = await connection.QueryFirstAsync<int>("select count(Id) from Gadget;");
            var gadgets = (await connection.QueryAsync<Gadget, Category, Gadget>(
                "select Gadget.*, Categories.* from Gadget left join Categories on CategoryId = Categories.Id " +
                "order by Gadget.Id " +
                "offset @Offset rows fetch next @PerPage rows only",
                (g, c) =>
                {
                    return new Gadget()
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Price = g.Price,
                        CategoryId = g.CategoryId,
                        Category = c
                    };
                },
                new { Offset = (page - 1) * perPage, PerPage = perPage })).ToList();

            return new GadgetsPageModel { TotalCount = count, Gadgets = gadgets };
        }

        public async Task<GadgetsPageModel> GetGadgetsByCategoryIdAsync(int id, int page, int perPage)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var count = await connection.QueryFirstAsync<int>("select count(Id) from Gadget where CategoryId = @CategoryId;", new { CategoryId = id });

            var gadgets = (await connection.QueryAsync<Gadget>(
                "select * from Gadget where CategoryId = @CategoryId " +
                "order by Id " +
                "offset @Offset rows fetch next @PerPage rows only",
                new { CategoryId = id, Offset = (page - 1) * perPage, PerPage = perPage })).ToList();


            var category = await connection.QueryFirstOrDefaultAsync<Category>("select top(1) * from Categories where Id = @CategoryId",
                new { CategoryId = id });

            gadgets.ForEach(g => g.Category = category);

            return new GadgetsPageModel { TotalCount = count, Gadgets = gadgets };
        }

        public async Task<List<Gadget>> GetGadgetsByCategoryIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var gadgets = (await connection.QueryAsync<Gadget>("select * from Gadget where CategoryId = @CategoryId",
                        new { CategoryId = id })).ToList();

            var category = (await connection.QueryFirstOrDefaultAsync<Category>("select top(1) * from Categories where Id = categoryId"));

            gadgets.ForEach(g => g.Category = category);

            return gadgets;
        }

        public async Task<GadgetsPageModel> GetGadgetsByNameAsync(string name, int page, int perPage)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var count = await connection.QueryFirstAsync<int>("select count(Id) from Gadget where [Name] like @Name;",
                new { @Name = $"%{name}%" });

            var gadgets = (await connection.QueryAsync<Gadget, Category, Gadget>(
                "select Gadget.*, Categories.* from Gadget " +
                "left join Categories on CategoryId = Categories.Id " +
                "where Gadget.[Name] like @Name " +
                "order by Gadget.Id " +
                "offset @Offset rows fetch next @PerPage rows only",
                (g, c) =>
                {
                    return new Gadget()
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Price = g.Price,
                        CategoryId = g.CategoryId,
                        Category = c
                    };
                },
                new { Name = $"%{name}%", Offset = (page - 1) * perPage, PerPage = perPage })).ToList();

            return new GadgetsPageModel{ TotalCount = count, Gadgets = gadgets };
        }

        public async Task<List<Gadget>> GetGadgetsByOwnerIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            return (await connection.QueryAsync<Gadget, Category, Gadget>(
                "select Gadget.*, Categories.* from Gadget left join Categories on CategoryId = Categories.Id where OwnerId = @OwnerId",
                (g, c) =>
                {
                    return new Gadget()
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Price = g.Price,
                        CategoryId = g.CategoryId,
                        Category = c
                    };
                },
                new { OwnerId = id })).ToList();
        }

        public async Task<bool> PassGadgetsToUserAsync(int ownerId, int inheritorId)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            if (await connection.QueryFirstOrDefaultAsync<User>("select * from Users where Id = @UserId", new { UserId = inheritorId }) == null)
            {
                throw new UserNotFoundException(inheritorId);
            }

            await connection.ExecuteAsync("update Gadget set OwnerId = @InheritorId where OwnerId = @OwnerId",
                new { OwnerId = ownerId, InheritorId = inheritorId });
            return true;
        }

        public async Task<bool> RemoveAllGadgetsByOwnerIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            int rows = await connection.ExecuteAsync("delete Gadget where OwnerId = @OwnerId", new { OwnerId = id });

            if (rows == 0)
            {
                throw new UserNotFoundException(id);
            }
            return true;
        }

        public async Task<bool> RemoveGadgetByIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("delete Gadget where Id = @Id", new { Id = id });
            if (rows == 0)
            {
                throw new GadgetNotFoundException(id);
            }

            return true;
        }

        public async Task<Gadget> UpdateGadgetAsync(Gadget gadget)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var rows = (await connection.ExecuteAsync("update Gadget set [Name] = @Name,[Price] = @Price, [CategoryId] = @CategoryId where Id = @Id",
                gadget));

            if (rows == 0)
            {
                throw new GadgetNotFoundException(gadget.Id);
            }
            return gadget;
        }
    }
}
