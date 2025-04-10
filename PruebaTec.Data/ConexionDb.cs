using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PruebaTec.Data
{
    public class ConexionDb(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public SqlConnection Crear()
        {
            return new SqlConnection(_connectionString);
        }
    }

    public class ProcedimientosAlmacenados
    {
        public const string CargarSaldo = "spCargarSaldo";
        public const string ObtenerUsuario = "spObtenerUsuario";
        public const string GuardarDatos = "spGuardarDatos";
    }
}
