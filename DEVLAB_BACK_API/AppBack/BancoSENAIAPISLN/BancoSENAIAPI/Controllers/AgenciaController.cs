using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class AgenciaController : ControllerBase
    {
        private readonly AppDbContext _context;
      

        public AgenciaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var agencias = await _context.Agencia.ToListAsync();
            return Ok(agencias);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Agencia novaAgencia)
        {

            if (await _context.Agencia.AnyAsync(a => a.NumeroAgencia == novaAgencia.NumeroAgencia))
                return BadRequest(new { message = "Este número de agência já existe." });

            _context.Agencia.Add(novaAgencia);
            await _context.SaveChangesAsync();
            return Created("", novaAgencia);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var agencia = await _context.Agencia.FirstOrDefaultAsync(a => a.NumeroAgencia == codigo);

            if (agencia == null)
                return NotFound(new { message = "Agência não encontrada." });

            return Ok(agencia);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Agencia agenciaAtualizada)
        {
            var agenciaExistente = await _context.Agencia.FirstOrDefaultAsync(a => a.NumeroAgencia == codigo);

            if (agenciaExistente == null) return NotFound();

            agenciaExistente.Cidade = agenciaAtualizada.Cidade;
            agenciaExistente.SiglaEstado = agenciaAtualizada.SiglaEstado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var agencia = await _context.Agencia.FirstOrDefaultAsync(a => a.NumeroAgencia == codigo);

            if (agencia == null) return NotFound();

            _context.Agencia.Remove(agencia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Agência excluída com sucesso." }); // Status 200 [6]
        }
    }
}