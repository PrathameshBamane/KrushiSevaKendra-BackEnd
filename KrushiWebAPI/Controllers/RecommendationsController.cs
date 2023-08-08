using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KrushiWebAPI.Models;
using KrushiWebAPI.context;

namespace KrushiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public RecommendationsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Recommendations
        [HttpGet ("all/{farmerid}")]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetRecommendations([FromRoute]int farmerid)
        {
         var recommendations=(from r in _context.Recommendations
                              join f in _context.Farmers on r.Farmerid equals f.Id
                              join c in _context.Crops on r.Cropid equals c.Id
                              where r.Farmerid == farmerid
                              select new
                              {
                                  id = r.Id,
                                  farmer=f.Name, crops=c.Name,message=r.Message
                              }
                              ).ToList();
            return Ok(recommendations);
        }

        // GET: api/Recommendations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recommendation>> GetRecommendation([FromRoute]int id)
        {
          if (_context.Recommendations == null)
          {
              return NotFound();
          }
            var recommendation = await _context.Recommendations.FindAsync(id);

            if (recommendation == null)
            {
                return NotFound();
            }

            return recommendation;
        }

        // PUT: api/Recommendations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecommendation(int id, Recommendation recommendation)
        {
            if (id != recommendation.Id)
            {
                return BadRequest();
            }

            _context.Entry(recommendation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecommendationExists(id))
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

        // POST: api/Recommendations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recommendation>> PostRecommendation(Recommendation recommendation)
        {
          if (_context.Recommendations == null)
          {
              return Problem("Entity set 'DatabaseContext.Recommendations'  is null.");
          }
            _context.Recommendations.Add(recommendation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecommendation", new { id = recommendation.Id }, recommendation);
        }

        // DELETE: api/Recommendations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecommendation(int id)
        {
            if (_context.Recommendations == null)
            {
                return NotFound();
            }
            var recommendation = await _context.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                return NotFound();
            }

            _context.Recommendations.Remove(recommendation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecommendationExists(int id)
        {
            return (_context.Recommendations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
