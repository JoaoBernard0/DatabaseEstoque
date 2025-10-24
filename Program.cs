using Microsoft.EntityFrameworkCore;
using EstoqueApi.Data;
using EstoqueApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Environment.EnvironmentName = "Development";

builder.WebHost.UseUrls("http://localhost:5099");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=estoque.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

var webTask = app.RunAsync();
Console.WriteLine("API online em http://localhost:5099 (Swagger em /swagger)");
Console.WriteLine("=============================================");
Console.WriteLine("            Gerenciador de Estoque           ");
Console.WriteLine("=============================================");
Console.WriteLine("Console e API executando juntos!");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1 - Cadastrar novo produto");
    Console.WriteLine("2 - Listar todos os produtos");
    Console.WriteLine("3 - Atualizar produto (por Id)");
    Console.WriteLine("4 - Remover produto (por Id)");
    Console.WriteLine("0 - Sair");
    Console.Write("\nOpção Selecionada: ");

    var option = Console.ReadLine();

    if (option == "0") break;

    switch (option)
    {
        case "1": await CreateProductAsync(); break;
        case "2": await ListProductsAsync(); break;
        case "3": await UpdateProductAsync(); break;
        case "4": await DeleteProductAsync(); break;
        default: Console.WriteLine("Opção inválida."); break;
    }
}

await app.StopAsync();
await webTask;

async Task CreateProductAsync()
{
    Console.Write("\nNome do produto: ");
    var name = (Console.ReadLine() ?? "").Trim();

    Console.Write("Categoria (opcional): ");
    var category = (Console.ReadLine() ?? "").Trim();

    Console.Write("Preço: ");
    if (!decimal.TryParse(Console.ReadLine(), out var price))
    {
        Console.WriteLine("Preço inválido.");
        return;
    }

    Console.Write("SKU (opcional): ");
    var sku = (Console.ReadLine() ?? "").Trim();
    if (sku == "") sku = null;

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Nome é obrigatório.");
        return;
    }

    using var db = new AppDbContext();
    if (await db.Products.AnyAsync(p => p.Name.ToUpper() == name.ToUpper()))
    {
        Console.WriteLine("Já existe um produto com esse nome.");
        return;
    }
    if (!string.IsNullOrWhiteSpace(sku) && await db.Products.AnyAsync(p => p.SKU == sku))
    {
        Console.WriteLine("SKU já existe.");
        return;
    }

    var product = new Product { Name = name, Category = category, Price = price, SKU = sku, CreatedAt = DateTime.UtcNow };
    db.Products.Add(product);
    await db.SaveChangesAsync();
    Console.WriteLine($"Produto '{product.Name}' cadastrado com sucesso! Id: {product.Id}");
}

async Task ListProductsAsync()
{
    using var db = new AppDbContext();
    var items = await db.Products.OrderBy(p => p.Id).ToListAsync();
    if (items.Count == 0) { Console.WriteLine("Nenhum produto encontrado."); return; }

    Console.WriteLine("\n-------------------------------------------------------------------");
    Console.WriteLine("Id | Nome                     | Categoria         | Preço     | SKU");
    Console.WriteLine("-------------------------------------------------------------------");
    foreach (var p in items)
    {
        Console.WriteLine($"{p.Id,2} | {p.Name,-24} | {p.Category,-16} | {p.Price,10:C2} | {p.SKU}");
    }
    Console.WriteLine("-------------------------------------------------------------------");
}

async Task UpdateProductAsync()
{
    Console.Write("\nInforme o Id do produto a atualizar: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var product = await db.Products.FindAsync(id);
    if (product is null) { Console.WriteLine("Produto não encontrado."); return; }

    Console.WriteLine($"Atualizando Id {product.Id}. Deixe em branco para manter o valor atual.");

    Console.Write($"Nome atual [{product.Name}]: ");
    var newName = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;

    Console.Write($"Categoria atual [{product.Category}]: ");
    var newCategory = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newCategory)) product.Category = newCategory;

    Console.Write($"Preço atual [{product.Price:C2}]: ");
    var newPriceStr = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newPriceStr) && decimal.TryParse(newPriceStr, out var newPrice))
        product.Price = newPrice;

    Console.Write($"SKU atual [{product.SKU}]: ");
    var newSku = (Console.ReadLine() ?? "").Trim();
    if (!string.IsNullOrWhiteSpace(newSku)) product.SKU = newSku;

    if (await db.Products.AnyAsync(p => p.Name.ToUpper() == product.Name.ToUpper() && p.Id != id))
    {
        Console.WriteLine("Já existe outro produto com o novo nome informado.");
        return;
    }
    if (!string.IsNullOrWhiteSpace(product.SKU) && await db.Products.AnyAsync(p => p.SKU == product.SKU && p.Id != id))
    {
        Console.WriteLine("SKU já está em uso por outro produto.");
        return;
    }

    await db.SaveChangesAsync();
    Console.WriteLine("Produto atualizado com sucesso.");
}

async Task DeleteProductAsync()
{
    Console.Write("\nInforme o Id do produto a remover: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var product = await db.Products.FindAsync(id);
    if (product is null) { Console.WriteLine("Produto não encontrado."); return; }

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    Console.WriteLine($"Produto '{product.Name}' removido com sucesso.");
}
