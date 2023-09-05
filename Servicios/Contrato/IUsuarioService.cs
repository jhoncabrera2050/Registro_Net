using Microsoft.EntityFrameworkCore;
using Cabrera.Models;

namespace Cabrera.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string correo, string clave);
        Task<Usuario> SaveUsuario(Usuario modelo);
    }
}
