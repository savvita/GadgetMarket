using GadgetMarket.Model;

namespace GadgetMarket.Repositories
{
    public interface ICategoriesRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> RemoveCategoryByIdAsync(int id);
    }
}
