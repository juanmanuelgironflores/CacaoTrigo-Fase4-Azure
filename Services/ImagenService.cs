using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CacaoTrigo.Services;

public class ImagenService : IImagenService
{
    public const string CarpetaUrl = "/uploads/productos/";
    private const long TamanoMaximo = 5 * 1024 * 1024; // 5 MB

    private static readonly HashSet<string> Extensiones = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp", ".gif", ".svg"
    };

    // Un SVG es XML y puede contener scripts: se rechazan los elementos y atributos activos.
    private static readonly Regex SvgPeligroso = new(
        @"<\s*script|<\s*foreignObject|<\s*iframe|<\s*embed|<\s*object|javascript\s*:|data\s*:\s*text/html|\son\w+\s*=|<!ENTITY|<!DOCTYPE[^>]*\[",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly IWebHostEnvironment _entorno;

    public ImagenService(IWebHostEnvironment entorno)
    {
        _entorno = entorno;
    }

    private string CarpetaFisica =>
        Path.Combine(_entorno.WebRootPath, "uploads", "productos");

    public async Task<ResultadoImagen> GuardarProductoAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return new ResultadoImagen(null, "El archivo está vacío.");
        }

        if (archivo.Length > TamanoMaximo)
        {
            return new ResultadoImagen(null, "La imagen no puede pesar más de 5 MB.");
        }

        var extension = Path.GetExtension(archivo.FileName);

        if (string.IsNullOrEmpty(extension) || !Extensiones.Contains(extension))
        {
            return new ResultadoImagen(null,
                "Formato no permitido. Usa PNG, JPG, JPEG, WebP, GIF o SVG.");
        }

        extension = extension.ToLowerInvariant();

        byte[] contenido;
        using (var memoria = new MemoryStream())
        {
            await archivo.CopyToAsync(memoria);
            contenido = memoria.ToArray();
        }

        if (!ContenidoCoincide(contenido, extension))
        {
            return new ResultadoImagen(null,
                "El archivo no es una imagen válida o no coincide con su formato.");
        }

        Directory.CreateDirectory(CarpetaFisica);

        var nombre = $"{Guid.NewGuid():N}{extension}";
        await File.WriteAllBytesAsync(Path.Combine(CarpetaFisica, nombre), contenido);

        return new ResultadoImagen(CarpetaUrl + nombre, null);
    }

    public void EliminarSiEsLocal(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            !url.StartsWith(CarpetaUrl, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Solo el nombre del archivo: evita rutas como ../
        var nombre = Path.GetFileName(url);
        var ruta = Path.Combine(CarpetaFisica, nombre);

        try
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
        catch (IOException)
        {
            // Si el archivo está en uso, se deja; no debe romper la operación.
        }
    }

    // Comprueba la firma real del archivo, no solo su extensión.
    private static bool ContenidoCoincide(byte[] b, string extension)
    {
        switch (extension)
        {
            case ".png":
                return b.Length > 8 &&
                       b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47;

            case ".jpg":
            case ".jpeg":
                return b.Length > 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF;

            case ".gif":
                return b.Length > 6 &&
                       b[0] == 'G' && b[1] == 'I' && b[2] == 'F' && b[3] == '8';

            case ".webp":
                return b.Length > 12 &&
                       b[0] == 'R' && b[1] == 'I' && b[2] == 'F' && b[3] == 'F' &&
                       b[8] == 'W' && b[9] == 'E' && b[10] == 'B' && b[11] == 'P';

            case ".svg":
                var texto = System.Text.Encoding.UTF8.GetString(b);
                return texto.Contains("<svg", StringComparison.OrdinalIgnoreCase) &&
                       !SvgPeligroso.IsMatch(texto);

            default:
                return false;
        }
    }
}
