using System.Text.Json.Serialization;

namespace PruebaTec.Models
{
    public class Usuario
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}
