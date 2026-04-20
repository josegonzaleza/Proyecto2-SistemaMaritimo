using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class AsignacionesTripulacionRepository
    {
        private readonly DbHelper _dbHelper;

        public AsignacionesTripulacionRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<AsignacionTripulacion> ObtenerPorPersonal(int personalId)
        {
            var lista = new List<AsignacionTripulacion>();

            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT Id, PersonalId, BarcoId, PuestoAsignado, FechaInicio, FechaFin, Activa
                             FROM AsignacionesTripulacion
                             WHERE PersonalId = @PersonalId
                             ORDER BY FechaInicio DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonalId", personalId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AsignacionTripulacion
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    PersonalId = Convert.ToInt32(reader["PersonalId"]),
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    PuestoAsignado = reader["PuestoAsignado"].ToString() ?? "",
                    FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                    FechaFin = reader["FechaFin"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaFin"]),
                    Activa = Convert.ToBoolean(reader["Activa"])
                });
            }

            return lista;
        }

        public void Crear(AsignacionTripulacion model)
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string cerrarActivas = @"
                    UPDATE AsignacionesTripulacion
                    SET Activa = 0,
                        FechaFin = CASE WHEN FechaFin IS NULL THEN GETDATE() ELSE FechaFin END
                    WHERE PersonalId = @PersonalId
                      AND Activa = 1";

                using (var cmd = new SqlCommand(cerrarActivas, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@PersonalId", model.PersonalId);
                    cmd.ExecuteNonQuery();
                }

                string insertar = @"
                    INSERT INTO AsignacionesTripulacion
                    (PersonalId, BarcoId, PuestoAsignado, FechaInicio, FechaFin, Activa)
                    VALUES
                    (@PersonalId, @BarcoId, @PuestoAsignado, @FechaInicio, @FechaFin, 1)";

                using (var cmd = new SqlCommand(insertar, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@PersonalId", model.PersonalId);
                    cmd.Parameters.AddWithValue("@BarcoId", model.BarcoId);
                    cmd.Parameters.AddWithValue("@PuestoAsignado", model.PuestoAsignado);
                    cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", (object?)model.FechaFin ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}