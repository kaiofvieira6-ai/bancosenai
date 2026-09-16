using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace BancoSENAIAPI.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(), "ClienteArquivos"
            );

        private static List<Models.DocumentoMetadado> _documentosMetadados = new List<Models.DocumentoMetadado>();

        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {

            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }


            long limiteEmBytes = 2 * 1024 * 1024; // 2 MB
            if (arquivo.Length > limiteEmBytes)
            {
                return BadRequest("O tamanho do arquivo excede o limite máximo permitido de 2 MB.");
            }

            string extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            string[] extensoesPermitidas = { ".pdf", ".jpg", ".png" };

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest($"A extensão '{extensao}' não é permitida. Apenas arquivos .pdf, .jpg e .png são homologados.");
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }


            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId++,
                Name = nameOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }

        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ListarArquivo(int codigoCliente)
        {
            var documentos = _documentosMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

            if (!documentos.Any())
            {
                return NotFound(new { mensagem = $"Nenhum documento encontrado para o cliente {codigoCliente}." });
            }

            return Ok(documentos);
        }

        [HttpGet("documento/download/{id}")]

         public IActionResult Download(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound("Arquivo físico não foi encontrado no servidor.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);
            
            return File(fileBytes, "application/octet-stream", documento.Extensao);
        }

        [HttpDelete("excluir/{id}")]

        public IActionResult Delete(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _documentosMetadados.Remove(documento);

            return Ok(new { mensagem = "Documento e arquivo físico excluídos com sucesso." });
        }
        
    }
}
