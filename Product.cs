namespace ProductApi;

public record Product(int Id, string Name, string Description, decimal Price);

public record ProductRequest(string Name, string Description, decimal Price);
