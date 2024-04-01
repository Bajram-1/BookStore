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
        public DTO.OrderDetail GetAll(Func<DTO.OrderDetail, bool> predicate, string includeProperties = "")
        {
            IQueryable<DAL.Entities.OrderDetail> query = (IQueryable<DAL.Entities.OrderDetail>)orderDetailsRepository.GetAll();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.Select(o => new DTO.OrderDetail
            {
                Id = o.Id,
                OrderHeaderId = o.OrderHeaderId,
                ProductId = o.ProductId,
                Price = o.Price,
                Count = o.Count
            }).FirstOrDefault(predicate);
        }

        public DTO.OrderDetail GetOrderDetailById(int id)
        {
            var orderDetail = orderDetailsRepository.Get(o => o.Id == id);

            if (orderDetail == null)
            {
                throw new Exception("Order details not found");
            }

            return new DTO.OrderDetail
            {
                Id = orderDetail.Id,
                ProductId = orderDetail.ProductId,
                OrderHeaderId = orderDetail.OrderHeaderId,
                Price = orderDetail.Price,
                Count = orderDetail.Count
            };
        }

        public void AddOrderDetail(DAL.Entities.OrderDetail orderDetail)
        {
            var orderDetails = new DAL.Entities.OrderDetail
            {
                Id = orderDetail.Id,
                Price = orderDetail.Price,
                Count = orderDetail.Count,
                OrderHeaderId = orderDetail.OrderHeaderId,
                ProductId = orderDetail.ProductId
            };

            orderDetailsRepository.Add(orderDetails);
            unitOfWork.SaveChanges();
        }

        public void UpdateOrderDetail(OrderDetailsAddEditRequestModel orderDetail)
        {
            var orderDetails = orderDetailsRepository.Get(s => s.Id == orderDetail.Id);

            if (orderDetailsRepository == null)
            {
                throw new Exception("Order Details not found");
            }

            orderDetails.Id = orderDetail.Id;
            orderDetails.Price = orderDetail.Price;
            orderDetails.Count = orderDetail.Count;
            orderDetails.OrderHeaderId = orderDetail.OrderHeaderId;
            orderDetails.ProductId = orderDetail.ProductId;

            orderDetailsRepository.Update(orderDetails);
            unitOfWork.SaveChanges();
        }

        public void DeleteOrderDetail(int id)
        {
            var orderDetail = orderDetailsRepository.Get(o => o.Id == id);

            if (orderDetail == null)
            {
                throw new Exception("Order Details not found.");
            }

            orderDetailsRepository.Remove(orderDetail);
            unitOfWork.SaveChanges();
        }

        public void RemoveRange()
        {
            var orderDetail = orderDetailsRepository.GetAll().ToList();

            if (orderDetail == null)
            {
                throw new Exception("Order Details not found");
            }

            orderDetailsRepository.RemoveRange(orderDetail);
            unitOfWork.SaveChanges();
        }
    }
}