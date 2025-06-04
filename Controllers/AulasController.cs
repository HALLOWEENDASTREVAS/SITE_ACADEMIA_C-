using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademiaAPI.Data;
using AcademiaAPI.Models;

namespace AcademiaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AulasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AulasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Aulas - Busca um ou mais aulas
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Aula>>> GetAula(
                [FromQuery] int? idAula
            )
        {
            // Prepara a query para buscar os dados
            var query = _context.Aulas.AsQueryable();

            // Verifica se foi informado o ID
            if (idAula.HasValue)
            {
                query = query.Where(i => i.IdAula == idAula.Value);
            }

            // verifica se a consulta retornou algum resultado
            if (!query.Any()) return NotFound("Nenhuma aula encontrado com os filtros selecionados!");

            // Pega a lista filtrada e retorna
            var vresult = await query.ToListAsync();

            return vresult;
        }

        // POST: api/Aulas - Cria uma nova aula
        [HttpPost]
        public async Task<ActionResult<Aula>> PostAula(Aula aula)
        {
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAula), new { id = aula.IdAula }, aula);
        }

        // PUT: api/Aulas/{id} - Atualiza uma aula existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAula(int id, Aula aula)
        {
            if (id != aula.IdAula) return BadRequest();

            _context.Entry(aula).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Aulas.Any(e => e.IdAula == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Aulas/{id} - Deleta uma aula pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAula(int id)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula == null) return NotFound();

            _context.Aulas.Remove(aula);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
