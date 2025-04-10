namespace PruebaTec.Models
{
    public record ApuestaReq(
        string Nombre,
        TiposApuesta TiposApuesta,
        string? Color,
        string? Paridad,
        int? Numero,
        decimal Monto,
        decimal Saldo
    );
}
