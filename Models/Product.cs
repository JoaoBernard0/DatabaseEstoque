using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstoqueApi.Data;
using EstoqueApi.DTOs;
using EstoqueApi.Models;

namespace EstoqueApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    // GET: /api/v1/products?name=abc
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll([FromQuery] string? name)
    {
        var query = _db.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));
        var list = await query.OrderBy(p => p.Id).ToListAsync();
        return Ok(list);
    }

    // GET: /api/v1/products/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound(new { message = "Produto não encontrado." });
        return Ok(product);
    }

    // POST: /api/v1/products
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        if (!string.IsNullOrWhiteSpace(dto.SKU))
        {
            var existsSku = await _db.Products.AnyAsync(p => p.SKU == dto.SKU);
            if (existsSku) return Conflict(new { error = "SKU já cadastrado." });
        }

        if (await _db.Products.AnyAsync(p => p.Name == dto.Name))
            return Conflict(new { error = "Um produto com este nome já foi cadastrado." });

        var product = new Product
        {
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            SKU = dto.SKU,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: /api/v1/products/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existing = await _db.Products.FindAsync(id);
        if (existing is null) return NotFound(new { message = "Produto não encontrado para atualização." });

        // Check name uniqueness
        if (await _db.Products.AnyAsync(p => p.Name == dto.Name && p.Id != id))
            return Conflict(new { error = "O nome informado já está em uso por outro produto." });

        // Check SKU uniqueness
        if (!string.IsNullOrWhiteSpace(dto.SKU) && await _db.Products.AnyAsync(p => p.SKU == dto.SKU && p.Id != id))
            return Conflict(new { error = "SKU já está em uso por outro produto." });

        existing.Name = dto.Name;
        existing.Category = dto.Category;
        existing.Price = dto.Price;
        existing.SKU = dto.SKU;

        _db.Entry(existing).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/v1/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound(new { message = "Produto não encontrado para exclusão." });

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
