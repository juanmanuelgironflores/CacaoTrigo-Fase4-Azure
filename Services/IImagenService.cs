using Microsoft.AspNetCore.Http;

namespace CacaoTrigo.Services;

public record ResultadoImagen(string? Url, string? Error)
{
    public bool Exitoso => Error == null;
}

public interface IImagenService
{
    /// <summary>Valida y guarda una imagen de producto en wwwroot/uploads/productos.</summary>
    Task<ResultadoImagen> GuardarProductoAsync(IFormFile archivo);

    /// <summary>Borra una imagen previamente subida (ignora URLs externas).</summary>
    void EliminarSiEsLocal(string? url);
}
