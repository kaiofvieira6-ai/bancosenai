using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var clientes = await _context.Cliente.ToListAsync();

            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Cliente novoCliente)
        {
            if (string.IsNullOrWhiteSpace(novoCliente.NomeCliente))
                return BadRequest(new { message = "O nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(novoCliente.CPF))
                return BadRequest(new { message = "O CPF é obrigatório." });

            if (novoCliente.NumeroAgencia == 0)
                novoCliente.NumeroAgencia = 10;
            _context.Cliente.Add(novoCliente);

            await _context.SaveChangesAsync();

            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            return Ok(cliente);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (clienteExistente == null) return NotFound();

            if (string.IsNullOrWhiteSpace(clienteAtualizado.NomeCliente))
                return BadRequest(new { message = "O nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(clienteAtualizado.CPF))
                return BadRequest(new { message = "O CPF é obrigatório." });

            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.CPF = clienteAtualizado.CPF;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null) return NotFound();

            _context.Cliente.Remove(cliente); await _context.SaveChangesAsync();
            return Ok(new { message = "Cliente excluído com sucesso." });
        }
    }
}