using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CacaoTrigo.Helpers;

/// <summary>Carrito de compras guardado en la sesión: ProductoId → cantidad.</summary>
public static class CarritoSesion
{
    private const string Clave = "carrito";

    public static Dictionary<int, int> Obtener(ISession sesion)
    {
        var json = sesion.GetString(Clave);

        if (string.IsNullOrEmpty(json))
        {
            return new Dictionary<int, int>();
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<int, int>>(json)
                   ?? new Dictionary<int, int>();
        }
        catch (JsonException)
        {
            return new Dictionary<int, int>();
        }
    }

    public static void Guardar(ISession sesion, Dictionary<int, int> carrito)
    {
        sesion.SetString(Clave, JsonSerializer.Serialize(carrito));
    }

    public static int Cantidad(ISession sesion) => Obtener(sesion).Values.Sum();

    public static void Vaciar(ISession sesion) => sesion.Remove(Clave);
}
