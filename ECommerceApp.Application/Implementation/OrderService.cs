using AutoMapper;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ECommerceApp.Application.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;


        public OrderService(IGenericRepository<Order> orderRepository, 
            IGenericRepository<Product> productRepository, 
            IGenericRepository<Customer> customerRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Order>> GetOrdersAsync() =>
            await _orderRepository.ListAllAsync();

        public async Task<Order> GetOrderByIdAsync(int id) 
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            return order;
        }

        public async Task<Order> PlaceOrderAsync(int customerId, List<OrderItem> orderItems)
        {
            try
            {
                _logger.LogInformation("Placing order for customer {CustomerId} with {ItemCount} items.", customerId, orderItems.Count);

                if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("Order must contain at least one item.");

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                    throw new ArgumentException($"Customer with ID {customerId} not found.");

                    decimal totalAmount = 0;
                List<OrderItem> items = new List<OrderItem>();

                foreach (var item in orderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                        throw new ArgumentException($"Product with ID {item.ProductId} not found.");

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };

                    items.Add(orderItem);
                    totalAmount += product.Price * item.Quantity;
                }

                var order = new Order
                {
                    Customer = customer,
                    OrderItems = items,
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.Now
                };

                _logger.LogInformation("Order successfully placed for Customer {CustomerId}", customerId);
                return await _orderRepository.AddAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while placing order for Customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            await _orderRepository.DeleteAsync(order);
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.ListAllAsync();
            return orders.Where(o => o.Customer.Id == userId).ToList();
        }
    }

}
