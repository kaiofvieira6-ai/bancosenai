namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        public int CodigoCliente { get; set; }
        public required string NomeCliente { get; set; }
        public required string CPF { get; set; }
        public int NumeroAgencia { get; set; } = 10;
        public int SaldoTotal { get; set; } = 0;
        public string Sexo { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

    }
}
