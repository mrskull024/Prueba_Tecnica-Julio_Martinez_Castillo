using PruebaTec.Models;

namespace PruebaTec.Data.Interafaces
{
    public interface IUsuarioService
    {
        Task<decimal> CargarSaldo(string nombre);
        Task<bool> ObtenerUsuario(string nombre);
        Task<Usuario> GuardarDatos(Usuario usuario);
    }
}
