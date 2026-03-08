namespace Application.Services;

/// <summary>Handles product catalog operations: add, update, delete, and querying.</summary>
public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>Adds a new product to the catalog. Throws if input is invalid.</summary>
    public void AddProduct(CreateProductRequest request)
    {
        Guard.Against.NullOrWhiteSpace(request.Name, message: "Product name cannot be empty.");
        Guard.Against.NullOrWhiteSpace(request.Description, message: "Product description cannot be empty.");
        Guard.Against.NullOrWhiteSpace(request.Category, message: "Product category cannot be empty.");
        Guard.Against.NegativeOrZero(request.Price, message: "Product price must be greater than zero.");
        Guard.Against.Negative(request.Stock, message: "Stock cannot be negative.");

        _productRepository.Add(new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            Category = request.Category
        });
    }

    /// <summary>Updates an existing product. Throws if the product is not found or input is invalid.</summary>
    public void UpdateProduct(UpdateProductRequest request)
    {
        if (!_productRepository.Exists(request.Id))
            throw new InvalidOperationException($"Product with ID {request.Id} not found.");
        Guard.Against.NullOrWhiteSpace(request.Name, message: "Product name cannot be empty.");
        Guard.Against.NullOrWhiteSpace(request.Description, message: "Product description cannot be empty.");
        Guard.Against.NullOrWhiteSpace(request.Category, message: "Product category cannot be empty.");
        Guard.Against.NegativeOrZero(request.Price, message: "Product price must be greater than zero.");
        Guard.Against.Negative(request.Stock, message: "Stock cannot be negative.");

        _productRepository.Update(new Product
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            Category = request.Category
        });
    }

    /// <summary>Deletes a product by ID. Throws if the product is not found.</summary>
    public void DeleteProduct(int id)
    {
        if (!_productRepository.Exists(id))
            throw new InvalidOperationException($"Product with ID {id} not found.");
        _productRepository.Delete(id);
    }

    /// <summary>Returns all products in the catalog.</summary>
    public IEnumerable<Product> GetAllProducts() => _productRepository.GetAll();

    /// <summary>Returns a product by ID. Throws if not found.</summary>
    public Product GetProductById(int id) =>
        _productRepository.GetById(id) ?? throw new InvalidOperationException($"Product with ID {id} not found.");

    /// <summary>Returns products whose name contains the search term (case-insensitive).</summary>
    public IEnumerable<Product> SearchByName(string name) => _productRepository.SearchByName(name);

    /// <summary>Returns products in the given category (case-insensitive).</summary>
    public IEnumerable<Product> SearchByCategory(string category) => _productRepository.SearchByCategory(category);
}
