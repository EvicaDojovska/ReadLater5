using Entity;
using System.Collections.Generic;

namespace Services
{
    public interface ICategoryService
    {
        Category CreateCategory(Category category);
        List<Category> GetCategories();
        Category? GetCategory(int id);
        Category? GetUserCategory(int id, string userId);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        List<Category> GetUserCategories(string userId);
    }
}
