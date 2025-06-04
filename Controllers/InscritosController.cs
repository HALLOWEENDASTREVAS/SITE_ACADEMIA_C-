using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademiaAPI.Data;
using AcademiaAPI.Models;

namespace AcademiaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscritosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InscritosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Inscritos/ - Busca inscri��o espec�fica ou todos, a depender dos par�metros
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Inscrito>>> GetInscrito (
                [FromQuery] int? idAluno,
                [FromQuery] int? idAula
            )
        {
            var query = _context.Inscritos
                .Include(i => i.Aluno)
                .Include(i => i.Aula)
                .AsQueryable();

            // Verifica se foi informado o ID do Aluno
            if (idAluno.HasValue)
            {
                query = query.Where(i => i.IdAluno == idAluno.Value);
            }

            // Verifica se foi informado o ID da Aula
            if (idAula.HasValue)
            {
                query = query.Where(i => i.IdAula == idAula.Value);
            }

            // verifica se a consulta retornou algum resultado
            if (!query.Any()) return NotFound("Nenhum inscrito encontrado com os filtros selecionados!");

            // Pega a lista de inscritos filtrada
            var inscrito = await query.ToListAsync();

            return inscrito;
        }

        // GET: api/Inscritos/filter - Busca relat�rio de aulas por aluno
        [HttpGet("filterRel")]
        public async Task<ActionResult<IEnumerable<Inscrito>>> GetRelPorAluno(int idAluno, int? mes, int? ano)
        {
            var dataAtual = DateTime.Now; // Data atual
            int mesRef = mes ?? dataAtual.Month; // Se n�o foi informado, usa o m�s atual
            int anoRef = ano ?? dataAtual.Year;  // Se n�o foi informado, usa o ano atual

            // Buscar inscri��es do aluno no m�s/ano
            var inscricoes = await _context.Inscritos
                .Where(i => i.IdAluno == idAluno && i.Aula.DataAula.Month == mesRef && i.Aula.DataAula.Year == anoRef)
                .Include(i => i.Aula)
                .ToListAsync();

            if (!_context.Alunos.Any(a => a.IdAluno == idAluno))
                return NotFound("Aluno n�o encontrado.");

            int totalAulas = inscricoes.Count;

            // Agrupa os tipos de aulas mais frequentadas pelo aluno
            var tiposMaisFrequentes = inscricoes
                .GroupBy(i => i.Aula.TipoAula)
                .Select(g => new RelatorioTipoAula
                    {
                        TipoAula = g.Key,
                        Quantidade = g.Count()
                    })
                .OrderByDescending(g => g.Quantidade)
                .ToList();

            var relatorio = new RelatorioAluno
            {
                IdAluno = idAluno,
                TotalAulasNoMes = totalAulas,
                TiposAulaMaisFrequente = tiposMaisFrequentes
            };

            return Ok(relatorio);
        }

        // POST: api/Inscritos - Cria uma nova inscri��o
        [HttpPost]
        public async Task<ActionResult<Inscrito>> PostInscrito(Inscrito inscrito)
        {
            // Verifica exist�ncia de Aluno e Aula
            var aluno = await _context.Alunos.FindAsync(inscrito.IdAluno);
            var aula = await _context.Aulas.FindAsync(inscrito.IdAula);

            // Se n�o encontrar, retorna erro
            if (aluno == null) return BadRequest("Aluno n�o encontrado!");

            if (aula == null) return BadRequest("Aula n�o encontrada!");

            if (_context.Inscritos.Any(e => e.IdAluno == inscrito.IdAluno && e.IdAula == inscrito.IdAula))
            {
                return BadRequest("O aluno j� est� inscrito nesta aula.");
            }

            // Valida��o para Respeitar capacidade da aula
            if (aula.Confirmados >= aula.Capacidade) return BadRequest("Aula atingiu capacidade m�xima!");

            // Valida��o para Respeitar limite de agendamentos do aluno
            if (aluno.AulasFeitas >= aluno.AulasPlano) return BadRequest("Aluno j� atingiu o n�mero m�ximo de aulas do plano!");

            // Se tudo OK, cria inscri��o
            _context.Inscritos.Add(inscrito);

            // Incrementa numero de confirmados na aula
            aula.Confirmados += 1;
            _context.Aulas.Update(aula);

            // Incrementa numero de aulas feitas pelo aluno
            aluno.AulasFeitas += 1;
            _context.Alunos.Update(aluno);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInscrito),
                new { idAluno = inscrito.IdAluno, idAula = inscrito.IdAula },
                inscrito);
        }

        // DELETE: api/Inscritos/{idAluno}/{idAula}
        [HttpDelete("{idAluno}/{idAula}")]
        public async Task<IActionResult> DeleteInscrito(int idAluno, int idAula)
        {
            var inscrito = await _context.Inscritos.FindAsync(idAluno, idAula);
            if (inscrito == null) return NotFound();

            // Antes de remover, decrementa Confirmados e AulasFeitas
            var aula = await _context.Aulas.FindAsync(idAula);
            var aluno = await _context.Alunos.FindAsync(idAluno);

            // Verifica se a aula existe e reduz o numero de confirmados
            if (aula != null && aula.Confirmados > 0)
            {
                aula.Confirmados -= 1;
                _context.Aulas.Update(aula);
            }

            // Verifica se o aluno existe e reduz o numero de aulas feitas
            if (aluno != null && aluno.AulasFeitas > 0)
            {
                aluno.AulasFeitas -= 1;
                _context.Alunos.Update(aluno);
            }

            _context.Inscritos.Remove(inscrito);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
