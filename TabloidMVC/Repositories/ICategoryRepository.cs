using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        public void AddCategory(Category category);
        Category GetCategoryById(int id);
        public void DeleteCategory(int id);
        public void UpdateCategory(Category category);
    }
}