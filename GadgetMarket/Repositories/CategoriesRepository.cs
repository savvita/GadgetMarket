using Dapper;
using GadgetMarket.Exceptions;
using GadgetMarket.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadgetMarket.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private DbConfig configuration;

        public CategoriesRepository(DbConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            var id = await connection.QueryFirstAsync<int>("insert into Categories values (@Name); select SCOPE_IDENTITY()", category);
            category.Id = id;

            return category;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return (await connection.QueryAsync<Category>("select * from Categories")).ToList();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return (await connection.QueryFirstOrDefaultAsync<Category>("select * from Categories where Id = @Id",
                new { Id = id }));
        }

        public async Task<bool> RemoveCategoryByIdAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            int rows = await connection.ExecuteAsync("delete Categories where Id = @Id", new { Id = id });

            if (rows == 0)
            {
                throw new CategoryNotFoundException(id);
            }

            return true;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            int rows = await connection.ExecuteAsync("update Categories set [Name] = @Name where Id = @Id", category);

            if(rows == 0)
            {
                throw new CategoryNotFoundException(category.Id);
            }

            return category;
        }
    }
}
