using AutoMapper;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, ICustomerService customerService, IMapper mapper)
        {
            _orderService = orderService;
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(_mapper.Map<List<OrderResponseModel>>(orders));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && order.Customer.Id != loggedInUserId)
                return Forbid("You can only view your own orders.");

            return Ok(_mapper.Map<OrderResponseModel>(order));
        }

        [HttpGet("user-orders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            List<Order> orders;

            if (userRole == "Admin")
            {
                orders = await _orderService.GetOrdersAsync();
            }
            else
            {
                var customer = await _customerService.GetCustomerByUserIdAsync(userId);
                if (customer == null)
                    return BadRequest("No customer record found for this user.");

                orders = await _orderService.GetUserOrdersAsync(customer.Id);
            }
            return Ok(_mapper.Map<List<OrderResponseModel>>(orders));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderModel orderModel)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var customer = await _customerService.GetCustomerByUserIdAsync(userId);
            if (customer == null)
                return BadRequest("Customer not found for this user.");

            var order = await _orderService.PlaceOrderAsync(customer.Id, _mapper.Map<List<OrderItem>>(orderModel.OrderItems));
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, _mapper.Map<OrderResponseModel>(order));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
