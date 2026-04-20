using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class OrdenesServicioRepository
    {
        private readonly DbHelper _dbHelper;

        public OrdenesServicioRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<OrdenServicioListItem> ObtenerTodas(string? barcoId = null, string? tecnicoId = null)
        {
            var lista = new List<OrdenServicioListItem>();

            using var connection = _dbHelper.GetConnection();

            string query = @"
                SELECT O.Id, O.CodigoOrden, O.BarcoId, B.NombreEmbarcacion AS NombreBarco,
                       O.TipoMantenimiento, O.Prioridad, O.Descripcion, O.FechaLimite,
                       O.Estado, O.FechaCierreReal, O.UsuarioCierre,
                       P.NombreCompleto AS TecnicoAsignado
                FROM OrdenesServicio O
                INNER JOIN Barcos B ON O.BarcoId = B.Id
                LEFT JOIN OrdenesServicioTecnicos OT ON O.Id = OT.OrdenServicioId
                LEFT JOIN Personal P ON OT.PersonalId = P.Id
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(barcoId))
                query += " AND O.BarcoId = @BarcoId";

            if (!string.IsNullOrWhiteSpace(tecnicoId))
                query += " AND OT.PersonalId = @TecnicoId";

            query += " ORDER BY O.Id DESC";

            using var command = new SqlCommand(query, connection);

            if (!string.IsNullOrWhiteSpace(barcoId))
                command.Parameters.AddWithValue("@BarcoId", Convert.ToInt32(barcoId));

            if (!string.IsNullOrWhiteSpace(tecnicoId))
                command.Parameters.AddWithValue("@TecnicoId", Convert.ToInt32(tecnicoId));

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new OrdenServicioListItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CodigoOrden = reader["CodigoOrden"].ToString() ?? "",
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    NombreBarco = reader["NombreBarco"].ToString() ?? "",
                    TipoMantenimiento = reader["TipoMantenimiento"].ToString() ?? "",
                    Prioridad = reader["Prioridad"].ToString() ?? "",
                    Descripcion = reader["Descripcion"].ToString() ?? "",
                    FechaLimite = Convert.ToDateTime(reader["FechaLimite"]),
                    Estado = reader["Estado"].ToString() ?? "",
                    TecnicoAsignado = reader["TecnicoAsignado"] == DBNull.Value ? null : reader["TecnicoAsignado"].ToString(),
                    FechaCierreReal = reader["FechaCierreReal"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaCierreReal"]),
                    UsuarioCierre = reader["UsuarioCierre"] == DBNull.Value ? null : reader["UsuarioCierre"].ToString()
                });
            }

            return lista;
        }

        public OrdenServicio? ObtenerPorId(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                SELECT Id, CodigoOrden, BarcoId, TipoMantenimiento, Prioridad,
                       Descripcion, FechaLimite, Estado, InformeCierre,
                       FechaCierreReal, UsuarioCierre
                FROM OrdenesServicio
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new OrdenServicio
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CodigoOrden = reader["CodigoOrden"].ToString() ?? "",
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    TipoMantenimiento = reader["TipoMantenimiento"].ToString() ?? "",
                    Prioridad = reader["Prioridad"].ToString() ?? "",
                    Descripcion = reader["Descripcion"].ToString() ?? "",
                    FechaLimite = Convert.ToDateTime(reader["FechaLimite"]),
                    Estado = reader["Estado"].ToString() ?? "",
                    InformeCierre = reader["InformeCierre"] == DBNull.Value ? null : reader["InformeCierre"].ToString(),
                    FechaCierreReal = reader["FechaCierreReal"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaCierreReal"]),
                    UsuarioCierre = reader["UsuarioCierre"] == DBNull.Value ? null : reader["UsuarioCierre"].ToString()
                };
            }

            return null;
        }

        public string GenerarCodigoOrden()
        {
            using var connection = _dbHelper.GetConnection();
            string query = "SELECT COUNT(*) + 1 FROM OrdenesServicio";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            int correlativo = Convert.ToInt32(command.ExecuteScalar());
            return $"OS-{DateTime.Now:yyyyMMdd}-{correlativo:D4}";
        }

        public void Crear(OrdenServicio model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                INSERT INTO OrdenesServicio
                (CodigoOrden, BarcoId, TipoMantenimiento, Prioridad, Descripcion, FechaLimite, Estado)
                VALUES
                (@CodigoOrden, @BarcoId, @TipoMantenimiento, @Prioridad, @Descripcion, @FechaLimite, @Estado)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CodigoOrden", model.CodigoOrden);
            command.Parameters.AddWithValue("@BarcoId", model.BarcoId);
            command.Parameters.AddWithValue("@TipoMantenimiento", model.TipoMantenimiento);
            command.Parameters.AddWithValue("@Prioridad", model.Prioridad);
            command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
            command.Parameters.AddWithValue("@FechaLimite", model.FechaLimite);
            command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(model.Estado) ? "Abierta" : model.Estado);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Editar(OrdenServicio model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                UPDATE OrdenesServicio
                SET BarcoId = @BarcoId,
                    TipoMantenimiento = @TipoMantenimiento,
                    Prioridad = @Prioridad,
                    Descripcion = @Descripcion,
                    FechaLimite = @FechaLimite
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@BarcoId", model.BarcoId);
            command.Parameters.AddWithValue("@TipoMantenimiento", model.TipoMantenimiento);
            command.Parameters.AddWithValue("@Prioridad", model.Prioridad);
            command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
            command.Parameters.AddWithValue("@FechaLimite", model.FechaLimite);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void AsignarTecnico(int ordenId, int personalId)
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (var deleteCmd = new SqlCommand("DELETE FROM OrdenesServicioTecnicos WHERE OrdenServicioId = @OrdenServicioId", connection, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("@OrdenServicioId", ordenId);
                    deleteCmd.ExecuteNonQuery();
                }

                using (var insertCmd = new SqlCommand("INSERT INTO OrdenesServicioTecnicos (OrdenServicioId, PersonalId) VALUES (@OrdenServicioId, @PersonalId)", connection, transaction))
                {
                    insertCmd.Parameters.AddWithValue("@OrdenServicioId", ordenId);
                    insertCmd.Parameters.AddWithValue("@PersonalId", personalId);
                    insertCmd.ExecuteNonQuery();
                }

                using (var updateCmd = new SqlCommand("UPDATE OrdenesServicio SET Estado = 'Asignada' WHERE Id = @Id AND Estado = 'Abierta'", connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@Id", ordenId);
                    updateCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void CambiarEstado(int ordenId, string nuevoEstado)
        {
            var orden = ObtenerPorId(ordenId);
            if (orden == null)
                throw new Exception("La orden no existe.");

            if (orden.Estado == "Cerrada")
                throw new Exception("La orden cerrada no puede reabrirse sin aprobación del administrador.");

            using var connection = _dbHelper.GetConnection();
            string query = "UPDATE OrdenesServicio SET Estado = @Estado WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Estado", nuevoEstado);
            command.Parameters.AddWithValue("@Id", ordenId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void CerrarOrden(int id, string informeCierre, string usuario)
        {
            if (string.IsNullOrWhiteSpace(informeCierre))
                throw new Exception("El informe de cierre es obligatorio.");

            using var connection = _dbHelper.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var ordenQuery = "SELECT BarcoId, TipoMantenimiento FROM OrdenesServicio WHERE Id = @Id";
                int barcoId;
                string tipoMantenimiento;

                using (var ordenCmd = new SqlCommand(ordenQuery, connection, transaction))
                {
                    ordenCmd.Parameters.AddWithValue("@Id", id);
                    using var reader = ordenCmd.ExecuteReader();

                    if (!reader.Read())
                        throw new Exception("La orden no existe.");

                    barcoId = Convert.ToInt32(reader["BarcoId"]);
                    tipoMantenimiento = reader["TipoMantenimiento"].ToString() ?? "";
                }

                var updateQuery = @"
                    UPDATE OrdenesServicio
                    SET Estado = 'Cerrada',
                        InformeCierre = @InformeCierre,
                        FechaCierreReal = GETDATE(),
                        UsuarioCierre = @UsuarioCierre
                    WHERE Id = @Id";

                using (var updateCmd = new SqlCommand(updateQuery, connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@Id", id);
                    updateCmd.Parameters.AddWithValue("@InformeCierre", informeCierre);
                    updateCmd.Parameters.AddWithValue("@UsuarioCierre", usuario);
                    updateCmd.ExecuteNonQuery();
                }

                var historialQuery = @"
                    INSERT INTO HistorialTecnico (BarcoId, OrdenServicioId, TipoMantenimiento, DescripcionTrabajo)
                    VALUES (@BarcoId, @OrdenServicioId, @TipoMantenimiento, @DescripcionTrabajo)";

                using (var historialCmd = new SqlCommand(historialQuery, connection, transaction))
                {
                    historialCmd.Parameters.AddWithValue("@BarcoId", barcoId);
                    historialCmd.Parameters.AddWithValue("@OrdenServicioId", id);
                    historialCmd.Parameters.AddWithValue("@TipoMantenimiento", tipoMantenimiento);
                    historialCmd.Parameters.AddWithValue("@DescripcionTrabajo", informeCierre);
                    historialCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<TecnicoLookupDto> ObtenerTecnicos()
        {
            var lista = new List<TecnicoLookupDto>();

            using var connection = _dbHelper.GetConnection();
            string query = @"
                SELECT Id, NombreCompleto
                FROM Personal
                WHERE Activo = 1
                  AND (RolPrimario = 'Ingeniero' OR RolPrimario = 'PersonalBase')
                ORDER BY NombreCompleto";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TecnicoLookupDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreCompleto = reader["NombreCompleto"].ToString() ?? ""
                });
            }

            return lista;
        }

        public List<HistorialTecnicoItem> ObtenerHistorialPorBarco(int barcoId)
        {
            var lista = new List<HistorialTecnicoItem>();

            using var connection = _dbHelper.GetConnection();
            string query = @"
                SELECT Id, BarcoId, OrdenServicioId, TipoMantenimiento, DescripcionTrabajo, FechaRegistro
                FROM HistorialTecnico
                WHERE BarcoId = @BarcoId
                ORDER BY FechaRegistro DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BarcoId", barcoId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new HistorialTecnicoItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    OrdenServicioId = Convert.ToInt32(reader["OrdenServicioId"]),
                    TipoMantenimiento = reader["TipoMantenimiento"].ToString() ?? "",
                    DescripcionTrabajo = reader["DescripcionTrabajo"].ToString() ?? "",
                    FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
