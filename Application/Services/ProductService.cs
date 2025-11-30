using Infrastructure.Data;
using Microsoft.Extensions.Logging;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching product with Id {ProductId}", id);
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            _logger.LogWarning("Product with Id {ProductId} not found", id);
        return product;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogInformation("Fetching all products");
        return await _productRepository.GetAllAsync(page, pageSize);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedOn = DateTime.UtcNow;
        _logger.LogInformation("Creating product {ProductName}", product.ProductName);
        await _productRepository.AddAsync(product);
        _logger.LogInformation("Product {ProductId} created successfully", product.Id);
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        product.ModifiedOn = DateTime.UtcNow;
        _logger.LogInformation("Updating product {ProductId}", product.Id);
        try
        {
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Product {ProductId} updated successfully", product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", product.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            throw new KeyNotFoundException($"Product with id {id} not found");
        }

        await _productRepository.DeleteAsync(existing);
        _logger.LogInformation("Product {ProductId} deleted successfully", id);
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(int productId)
    {
        _logger.LogInformation("Fetching items for product {ProductId}", productId);
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found, returning empty item list", productId);
            return Enumerable.Empty<Item>();
        }
        return product.Items;
    }
}
