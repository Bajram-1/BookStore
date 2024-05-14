using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Azure.Core.HttpHeader;

public class CategoriesService(ICategoriesRepository categoriesRepository, 
                               IUnitOfWork unitOfWork,
                               ICacheService cacheService) : ICategoriesService
{
    public static event ObjectAddedEvent OnCategoryAdded;
    public static event OnEntityDeleted OnEntityDeleted;

    public async Task<CategoryAddEditRequestModel> Create(CategoryAddEditRequestModel model)
    {
        var existingCategory = await categoriesRepository.GetByNameAsync(model.Name);
        if (existingCategory != null)
        {
            throw new Exception("This category already exists.");
        }

        if (model.DisplayOrder == null)
        {
            model.DisplayOrder = 1;
        }

        int? actualDisplayOrder = await categoriesRepository.GetActualDisplayOrderAsync();
        int nextDisplayOrder = actualDisplayOrder ?? 1;
        nextDisplayOrder++;

        model.DisplayOrder = nextDisplayOrder;

        var category = new BookStore.DAL.Entities.Category
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            DisplayOrder = model.DisplayOrder
        };

        await categoriesRepository.CreateAsync(category);
        await unitOfWork.SaveChangesAsync();
        await cacheService.RemoveAsync("categories");
        OnCategoryAdded?.Invoke(category.Id);
        return await GetByIdAsync(category.Id);
    }

    public async Task Delete(int id)
    {
        await categoriesRepository.DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
        OnEntityDeleted?.Invoke(new BookStore.DAL.Entities.AuditLog
        {
            EntityId = id.ToString(),
            EntityName = "Category",
            Details = "Category was deleted",
            LogType = BookStore.Common.Enums.AuditLogType.Delete
        });
        await cacheService.RemoveAsync("categories");
    }

    public async Task<CategoryAddEditRequestModel> GetByIdAsync(int id)
    {
        var dbCategory = await categoriesRepository.GetByIdAsync(id);
        if (dbCategory == null)
        {
            throw new Exception("Category not found");
        }

        return new CategoryAddEditRequestModel
        {
            Id = dbCategory.Id,
            Name = dbCategory.Name,
            Description = dbCategory.Description,
            DisplayOrder = dbCategory.DisplayOrder
        };
    }

    public async Task<List<BookStore.BLL.DTO.Category>> GetCategories()
    {
        string cacheKey = "categories";

        return await cacheService.GetOrAddAsync(cacheKey, async () =>
        {
            var dbCategories = await categoriesRepository.GetAllAsync();
            var categoriesList = dbCategories.Select(category => new BookStore.BLL.DTO.Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder
            }).ToList();

            return categoriesList;
        });
    }

    public async Task Update(int id, CategoryAddEditRequestModel model)
    {
        var existingCategory = await categoriesRepository.GetByNameAsync(model.Name);
        
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new Exception("This category already exists.");
        }

        var category = await categoriesRepository.GetByIdAsync(id);
        
        if (category == null)
        {
            throw new Exception("Category not found");
        }

        category.Name = model.Name;
        category.Description = model.Description;
        category.DisplayOrder = model.DisplayOrder;

        await unitOfWork.SaveChangesAsync();
        await cacheService.RemoveAsync("categories");
    }
}