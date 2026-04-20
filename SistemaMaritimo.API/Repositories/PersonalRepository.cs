using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class PersonalRepository
    {
        private readonly DbHelper _dbHelper;

        public PersonalRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Personal> ObtenerTodos()
        {
            var lista = new List<Personal>();

            using var connection = _dbHelper.GetConnection();
            var query = @"SELECT Id, NombreCompleto, IdentificacionUnica, RolPrimario, FechaContratacion, Activo
                          FROM Personal
                          ORDER BY NombreCompleto";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Personal
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreCompleto = reader["NombreCompleto"].ToString() ?? "",
                    IdentificacionUnica = reader["IdentificacionUnica"].ToString() ?? "",
                    RolPrimario = reader["RolPrimario"].ToString() ?? "",
                    FechaContratacion = Convert.ToDateTime(reader["FechaContratacion"]),
                    Activo = Convert.ToBoolean(reader["Activo"])
                });
            }

            return lista;
        }

        public Personal? ObtenerPorId(int id)
        {
            using var connection = _dbHelper.GetConnection();
            var query = @"SELECT Id, NombreCompleto, IdentificacionUnica, RolPrimario, FechaContratacion, Activo
                          FROM Personal
                          WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Personal
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreCompleto = reader["NombreCompleto"].ToString() ?? "",
                    IdentificacionUnica = reader["IdentificacionUnica"].ToString() ?? "",
                    RolPrimario = reader["RolPrimario"].ToString() ?? "",
                    FechaContratacion = Convert.ToDateTime(reader["FechaContratacion"]),
                    Activo = Convert.ToBoolean(reader["Activo"])
                };
            }

            return null;
        }

        public bool ExisteIdentificacion(string identificacion, int? excluirId = null)
        {
            using var connection = _dbHelper.GetConnection();

            string query = @"SELECT COUNT(*)
                             FROM Personal
                             WHERE IdentificacionUnica = @IdentificacionUnica";

            if (excluirId.HasValue)
            {
                query += " AND Id <> @Id";
            }

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdentificacionUnica", identificacion);

            if (excluirId.HasValue)
            {
                command.Parameters.AddWithValue("@Id", excluirId.Value);
            }

            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        public void Crear(Personal model)
        {
            using var connection = _dbHelper.GetConnection();
            var query = @"INSERT INTO Personal
                          (NombreCompleto, IdentificacionUnica, RolPrimario, FechaContratacion, Activo)
                          VALUES
                          (@NombreCompleto, @IdentificacionUnica, @RolPrimario, @FechaContratacion, 1)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NombreCompleto", model.NombreCompleto);
            command.Parameters.AddWithValue("@IdentificacionUnica", model.IdentificacionUnica);
            command.Parameters.AddWithValue("@RolPrimario", model.RolPrimario);
            command.Parameters.AddWithValue("@FechaContratacion", model.FechaContratacion);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Editar(Personal model)
        {
            using var connection = _dbHelper.GetConnection();
            var query = @"UPDATE Personal
                          SET NombreCompleto = @NombreCompleto,
                              IdentificacionUnica = @IdentificacionUnica,
                              RolPrimario = @RolPrimario,
                              FechaContratacion = @FechaContratacion,
                              Activo = @Activo
                          WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@NombreCompleto", model.NombreCompleto);
            command.Parameters.AddWithValue("@IdentificacionUnica", model.IdentificacionUnica);
            command.Parameters.AddWithValue("@RolPrimario", model.RolPrimario);
            command.Parameters.AddWithValue("@FechaContratacion", model.FechaContratacion);
            command.Parameters.AddWithValue("@Activo", model.Activo);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void EliminarLogico(int id)
        {
            using var connection = _dbHelper.GetConnection();
            var query = "UPDATE Personal SET Activo = 0 WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public PersonalDetalleDto? ObtenerDetalle(int id)
        {
            var personal = ObtenerPorId(id);
            if (personal == null) return null;

            var detalle = new PersonalDetalleDto
            {
                Id = personal.Id,
                NombreCompleto = personal.NombreCompleto,
                IdentificacionUnica = personal.IdentificacionUnica,
                RolPrimario = personal.RolPrimario,
                FechaContratacion = personal.FechaContratacion,
                Activo = personal.Activo
            };

            using var connection = _dbHelper.GetConnection();
            connection.Open();

            string asignacionActualQuery = @"
                SELECT TOP 1 B.NombreEmbarcacion, A.PuestoAsignado
                FROM AsignacionesTripulacion A
                INNER JOIN Barcos B ON A.BarcoId = B.Id
                WHERE A.PersonalId = @PersonalId AND A.Activa = 1
                ORDER BY A.FechaInicio DESC";

            using (var cmd = new SqlCommand(asignacionActualQuery, connection))
            {
                cmd.Parameters.AddWithValue("@PersonalId", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    detalle.BarcoActual = reader["NombreEmbarcacion"].ToString();
                    detalle.PuestoActual = reader["PuestoAsignado"].ToString();
                }
            }

            string licenciasQuery = @"
                SELECT Id, PersonalId, NombreLicencia, FechaVencimiento
                FROM LicenciasMaritimas
                WHERE PersonalId = @PersonalId
                ORDER BY FechaVencimiento";

            using (var cmd = new SqlCommand(licenciasQuery, connection))
            {
                cmd.Parameters.AddWithValue("@PersonalId", id);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    detalle.Licencias.Add(new LicenciaMaritima
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        PersonalId = Convert.ToInt32(reader["PersonalId"]),
                        NombreLicencia = reader["NombreLicencia"].ToString() ?? "",
                        FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"])
                    });
                }
            }

            string historialQuery = @"
                SELECT A.Id, B.NombreEmbarcacion, A.PuestoAsignado, A.FechaInicio, A.FechaFin, A.Activa
                FROM AsignacionesTripulacion A
                INNER JOIN Barcos B ON A.BarcoId = B.Id
                WHERE A.PersonalId = @PersonalId
                ORDER BY A.FechaInicio DESC";

            using (var cmd = new SqlCommand(historialQuery, connection))
            {
                cmd.Parameters.AddWithValue("@PersonalId", id);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    detalle.HistorialAsignaciones.Add(new AsignacionHistorialItem
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreBarco = reader["NombreEmbarcacion"].ToString() ?? "",
                        PuestoAsignado = reader["PuestoAsignado"].ToString() ?? "",
                        FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                        FechaFin = reader["FechaFin"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaFin"]),
                        Activa = Convert.ToBoolean(reader["Activa"])
                    });
                }
            }

            return detalle;
        }

        public List<BarcoLookupDto> ObtenerBarcosActivos()
        {
            var lista = new List<BarcoLookupDto>();

            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT Id, NombreEmbarcacion
                             FROM Barcos
                             WHERE Archivado = 0
                             ORDER BY NombreEmbarcacion";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new BarcoLookupDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreEmbarcacion = reader["NombreEmbarcacion"].ToString() ?? ""
                });
            }

            return lista;
        }
    }
}