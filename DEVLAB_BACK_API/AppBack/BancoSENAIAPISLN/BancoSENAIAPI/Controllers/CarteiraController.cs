using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteira.ToListAsync();
            return Ok(carteiras);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {
            if (await _context.Carteira.AnyAsync(c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de carteira já existe." });

            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetite da carteira deve ser maior ou igual a zero." });

            _context.Carteira.Add(novaCarteira);
            await _context.SaveChangesAsync();
            return Created("", novaCarteira);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var carteira = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == codigo);

            if (carteira == null)
                return NotFound(new { message = "Carteira não encontrada." });

            return Ok(carteira);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == codigo);

            if (carteiraExistente == null) return NotFound();

            if (carteiraAtualizada.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetite da carteira deve ser maior ou igual a zero." });

            carteiraExistente.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteiraExistente.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var carteira = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == codigo);

            if (carteira == null) return NotFound();

            _context.Carteira.Remove(carteira);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Carteira excluída com sucesso." });
        }
    }
}