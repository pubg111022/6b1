using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdf6b1.Models;

namespace pdf6b1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProcedureController : ControllerBase
    {
        private readonly EFCore_Exam1Context _context;

        public StoredProcedureController(EFCore_Exam1Context context)
        {
            _context = context;
        }

        // GET: api/StoredProcedure
        [HttpGet]
        [Route("{id?}")]
        public IActionResult GetAllProducts(int? id = null)
        {
            var idParameter = new SqlParameter("@Id", id ?? (object)DBNull.Value);

            var products = _context.Product
                .FromSql("EXEC Sp_getallproduct @Id", idParameter)
                .ToList();

            return Ok(products);
        }
        

        // PUT: api/StoredProcedure/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StoredProcedure
        [HttpPost]
        public IActionResult InsertProduct([FromBody] Product productInput)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Name", productInput.Name),
                    new SqlParameter("@Price", productInput.Price),
                    new SqlParameter("@Status", productInput.Status),
                    new SqlParameter("@CategoryId", productInput.CategoryId),
                    new SqlParameter("@CreateDate", productInput.CreateDate)
                };

                _context.Database.ExecuteSqlCommand("EXEC Sp_insertProduct @Name, @Price, @Status, @CategoryId, @CreateDate", parameters);

                return Ok("Product inserted successfully.");
            }
            catch (Exception ex)
            {
                // Handle exceptions and provide appropriate error response
                return BadRequest("Failed to insert product.");
            }
        }

        // DELETE: api/StoredProcedure/5
        [HttpDelete]
        [Route("{id?}")]
        public IActionResult DeleteProduct(int? id = null)
        {
            try
            {
                if (id.HasValue)
                {
                    var idParameter = new SqlParameter("@id", id);
                    _context.Database.ExecuteSqlCommand("EXEC Sp_deleteProduct @id", idParameter);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("EXEC Sp_deleteProduct");
                }

                return Ok("Product(s) deleted successfully.");
            }
            catch (Exception ex)
            {
                // Handle exceptions and provide appropriate error response
                return BadRequest("Failed to delete product(s).");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}