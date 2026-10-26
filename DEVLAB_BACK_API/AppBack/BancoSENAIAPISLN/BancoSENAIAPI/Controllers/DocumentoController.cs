using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _caminhoRaiz = Path.Combine(Directory.GetCurrentDirectory(), "ClienteArquivos");
        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado");
            }
            if (arquivo.Length > 2 * 1024 * 1024)
            {
                return BadRequest("O arquivo não pode ter mais de 2 MB.");
            }
            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }
            string extensao = Path.GetExtension(arquivo.FileName);
            string[] extensoesPermitidas = { ".pdf", ".jpg", ".png" };

            if (!extensoesPermitidas.Contains(extensao.ToLower()))
            {
                return BadRequest("Extensão de arquivo não permitida. Apenas .pdf, .jpg e .png são aceitos.");
            }
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}{nomeOriginal}{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            var documentoMetadados = new Models.DocumentoMetadados
            {

                Name = nomeOriginal,
                Extensão = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };
            _context.DocumentoMetadados.Add(documentoMetadados);
            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });

        }
        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> listarDocumentos(int codigoCliente)
        {
            var documentos = await _context.DocumentoMetadados
            .Where(d => d.CodigoCliente == codigoCliente)
            .ToListAsync();

            if (!documentos.Any())
            {
                return NotFound("Nenhum Documento foi encontrado para este cliente");
            }
            return Ok(documentos);
        }
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocumentos(int id)
        {
            var documentos = await _context.DocumentoMetadados.FirstOrDefaultAsync(d => d.ID == id);

            if (documentos == null)
            {
                return NotFound("Documento não encontrado");
            }
            if (!System.IO.File.Exists(documentos.Caminho))
            {
                return NotFound("Arquivo não encontrado no servidor.");
            }

            byte[] arquivo = await System.IO.File.ReadAllBytesAsync(documentos.Caminho);
            return File(arquivo, "application/octet-stream", documentos.Name + documentos.Extensão);

        }
        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> ExcluirDocumentos(int id)
        {
            var documento = await _context.DocumentoMetadados.FirstOrDefaultAsync(d => d.ID == id);

            if (documento == null)
            {
                return NotFound("Arquivo não encontrado");
            }
            if (System.IO.File.Exists(documento.Caminho))
                System.IO.File.Delete(documento.Caminho);
            _context.DocumentoMetadados.Remove(documento);

            await _context.SaveChangesAsync();

            return Ok("Documento Excluído com sucesso.");
        }

    }
}