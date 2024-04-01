using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.BLL.Services;
using BookStore.Common;
using BookStore.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController(IOrderHeadersService orderHeadersService, IOrderDetailsService orderDetailsService) : Controller
    {
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public IActionResult Index()
        {
            var orderHeaders = orderHeadersService.GetAllOrderDetails();
            return View(orderHeaders);
        }

        public IActionResult Details(int orderId)
        {
            var orderVM = new OrderVM
            {
                OrderHeader = orderHeadersService.Get(o => o.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = (IEnumerable<BLL.DTO.OrderDetail>)orderDetailsService.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetail(OrderHeaderAddEditRequestModel model)
        {
            try
            {
                orderHeadersService.Update(model);
                TempData["success"] = "Order Details Updated Successfully.";
                return RedirectToAction(nameof(Details), new { orderId = model.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating order details: {ex.Message}");
                return View(nameof(Details), model);
            }
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult StartProcessing(int orderId)
        {
            orderHeadersService.UpdateStatus(orderId, StaticDetails.StatusInProcess);
            TempData["success"] = "Order Processing Started Successfully.";
            return RedirectToAction(nameof(Details), new { orderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder(int orderId, string carrier, string trackingNumber)
        {
            orderHeadersService.UpdateStatus(orderId, StaticDetails.StatusShipped);
            orderHeadersService.UpdateShippingInfo(orderId, carrier, trackingNumber);
            TempData["success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult CancelOrder(int orderId)
        {
            orderHeadersService.UpdateStatus(orderId, StaticDetails.StatusCancelled);
            TempData["success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var orderHeaders = orderHeadersService.GetAllOrderDetails();

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "cancelled":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusCancelled);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        #endregion
    }
}
