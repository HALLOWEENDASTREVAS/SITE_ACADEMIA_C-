using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademiaAPI.Data;
using AcademiaAPI.Models;

namespace AcademiaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlunosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Alunos - Busca um ou todos os Alunos
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Aluno>>> GetAluno (
                [FromQuery] int? idAluno
            )
        {
            // Prepara a query para buscar os dados
            var query = _context.Alunos.AsQueryable();

            // Verifica se foi informado o ID
            if (idAluno.HasValue)
            {
                query = query.Where(i => i.IdAluno == idAluno.Value);
            }

            // verifica se a consulta retornou algum resultado
            if (!query.Any()) return NotFound("Nenhum aluno encontrado com os filtros selecionados!");

            // Pega a lista filtrada e retorna
            var vresult = await query.ToListAsync();

            return vresult;
        }

        // POST: api/Alunos - Cria um novo aluno
        [HttpPost]
        public async Task<ActionResult<Aluno>> PostAluno(Aluno aluno)
        {
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAluno), new { id = aluno.IdAluno }, aluno);
        }

        // PUT: api/Alunos/{id} - Atualiza um aluno existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAluno(int id, Aluno aluno)
        {
            if (id != aluno.IdAluno) return BadRequest();

            _context.Entry(aluno).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Alunos.Any(e => e.IdAluno == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Alunos/{id} - Deleta um aluno pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluno(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null) return NotFound();

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
