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
        public async Task<DTO.OrderHeader> GetOrderHeaderAsync(Expression<Func<DAL.Entities.OrderHeader, bool>> predicate)
        {
            var orderHeaderEntity = await orderHeadersRepository.GetAsync(predicate, includeProperties: "ApplicationUser");

            if (orderHeaderEntity == null)
            {
                throw new InvalidOperationException("Order header not found in the database.");
            }

            var orderHeaderDto = new DTO.OrderHeader
            {
                Id = orderHeaderEntity.Id,
                Name = orderHeaderEntity.Name,
                PhoneNumber = orderHeaderEntity.PhoneNumber,
                StreetAddress = orderHeaderEntity.StreetAddress,
                City = orderHeaderEntity.City,
                State = orderHeaderEntity.State,
                PostalCode = orderHeaderEntity.PostalCode,
                OrderDate = orderHeaderEntity.OrderDate,
                OrderStatus = orderHeaderEntity.OrderStatus,
                OrderTotal = orderHeaderEntity.OrderTotal,
                PaymentDate = orderHeaderEntity.PaymentDate,
                PaymentDueDate = orderHeaderEntity.PaymentDueDate,
                PaymentStatus = orderHeaderEntity.PaymentStatus,
                ShippingDate = orderHeaderEntity.ShippingDate,
                SessionId = orderHeaderEntity.SessionId,
                PaymentIntentId = orderHeaderEntity.PaymentIntentId,
                TrackingNumber = orderHeaderEntity.TrackingNumber,
                ApplicationUserId = orderHeaderEntity.ApplicationUserId,
                Carrier = orderHeaderEntity.Carrier,
            };

            return orderHeaderDto;
        }

        public async Task<DTO.OrderHeader> GetAsync(Expression<Func<DAL.Entities.OrderHeader, bool>> predicate, string includeProperties = "ApplicationUser")
        {
            var entity = await orderHeadersRepository.GetAsync(predicate, includeProperties, tracked: false);

            if (entity == null) return null;

            return new DTO.OrderHeader
            {
                Id = entity.Id,
                ApplicationUserId = entity.ApplicationUserId,
                Name = entity.Name,
                City = entity.City,
                StreetAddress = entity.StreetAddress,
                State = entity.State,
                Carrier = entity.Carrier,
                TrackingNumber = entity.TrackingNumber,
                OrderDate = entity.OrderDate,
                OrderStatus = entity.OrderStatus,
                OrderTotal = entity.OrderTotal,
                PaymentDate = entity.PaymentDate,
                PaymentDueDate = entity.PaymentDueDate,
                PaymentStatus = entity.PaymentStatus,
                PostalCode = entity.PostalCode,
                PhoneNumber = entity.PhoneNumber,
                ShippingDate = entity.ShippingDate,
                SessionId = entity.SessionId,
                PaymentIntentId = entity.PaymentIntentId,
                ApplicationUser = new DTO.ApplicationUser
                {
                    Id = entity.ApplicationUser.Id,
                    Email = entity.ApplicationUser.Email,
                    PhoneNumber = entity.ApplicationUser.PhoneNumber
                }
            };
        }

        public async Task<IEnumerable<DTO.OrderHeader>> GetAllAsync(Expression<Func<DAL.Entities.OrderHeader, bool>>? filter = null, string includeProperties = null)
        {
            var orderHeaders = await orderHeadersRepository.GetAllAsync(filter, includeProperties);
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
                ApplicationUser = new DTO.ApplicationUser
                {
                    Id = o.ApplicationUser.Id,
                    Email = o.ApplicationUser.Email,
                    PhoneNumber = o.ApplicationUser.PhoneNumber
                }
            });
        }

        public async Task<BLL.DTO.OrderHeader> AddAsync(BLL.DTO.OrderHeader model)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                try
                {
                    var orderHeader = new DAL.Entities.OrderHeader
                    {
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

                    await orderHeadersRepository.CreateAsync(orderHeader);
                    await unitOfWork.SaveChangesAsync();
                    transaction.Commit();

                    var dtoOrderHeader = new BLL.DTO.OrderHeader
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

                    return dtoOrderHeader;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateAsync(BLL.DTO.OrderHeader model)
        {
            var orderHeader = await orderHeadersRepository.GetAsync(s => s.Id == model.Id);

            if (orderHeadersRepository == null)
            {
                throw new Exception("Order Header not found");
            }

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

            await orderHeadersRepository.UpdateAsync(orderHeader);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, string orderStatus, string? paymentStatus = null)
        {
            await orderHeadersRepository.UpdateStatusAsync(id, orderStatus, paymentStatus);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = await unitOfWork.OrderHeadersRepository.GetItemAsync(id, "ApplicationUser");

            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderFromDb.PaymentIntentId = paymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }

                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Order not found");
            }
        }
    }
}