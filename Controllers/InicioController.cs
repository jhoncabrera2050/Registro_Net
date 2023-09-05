using Microsoft.AspNetCore.Mvc;
using Cabrera.Models;
using Cabrera.Recursos;
using Cabrera.Servicios.Contrato;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic; // Agregamos esto para usar List<Claim>
using System.Threading.Tasks; // Agregamos esto para usar Task

namespace Cabrera.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
          
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario modelo)
        {
            // Validación de modelo omitida por simplicidad
            modelo.Clave = Utilidades.EncriptarClave(modelo.Clave);
            Usuario usuario_creado = await _usuarioService.SaveUsuario(modelo);
            if (usuario_creado != null && usuario_creado.IdUsuario > 0)
            {
                return RedirectToAction("IniciarSesion","Inicio");
            }
            ViewData["Mensaje"] = "No se puede crear el usuario";
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string clave)
        {
            // Validación de entrada y lógica de autenticación
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
            {
                ViewData["Mensaje"] = "Correo y clave son obligatorios";
                return View();
            }

            Usuario usuario_encontrado = await _usuarioService.GetUsuario(correo, Utilidades.EncriptarClave(clave));
            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            // Creación de claims y autenticación
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.NombreUsuario)
                // Puedes agregar más claims según sea necesario
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }
    }
}
