using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using GadgetMarket.Model;
using Microsoft.AspNetCore.Authorization;
using GadgetMarket.Repositories;
using GadgetMarket.Exceptions;

namespace GadgetMarket.Web.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private ICategoriesRepository categoriesRepository;
        public CategoryController(ICategoriesRepository categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        //TODO check this
        [HttpGet("")]
        public async Task<List<Category>> GetAllCategories()
        {
            return await categoriesRepository.GetAllCategoriesAsync();
        }


        //TODO check this
        [HttpGet("{id:int}")]
        public async Task<Category> GetCategoryById(int categoryId)
        {
            var category = await categoriesRepository.GetCategoryByIdAsync(categoryId);

            if(category == null)
            {
                throw new CategoryNotFoundException(categoryId);
            }

            return category;
        }

        //TODO check this
        [HttpPost("new")]
        [Authorize]
        public async Task<Category> AddCategory(string name)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                //TODO add check for admin role

                Category category = new Category()
                {
                    Name = name
                };

                return await categoriesRepository.CreateCategoryAsync(category);

                
            }

            throw new BadRequestException();
        }

        //TODO check this
        [HttpPut]
        [Authorize]
        public async Task<Category> UpdateCategory(Category category)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                //TODO add check for admin role

                return await categoriesRepository.UpdateCategoryAsync(category);
            }

            throw new BadRequestException();
        }

        //TODO check this
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<bool> RemoveCategoryById(int categoryId)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }
            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                //TODO add check for admin role

                return await categoriesRepository.RemoveCategoryByIdAsync(categoryId);
            }

            throw new BadRequestException();
        }
    }
}
