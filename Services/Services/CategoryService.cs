using Data;
using Entity;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ReadLaterDataContext _readLaterDataContext;

        public CategoryService(ReadLaterDataContext readLaterDataContext) 
        {
            _readLaterDataContext = readLaterDataContext;            
        }

        public Category CreateCategory(Category category)
        {
            _readLaterDataContext.Add(category);
            _readLaterDataContext.SaveChanges();
            return category;
        }

        public void UpdateCategory(Category category)
        {
            _readLaterDataContext.Update(category);
            _readLaterDataContext.SaveChanges();
        }

        public List<Category> GetCategories()
        {
            return _readLaterDataContext.Categories.ToList();
        }

        public Category? GetCategory(int id)
        {
            return _readLaterDataContext.Categories.FirstOrDefault(c => c.Id == id);
        }

        public Category? GetUserCategory(int id, string userId)
        {
            return _readLaterDataContext.Categories.FirstOrDefault(x => x.Id == id && x.UserId == userId);
        }

        public void DeleteCategory(Category category)
        {
            _readLaterDataContext.Categories.Remove(category);
            _readLaterDataContext.SaveChanges();
        }

        public List<Category> GetUserCategories(string userId)
        {
            return _readLaterDataContext.Categories.Where(x => x.UserId == userId).ToList();
        }
    }
}
