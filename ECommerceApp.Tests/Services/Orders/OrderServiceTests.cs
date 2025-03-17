using AutoMapper;
using ECommerceApp.Application.Implementation;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerceApp.Tests.Services.Orders
{
    public class OrderServiceTests
    {
        private readonly Mock<IGenericRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IGenericRepository<Product>> _productRepositoryMock;
        private readonly Mock<IGenericRepository<Customer>> _customerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IGenericRepository<Order>>();
            _productRepositoryMock = new Mock<IGenericRepository<Product>>();
            _customerRepositoryMock = new Mock<IGenericRepository<Customer>>();
            _mapperMock = new Mock<IMapper>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _productRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _mapperMock.Object
            );
        }

  
        [Fact]
        public async Task PlaceOrderAsync_ShouldCreateOrder_WhenValidCustomerAndProducts()
        {
            // Arrange
            int customerId = 1;
            var customer = new Customer { Id = customerId, Name = "Mic Fleetwood" };
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "product 1", Price = 1000 },
                new Product { Id = 2, Name = "product 2", Price = 50 }
            };
            var orderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 1, Price = 1000 },
                new OrderItem { ProductId = 2, Quantity = 2, Price = 100 }
            };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customer);
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => products.FirstOrDefault(p => p.Id == id));
            _orderRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order order) => order);

            // Act
            var result = await _orderService.PlaceOrderAsync(customerId, orderItems);

            // Assert
            result.Should().NotBeNull();
            result.Customer.Should().BeEquivalentTo(customer);
            result.OrderItems.Should().HaveCount(2);
            result.TotalAmount.Should().Be(1100);
        }

        [Fact]
        public async Task PlaceOrderAsync_ShouldThrowException_WhenCustomerNotFound()
        {
            // Arrange
            int customerId = 99;
            var orderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 1, Price = 1000 }
            };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            Func<Task> act = async () => await _orderService.PlaceOrderAsync(customerId, orderItems);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Customer with ID 99 not found.");
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, TotalAmount = 200 },
                new Order { Id = 2, TotalAmount = 500 }
            };

            _orderRepositoryMock.Setup(repo => repo.ListAllAsync())
                .ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetOrdersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}
