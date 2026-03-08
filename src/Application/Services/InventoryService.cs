namespace Application.Services;

/// <summary>Handles inventory operations such as restocking products and querying low-stock items.</summary>
public class InventoryService
{
    private readonly IProductRepository _productRepository;

    /// <summary>Products at or below this threshold are considered low stock.</summary>
    private const int LowStockThreshold = 5;

    public InventoryService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// Restocks a product by adding the specified quantity to its current stock.
    /// Throws if the product is not found or the quantity is invalid.
    /// </summary>
    public void RestockProduct(RestockProductRequest request)
    {
        Guard.Against.NegativeOrZero(request.Quantity, message: "Restock quantity must be greater than zero.");

        var product = _productRepository.GetById(request.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

        product.Stock += request.Quantity;
        _productRepository.Update(product);
    }

    /// <summary>
    /// Returns all products whose stock is at or below the low-stock threshold,
    /// ordered by stock level ascending using LINQ.
    /// </summary>
    public IEnumerable<Product> GetLowStockProducts() =>
        _productRepository.GetAll()
            .Where(p => p.Stock <= LowStockThreshold)
            .OrderBy(p => p.Stock)
            .ToList();

    /// <summary>
    /// Returns all products that are completely out of stock using LINQ.
    /// </summary>
    public IEnumerable<Product> GetOutOfStockProducts() =>
        _productRepository.GetAll()
            .Where(p => p.Stock == 0)
            .ToList();

    /// <summary>Returns the current stock level for a given product.</summary>
    public int GetStockLevel(int productId)
    {
        var product = _productRepository.GetById(productId)
            ?? throw new InvalidOperationException($"Product with ID {productId} not found.");

        return product.Stock;
    }
}
