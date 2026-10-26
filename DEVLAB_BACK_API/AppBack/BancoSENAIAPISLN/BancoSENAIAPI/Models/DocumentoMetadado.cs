namespace BancoSENAIAPI.Models
{
    public class DocumentoMetadados
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Extensão { get; set; }

        public string Caminho { get; set; }

        public int CodigoCliente { get; set; }
    }
}

