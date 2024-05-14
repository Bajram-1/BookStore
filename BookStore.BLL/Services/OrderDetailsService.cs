using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class OrderDetailsService(IOrderDetailsRepository orderDetailsRepository, IUnitOfWork unitOfWork) : IOrderDetailsService
    {
        public async Task<IEnumerable<DTO.OrderDetail>> GetAllByOrderHeaderIdAsync(Expression<Func<DAL.Entities.OrderDetail, bool>> predicate, string includeProperties = "Product")
        {
            return await Task.Run(async () =>
            {
                IQueryable<DAL.Entities.OrderDetail> query = (await orderDetailsRepository.GetAllAsync(predicate, includeProperties: "Product")).AsQueryable();

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }

                var filteredQuery = query.Where(predicate);

                var result = filteredQuery.Select(o => new DTO.OrderDetail
                {
                    Id = o.Id,
                    ProductId = o.ProductId,
                    OrderHeaderId = o.OrderHeaderId,
                    Price = o.Price,
                    Count = o.Count,
                    Product = o.Product != null ? new DTO.Product
                    {
                        Id = o.Product.Id,
                        Title = o.Product.Title,
                        Description = o.Product.Description,
                        Author = o.Product.Author,
                        ISBN = o.Product.ISBN,
                        ListPrice = o.Product.ListPrice,
                        Price = o.Product.Price,
                        Price50 = o.Product.Price50,
                        Price100 = o.Product.Price100
                    } : new DTO.Product()
                }).ToList();

                return (IEnumerable<DTO.OrderDetail>)result;
            });
        }

        public async Task AddOrderDetailAsync(BLL.DTO.OrderDetail orderDetail, int orderHeaderId)
        {
            if (orderDetail == null)
            {
                return;
            }

            var orderDetails = new DAL.Entities.OrderDetail
            {
                Id = orderDetail.Id,
                Price = orderDetail.Price,
                Count = orderDetail.Count,
                ProductId = orderDetail.ProductId,
                OrderHeaderId = orderHeaderId,
            };

            await orderDetailsRepository.CreateAsync(orderDetails);
            await unitOfWork.SaveChangesAsync();
        }
    }
}