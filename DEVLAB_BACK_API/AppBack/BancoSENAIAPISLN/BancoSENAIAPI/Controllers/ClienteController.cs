using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private static List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente
            {
                CodigoCliente = 1,
                NomeCliente = "João Silva",
                CPF = "123.456.789-00",
                NumeroAgencia = 10,
                SaldoTotal = 0,
                Sexo = "M",
                Endereco = "Rua A, 123",
                Cidade = "São Paulo",
                Estado = "SP"
            }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_clientes);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente novoCliente)
        {
            if (string.IsNullOrWhiteSpace(novoCliente.NomeCliente))
                return BadRequest(new { message = "O nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(novoCliente.CPF))
                return BadRequest(new { message = "O CPF é obrigatório." });

            novoCliente.CodigoCliente = _clientes.Any() ? _clientes.Max(c => c.CodigoCliente) + 1 : 1;

            if (novoCliente.NumeroAgencia == 0) novoCliente.NumeroAgencia = 10;

            _clientes.Add(novoCliente);
            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            return Ok(cliente);
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = _clientes.FirstOrDefault(c => c.CodigoCliente == codigo);

            if (clienteExistente == null) return NotFound();

            if (string.IsNullOrWhiteSpace(clienteAtualizado.NomeCliente))
                return BadRequest(new { message = "O nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(clienteAtualizado.CPF))
                return BadRequest(new { message = "O CPF é obrigatório." });

            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.CPF = clienteAtualizado.CPF;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;
            clienteExistente.Sexo = clienteAtualizado.Sexo;
            clienteExistente.Endereco = clienteAtualizado.Endereco;
            clienteExistente.Cidade = clienteAtualizado.Cidade;
            clienteExistente.Estado = clienteAtualizado.Estado;

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(c => c.CodigoCliente == codigo);

            if (cliente == null) return NotFound();

            _clientes.Remove(cliente);
            return Ok(new { message = "Cliente excluído com sucesso." });
        }
    }
}