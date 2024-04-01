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
using System.Threading;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class OrderHeadersService(IOrderHeadersRepository orderHeadersRepository, IUnitOfWork unitOfWork) : IOrderHeadersService
    {
        public DTO.OrderHeader Get(Func<DTO.OrderHeader, bool> predicate, string includeProperties = "")
        {
            IQueryable<DAL.Entities.OrderHeader> query = (IQueryable<DAL.Entities.OrderHeader>)orderHeadersRepository.GetAll();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.Select(o => new DTO.OrderHeader
            {
                Id = o.Id,
                ApplicationUserId = o.ApplicationUserId,
                Name = o.Name,
                City = o.City,
                StreetAddress = o.StreetAddress,
                State = o.State,
                Carrier = o.Carrier,
                TrackingNumber = o.TrackingNumber,
                OrderDate = o.OrderDate,
                OrderStatus = o.OrderStatus,
                OrderTotal = o.OrderTotal,
                PaymentDate = o.PaymentDate,
                PaymentDueDate = o.PaymentDueDate,
                PaymentStatus = o.PaymentStatus,
                PostalCode = o.PostalCode,
                PhoneNumber = o.PhoneNumber,
                ShippingDate = o.ShippingDate,
                SessionId = o.SessionId,
                PaymentIntentId = o.PaymentIntentId
            }).FirstOrDefault(predicate);
        }

        public IEnumerable<DTO.OrderHeader> GetAllOrderDetails()
        {
            var orderHeaders = orderHeadersRepository.GetAll();
            return orderHeaders.Select(o => new DTO.OrderHeader
            {
                Id = o.Id,
                ApplicationUserId = o.ApplicationUserId,
                Name = o.Name,
                City = o.City,
                StreetAddress = o.StreetAddress,
                State = o.State,
                Carrier = o.Carrier,
                TrackingNumber = o.TrackingNumber,
                OrderDate = o.OrderDate,
                OrderStatus = o.OrderStatus,
                OrderTotal = o.OrderTotal,
                PaymentDate = o.PaymentDate,
                PaymentDueDate = o.PaymentDueDate,
                PaymentStatus = o.PaymentStatus,
                PostalCode = o.PostalCode,
                PhoneNumber = o.PhoneNumber,
                ShippingDate = o.ShippingDate,
                SessionId = o.SessionId,
                PaymentIntentId = o.PaymentIntentId,
            });
        }

        public DTO.OrderHeader GetOrderDetailById(int id)
        {
            var orderHeader = orderHeadersRepository.Get(o => o.Id == id);
            if (orderHeader == null)
            {
                throw new Exception("Order header not found");
            }

            return new DTO.OrderHeader
            {
                Id = orderHeader.Id,
                ApplicationUserId = orderHeader.ApplicationUserId,
                Name = orderHeader.Name,
                City = orderHeader.City,
                StreetAddress = orderHeader.StreetAddress,
                State = orderHeader.State,
                Carrier = orderHeader.Carrier,
                TrackingNumber = orderHeader.TrackingNumber,
                OrderDate = orderHeader.OrderDate,
                OrderStatus = orderHeader.OrderStatus,
                OrderTotal = orderHeader.OrderTotal,
                PaymentDate = orderHeader.PaymentDate,
                PaymentDueDate = orderHeader.PaymentDueDate,
                PaymentStatus = orderHeader.PaymentStatus,
                PostalCode = orderHeader.PostalCode,
                PhoneNumber = orderHeader.PhoneNumber,
                ShippingDate = orderHeader.ShippingDate,
                SessionId = orderHeader.SessionId,
                PaymentIntentId = orderHeader.PaymentIntentId
            };
        }

        public void Add(OrderHeaderAddEditRequestModel model)
        {
            var orderHeader = new DAL.Entities.OrderHeader
            {
                Id = model.Id,
                ApplicationUserId = model.ApplicationUserId,
                Name = model.Name,
                City = model.City,
                StreetAddress = model.StreetAddress,
                State = model.State,
                Carrier = model.Carrier,
                TrackingNumber = model.TrackingNumber,
                OrderDate = model.OrderDate,
                OrderStatus = model.OrderStatus,
                OrderTotal = model.OrderTotal,
                PaymentDate = model.PaymentDate,
                PaymentDueDate = model.PaymentDueDate,
                PaymentStatus = model.PaymentStatus,
                PostalCode = model.PostalCode,
                PhoneNumber = model.PhoneNumber,
                ShippingDate = model.ShippingDate,
                SessionId = model.SessionId,
                PaymentIntentId = model.PaymentIntentId
            };

            orderHeadersRepository.Add(orderHeader);
            unitOfWork.SaveChanges();
        }

        public void Update(OrderHeaderAddEditRequestModel model)
        {
            var orderHeader = orderHeadersRepository.Get(s => s.Id == model.Id);

            if (orderHeadersRepository == null)
            {
                throw new Exception("Order Header not found");
            }

            orderHeader.Id = model.Id;
            orderHeader.ApplicationUserId = model.ApplicationUserId;
            orderHeader.Name = model.Name;
            orderHeader.City = model.City;
            orderHeader.StreetAddress = model.StreetAddress;
            orderHeader.State = model.State;
            orderHeader.Carrier = model.Carrier;
            orderHeader.TrackingNumber = model.TrackingNumber;
            orderHeader.OrderDate = model.OrderDate;
            orderHeader.OrderStatus = model.OrderStatus;
            orderHeader.OrderTotal = model.OrderTotal;
            orderHeader.PaymentDate = model.PaymentDate;
            orderHeader.PaymentDueDate = model.PaymentDueDate;
            orderHeader.PaymentStatus = model.PaymentStatus;
            orderHeader.PostalCode = model.PostalCode;
            orderHeader.PhoneNumber = model.PhoneNumber;
            orderHeader.ShippingDate = model.ShippingDate;
            orderHeader.SessionId = model.SessionId;
            orderHeader.PaymentIntentId = model.PaymentIntentId;

            orderHeadersRepository.Update(orderHeader);
            unitOfWork.SaveChanges();
        }

        public void Delete(int id)
        {
            var orderHeader = orderHeadersRepository.Get(o => o.Id == id);
            if (orderHeader == null)
            {
                throw new Exception("Order header not found");
            }

            orderHeadersRepository.Remove(orderHeader);
            unitOfWork.SaveChanges();
        }

        public void RemoveRange(IEnumerable<DTO.OrderHeader> orderHeaders)
        {
            var orderHeader = orderHeadersRepository.GetAll().ToList();

            if (orderHeader == null)
            {
                throw new Exception("Order Header not found");
            }

            orderHeadersRepository.RemoveRange(orderHeader);
            unitOfWork.SaveChanges();
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            orderHeadersRepository.UpdateStatus(id, orderStatus, paymentStatus);
            unitOfWork.SaveChanges();
        }

        public void UpdateShippingInfo(int orderId, string carrier, string trackingNumber)
        {
            var orderHeader = orderHeadersRepository.Get(o => o.Id == orderId);
            if (orderHeader == null)
            {
                throw new Exception("Order header not found");
            }

            orderHeader.Carrier = carrier;
            orderHeader.TrackingNumber = trackingNumber;

            orderHeadersRepository.Update(orderHeader);
            unitOfWork.SaveChanges();
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            orderHeadersRepository.UpdateStripePaymentID(id, sessionId, paymentIntentId);
            unitOfWork.SaveChanges();
        }
    }
}