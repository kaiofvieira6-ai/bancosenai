namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        public int CodigoCliente { get; set; }

        public string NomeCliente { get; set; }

        public string CPF { get; set; }

        public int NumeroAgencia { get; set; }

        public decimal SaldoTotal { get; set; } = 0.0m;
    }
}
