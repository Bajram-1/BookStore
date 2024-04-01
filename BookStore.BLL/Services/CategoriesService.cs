using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using System.Linq.Expressions;

public class CategoriesService(ICategoriesRepository categoriesRepository, IUnitOfWork unitOfWork) : ICategoriesService
{
    public CategoryAddEditRequestModel Create(CategoryAddEditRequestModel model)
    {
        var category = new BookStore.DAL.Entities.Category
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            DisplayOrder = model.DisplayOrder
        };
        categoriesRepository.Create(category);
        unitOfWork.SaveChanges();
        return GetById(category.Id);
    }

    public void Delete(int id)
    {
        categoriesRepository.Delete(id);
        unitOfWork.SaveChanges();
    }

    public CategoryAddEditRequestModel GetById(int id)
    {
        var dbCategory = categoriesRepository.GetById(id) ?? throw new Exception("Category not found");
        return new CategoryAddEditRequestModel
        {
            Id = dbCategory.Id,
            Name = dbCategory.Name,
            Description = dbCategory.Description,
            DisplayOrder = dbCategory.DisplayOrder
        };
    }

    public IEnumerable<CategoryAddEditRequestModel> GetCategories()
    {
        var dbCategories = categoriesRepository.GetAll();
        var result = new List<CategoryAddEditRequestModel>();
        foreach (var x in dbCategories)
        {
            result.Add(new CategoryAddEditRequestModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder
            });
        }
        return result;
    }

    public void Update(int id, CategoryAddEditRequestModel model)
    {
        var category = categoriesRepository.GetById(id);
        category.Name = model.Name;
        category.Description = model.Description;
        category.DisplayOrder = model.DisplayOrder;
        unitOfWork.SaveChanges();
    }
}