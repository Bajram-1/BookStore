using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface ICategoriesService
    {
        Task<CategoryAddEditRequestModel> Create(CategoryAddEditRequestModel model);
        Task Delete(int id);
        Task<CategoryAddEditRequestModel> GetByIdAsync(int id);
        Task<List<Category>> GetCategories();
        Task Update(int id, CategoryAddEditRequestModel model);
    }
}
