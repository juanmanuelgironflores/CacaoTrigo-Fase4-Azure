namespace CacaoTrigo.Models;

/// <summary>Nombres de los roles de la aplicación.</summary>
public static class RolesApp
{
    public const string Administrador = "Administrador";
    public const string Cliente = "Cliente";
}

/// <summary>Zonas del local donde se puede reservar.</summary>
public static class ZonasReserva
{
    public const string Terraza = "Terraza";
    public const string Interior = "Interior";

    public static readonly string[] Todas = { Terraza, Interior };

    public static bool EsValida(string? zona) =>
        zona == Terraza || zona == Interior;
}

/// <summary>Franjas horarias ofrecidas a los clientes al reservar.</summary>
public static class HorarioReservas
{
    // Ajusta estos valores al horario real del local.
    public const int HoraPrimera = 7;     // 07:00
    public const int HoraUltima = 20;     // última franja: 20:30
    public const int MinutosPorFranja = 30;
    public const int DiasMaximosDeAnticipacion = 60;
    public const int MinutosMinimosDeAnticipacion = 30;

    public static List<string> Franjas()
    {
        var lista = new List<string>();
        for (var h = HoraPrimera; h <= HoraUltima; h++)
        {
            for (var m = 0; m < 60; m += MinutosPorFranja)
            {
                lista.Add($"{h:00}:{m:00}");
            }
        }
        return lista;
    }
}
