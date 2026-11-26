using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models.ViewModels;

public class MenuViewModel
{
    public List<Categoria> Categorias { get; set; } = new();
    public List<Producto> Productos { get; set; } = new();
    public int? CategoriaSeleccionada { get; set; }
}

public class LineaCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
}

public class DatosPedidoViewModel
{
    public const string Recoger = "Recoger";
    public const string Domicilio = "Domicilio";

    [Required(ErrorMessage = "Indica cómo quieres recibir tu pedido.")]
    [Display(Name = "Modalidad")]
    public string Modalidad { get; set; } = Recoger;

    [Required(ErrorMessage = "Escribe el nombre de quien recibe el pedido.")]
    [StringLength(120)]
    [Display(Name = "Nombre de contacto")]
    public string NombreContacto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe un teléfono de contacto.")]
    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string TelefonoContacto { get; set; } = string.Empty;

    [StringLength(250)]
    [Display(Name = "Dirección de entrega")]
    public string? DireccionEntrega { get; set; }
}

public class CarritoViewModel
{
    public List<LineaCarrito> Lineas { get; set; } = new();
    public decimal Total => Lineas.Sum(l => l.Subtotal);
    public DatosPedidoViewModel Datos { get; set; } = new();
}

public class PedidoHistorialViewModel
{
    public Pedido Pedido { get; set; } = null!;
    public List<DetallePedido> Detalles { get; set; } = new();
}

public class ReservaClienteViewModel
{
    [Required(ErrorMessage = "Elige dónde quieres sentarte.")]
    [Display(Name = "Zona")]
    public string Zona { get; set; } = ZonasReserva.Interior;

    [Required(ErrorMessage = "Elige la fecha de tu reserva.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "Elige la hora de tu reserva.")]
    [Display(Name = "Hora")]
    public string Hora { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indica cuántas personas serán.")]
    [Range(1, 20, ErrorMessage = "Puedes reservar de 1 a 20 personas.")]
    [Display(Name = "Número de personas")]
    public int? NumeroPersonas { get; set; } = 2;

    [Required(ErrorMessage = "Escribe el nombre a nombre de quien reservas.")]
    [StringLength(120)]
    [Display(Name = "Nombre de contacto")]
    public string NombreContacto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe un teléfono de contacto.")]
    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string TelefonoContacto { get; set; } = string.Empty;
}
