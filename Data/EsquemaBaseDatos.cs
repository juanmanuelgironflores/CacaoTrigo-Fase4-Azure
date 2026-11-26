using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Data;

/// <summary>
/// Sentencias SQL idempotentes que dejan la base de datos con las columnas de
/// clientes (UsuarioId, Zona, MesaId opcional). Se pueden ejecutar varias veces
/// sin problema: solo cambian lo que falta. Las usan la migración y el arranque.
/// </summary>
public static class EsquemaBaseDatos
{
    public static readonly string[] Sentencias =
    {
        // Columnas nuevas
        "IF COL_LENGTH(N'dbo.Pedidos', N'UsuarioId') IS NULL ALTER TABLE dbo.Pedidos ADD UsuarioId nvarchar(450) NULL;",
        "IF COL_LENGTH(N'dbo.Reservas', N'UsuarioId') IS NULL ALTER TABLE dbo.Reservas ADD UsuarioId nvarchar(450) NULL;",
        "IF COL_LENGTH(N'dbo.Reservas', N'Zona') IS NULL ALTER TABLE dbo.Reservas ADD Zona nvarchar(20) NULL;",

        // La mesa de una reserva pasa a ser opcional
        @"IF EXISTS (SELECT 1 FROM sys.columns
                     WHERE object_id = OBJECT_ID(N'dbo.Reservas') AND name = N'MesaId' AND is_nullable = 0)
          BEGIN
              IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Reservas_Mesas_MesaId')
                  ALTER TABLE dbo.Reservas DROP CONSTRAINT FK_Reservas_Mesas_MesaId;
              IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservas_MesaId' AND object_id = OBJECT_ID(N'dbo.Reservas'))
                  DROP INDEX IX_Reservas_MesaId ON dbo.Reservas;
              ALTER TABLE dbo.Reservas ALTER COLUMN MesaId int NULL;
              CREATE INDEX IX_Reservas_MesaId ON dbo.Reservas (MesaId);
              ALTER TABLE dbo.Reservas ADD CONSTRAINT FK_Reservas_Mesas_MesaId
                  FOREIGN KEY (MesaId) REFERENCES dbo.Mesas (MesaId) ON DELETE SET NULL;
          END",

        // Índices
        "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_UsuarioId' AND object_id = OBJECT_ID(N'dbo.Pedidos')) CREATE INDEX IX_Pedidos_UsuarioId ON dbo.Pedidos (UsuarioId);",
        "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservas_UsuarioId' AND object_id = OBJECT_ID(N'dbo.Reservas')) CREATE INDEX IX_Reservas_UsuarioId ON dbo.Reservas (UsuarioId);",

        // Relaciones con el usuario
        "IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Pedidos_AspNetUsers_UsuarioId') ALTER TABLE dbo.Pedidos ADD CONSTRAINT FK_Pedidos_AspNetUsers_UsuarioId FOREIGN KEY (UsuarioId) REFERENCES dbo.AspNetUsers (Id) ON DELETE SET NULL;",
        "IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Reservas_AspNetUsers_UsuarioId') ALTER TABLE dbo.Reservas ADD CONSTRAINT FK_Reservas_AspNetUsers_UsuarioId FOREIGN KEY (UsuarioId) REFERENCES dbo.AspNetUsers (Id) ON DELETE SET NULL;"
    };

    /// <summary>Ejecuta las sentencias, una por una (cada una es su propio lote).</summary>
    public static async Task AsegurarAsync(DbContext contexto, CancellationToken token = default)
    {
        foreach (var sql in Sentencias)
        {
            await contexto.Database.ExecuteSqlRawAsync(sql, token);
        }
    }
}
