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
        IEnumerable<CategoryAddEditRequestModel> GetCategories();
        CategoryAddEditRequestModel GetById(int id);
        void Update(int id, CategoryAddEditRequestModel model);
        CategoryAddEditRequestModel Create(CategoryAddEditRequestModel model);
        void Delete(int id);
    }
}
