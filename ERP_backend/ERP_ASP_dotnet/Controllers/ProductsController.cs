using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP_backend.Data;

namespace ERP_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(AppDb db) : ControllerBase
    {
        private readonly AppDb _db = db;

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll() =>
            await _db.Products.AsNoTracking().ToListAsync();

        // GET /api/products/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var p = await _db.Products.FindAsync(id);
            return p is null ? NotFound() : p;
        }

        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product input)
        {
            _db.Products.Add(input);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        // PUT /api/products/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Product input)
        {
            if (id != input.Id) return BadRequest("ID mismatch");
            var exists = await _db.Products.AnyAsync(p => p.Id == id);
            if (!exists) return NotFound();

            _db.Entry(input).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE /api/products/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p is null) return NotFound();
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
