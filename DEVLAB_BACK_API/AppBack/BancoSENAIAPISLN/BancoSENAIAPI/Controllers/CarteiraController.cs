using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private static List<Carteira> _carteiras = new List<Carteira>
        {
            new Carteira { NumeroCarteira = 1, NomeCarteira = "Agro", ApetiteCarteira = 1000000.00m },
            new Carteira { NumeroCarteira = 2, NomeCarteira = "Atacado", ApetiteCarteira = 1500000.00m },
            new Carteira { NumeroCarteira = 3, NomeCarteira = "Varejo", ApetiteCarteira = 2000000.00m }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_carteiras);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Carteira novaCarteira)
        {
            if (_carteiras.Any(c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de carteira já existe." });

            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetite da carteira deve ser maior ou igual a zero." });

            _carteiras.Add(novaCarteira);
            return Created("", novaCarteira);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var carteira = _carteiras.FirstOrDefault(c => c.NumeroCarteira == codigo);

            if (carteira == null)
                return NotFound(new { message = "Carteira não encontrada." });

            return Ok(carteira);
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = _carteiras.FirstOrDefault(c => c.NumeroCarteira == codigo);

            if (carteiraExistente == null) return NotFound();

            if (carteiraAtualizada.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetite da carteira deve ser maior ou igual a zero." });

            carteiraExistente.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteiraExistente.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var carteira = _carteiras.FirstOrDefault(c => c.NumeroCarteira == codigo);

            if (carteira == null) return NotFound();

            _carteiras.Remove(carteira);
            return Ok(new { message = "Carteira excluída com sucesso." });
        }
    }
}