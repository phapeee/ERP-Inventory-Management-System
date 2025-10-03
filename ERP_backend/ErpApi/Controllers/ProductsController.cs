using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpApi.Data.Generated;
using ErpApi.Data.Generated.Entities;
using System.Text.Json.Serialization;

namespace ErpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SuppressMessage("Design", "CA1515:Consider marking type as internal", Justification = "Controllers must stay public for ASP.NET Core discovery.")]
    public class ProductsController(AppDb db) : ControllerBase
    {
        private readonly AppDb _db = db;

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll() =>
            await _db.Products.AsNoTracking()
                .Select(p => ToDto(p))
                .ToListAsync();

        // GET /api/products/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _db.Products.FindAsync(id);
            return product is null ? NotFound() : ToDto(product);
        }

        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductRequest input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var entity = new Product { Name = input.Name, Price = input.Price };
            _db.Products.Add(entity);
            await _db.SaveChangesAsync();

            var dto = ToDto(entity);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // PUT /api/products/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ProductRequest input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var entity = await _db.Products.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Name = input.Name;
            entity.Price = input.Price;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE /api/products/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Products.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static ProductDto ToDto(Product product) => new(product.Id, product.Name, product.Price);
    }

    [SuppressMessage("Design", "CA1515:Consider marking type as internal", Justification = "Returned to clients as part of the public API contract.")]
    public record ProductDto(int Id, string Name, decimal Price);

    [SuppressMessage("Design", "CA1515:Consider marking type as internal", Justification = "Used by model binding for incoming requests.")]
    public record ProductRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;

        [Required]
        [JsonRequired]
        public decimal Price { get; init; }
    }
}
