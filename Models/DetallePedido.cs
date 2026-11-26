using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class DetallePedido
{
    public int DetallePedidoId { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Range(1, 999)]
    public int Cantidad { get; set; }

    [Range(typeof(decimal), "0.01", "999999.99")]
    public decimal PrecioUnitario { get; set; }

    [Range(typeof(decimal), "0.00", "999999.99")]
    public decimal Subtotal { get; set; }
}