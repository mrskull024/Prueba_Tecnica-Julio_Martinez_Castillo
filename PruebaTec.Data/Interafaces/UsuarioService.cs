using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PruebaTec.Models;
using System.Data;

namespace PruebaTec.Data.Interafaces
{
    public class UsuarioService(IConfiguration configuration) : ConexionDb(configuration), IUsuarioService
    {
        public async Task<decimal> CargarSaldo(string nombre)
        {
            using var connection = Crear();
            await connection.OpenAsync();
            using var command = new SqlCommand(ProcedimientosAlmacenados.CargarSaldo, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@nombre", nombre);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetDecimal(0);
            }

            return 0;
        }

        public async Task<bool> ObtenerUsuario(string nombre)
        {
            using var connection = Crear();
            await connection.OpenAsync();
            using var command = new SqlCommand(ProcedimientosAlmacenados.ObtenerUsuario, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@nombre", nombre);
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        public async Task<Usuario?> GuardarDatos(Usuario usuario)
        {
            SqlConnection connection = null;
            SqlTransaction? transaction = null;

            try
            {
                connection = Crear();
                await connection.OpenAsync();
                transaction = connection.BeginTransaction();

                using var command = new SqlCommand(ProcedimientosAlmacenados.GuardarDatos, connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@saldo", usuario.Saldo);
                using var reader = await command.ExecuteReaderAsync();
                Usuario? usuarioGuardado = null;

                if (await reader.ReadAsync())
                {
                    usuarioGuardado = new Usuario
                    {
                        Nombre = reader.GetString(0),
                        Saldo = reader.GetDecimal(1)
                    };
                }

                bool guardadoExitoso = false;
                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        guardadoExitoso = reader.GetInt32(0) == 1;
                    }
                }

                if (guardadoExitoso && usuarioGuardado != null)
                {
                    reader.Close();
                    transaction.Commit();
                    return usuarioGuardado;
                }

                transaction?.Rollback();
                return null;
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                if (connection?.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
