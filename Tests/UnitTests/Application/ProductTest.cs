using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Moq;
using Xunit;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenProductExists()
    {
        var product = this.setProductData();
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Sample Product A", result.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

        var result = await _productService.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            this.setProductData(),
            this.setProductData()
        };
        _productRepoMock.Setup(r => r.GetAllAsync(It.IsAny<int>(),It.IsAny<int>())).ReturnsAsync(products);

        var result = await _productService.GetAllAsync(1,10);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedOnAndCallsAddAsync()
    {
        var product = this.setProductData();
        _productRepoMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);

        var result = await _productService.CreateAsync(product);

        Assert.Equal(product, result);
        Assert.True(result.CreatedOn <= DateTime.UtcNow);
        _productRepoMock.Verify(r => r.AddAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedOnAndCallsUpdateAsync()
    {
        var product = this.setProductData();
        _productRepoMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

        await _productService.UpdateAsync(product);

        Assert.True(product.ModifiedOn <= DateTime.UtcNow);
        _productRepoMock.Verify(r => r.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesExistingProduct()
    {
        var product = this.setProductData();
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _productRepoMock.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

        await _productService.DeleteAsync(1);

        _productRepoMock.Verify(r => r.DeleteAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.DeleteAsync(1));
    }

    [Fact]
    public async Task GetItemsAsync_ReturnsItems_WhenProductExists()
    {
        var items = new List<Item> { new Item { Id = 1 }, new Item { Id = 2 } };
        var product = new Product { Id = 1, Items = items };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _productService.GetItemsAsync(1);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetItemsAsync_ReturnsEmpty_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

        var result = await _productService.GetItemsAsync(1);

        Assert.Empty(result);
    }

    private Product setProductData()
    {
        var product = new Product
        {
            Id = 1,
            ProductName = "Sample Product A",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow,
            ModifiedBy = null,
            ModifiedOn = null,
            Items = new List<Item>
    {
        new Item
{
    Id = 1,
    ProductId = 1,  // Link to Product with Id = 1
    Quantity = 10,
    Product = new Product
    {
        Id = 1,
        ProductName = "Sample Product A",
        CreatedBy = "admin",
        CreatedOn = DateTime.UtcNow
    }
}
    }

        };
        return product;
    }
}
