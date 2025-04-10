using Microsoft.AspNetCore.Mvc;
using PruebaTec.Models;

namespace PrruebaTec.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuletaController : ControllerBase
    {
        private static readonly int[] numerosArray = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

        [HttpPost("Apostar")]
        public IActionResult Apostar([FromBody] ApuestaReq request)
        {
            decimal nuevoSaldo;
            var random = new Random();
            int numero = random.Next(0, 37);
            string color = (numero == 0) ?
                "verde" :
                (numerosArray.Contains(numero) ? "rojo" : "negro");
            bool esPar = numero % 2 == 0;
            decimal premio = 0;
            string mensaje = "Perdiste la apuesta";

            switch (request.TiposApuesta)
            {
                case TiposApuesta.COLOR:
                    if (request.Color == color)
                    {
                        premio = request.Monto / 2;
                        mensaje = "Ganaste por color.";
                    }
                    break;
                case TiposApuesta.PAR_IMPAR_COLOR:
                    if (color.Equals(request.Color) &&
                      (esPar && request.Paridad!.Equals("par") ||
                      (!esPar && request.Paridad!.Equals("impar"))))
                    {
                        premio = request.Monto;
                        mensaje = "Ganaste por paridad de color.";
                    }
                    break;
                case TiposApuesta.NUMERO_COLOR:
                    if (numero == request.Numero && color.Equals(request.Color))
                    {
                        premio = request.Monto * 3;
                        mensaje = "Ganaste por número y color.";
                    }
                    break;
            }

            nuevoSaldo = string.Equals(mensaje, "Perdiste la apuesta")
             ? request.Saldo - request.Monto
             : request.Saldo + premio;

            return Ok(new
            {
                numero,
                color,
                mensaje,
                premio,
                nuevoSaldo
            });
        }
    }
}
