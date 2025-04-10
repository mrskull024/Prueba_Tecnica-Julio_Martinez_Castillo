using Microsoft.AspNetCore.Mvc;
using PruebaTec.Data.Interafaces;
using PruebaTec.Models;

namespace PrruebaTec.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;

        [HttpGet("Obtener/{nombre}")]
        public async Task<IActionResult> ObtenerUsuario(string nombre)
        {
            var usuario = await _usuarioService.ObtenerUsuario(nombre.Trim().ToLower());

            return Ok(usuario);
        }

        [HttpGet("CargarSaldo/{nombre}")]
        public async Task<IActionResult> CargarSaldo(string nombre)
        {
            var saldoUsuario = await _usuarioService.CargarSaldo(nombre.Trim().ToLower());

            return saldoUsuario is not 0
                ? Ok(new { saldo = saldoUsuario })
                : NotFound($"No se encontró saldo con nombre {nombre.Trim().ToLower()}");
        }

        [HttpPost("GuardarDatos")]
        public async Task<IActionResult> GuardarDatos([FromBody] Usuario usuarioReq)
        {
            if (usuarioReq.Saldo < 0 || usuarioReq.Nombre.Trim() == string.Empty)
            {
                return BadRequest($"Saldo debe ser mayor o igual a 0 y nombre no debe estar vacio");
            }

            var usuario = new Usuario
            {
                Nombre = usuarioReq.Nombre.Trim().ToLower(),
                Saldo = usuarioReq.Saldo
            };

            var resultado = await _usuarioService.GuardarDatos(usuario);

            return resultado is not null ? Ok(new
            {
                nombre = resultado?.Nombre ?? usuarioReq.Nombre,
                saldo = resultado?.Saldo ?? usuarioReq.Saldo
            }) : BadRequest($"Ocurrio un error al gurdar los datos para el usuario {usuarioReq.Nombre}");
        }
    }
}
