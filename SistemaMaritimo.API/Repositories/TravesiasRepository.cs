using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class TravesiasRepository
    {
        private readonly DbHelper _dbHelper;

        public TravesiasRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<TravesiaListItem> ObtenerTodas(string? barcoId = null, string? puertoDestino = null)
        {
            var lista = new List<TravesiaListItem>();

            using var connection = _dbHelper.GetConnection();

            string query = @"
                SELECT T.Id, T.BarcoId, B.NombreEmbarcacion AS NombreBarco,
                       T.PuertoOrigen, T.PuertoDestino,
                       T.FechaPrevistaSalida, T.FechaPrevistaLlegada,
                       T.Estado, T.FechaCierreReal, T.UsuarioCierre
                FROM Travesias T
                INNER JOIN Barcos B ON T.BarcoId = B.Id
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(barcoId))
                query += " AND T.BarcoId = @BarcoId";

            if (!string.IsNullOrWhiteSpace(puertoDestino))
                query += " AND T.PuertoDestino LIKE @PuertoDestino";

            query += " ORDER BY T.FechaPrevistaSalida DESC";

            using var command = new SqlCommand(query, connection);

            if (!string.IsNullOrWhiteSpace(barcoId))
                command.Parameters.AddWithValue("@BarcoId", Convert.ToInt32(barcoId));

            if (!string.IsNullOrWhiteSpace(puertoDestino))
                command.Parameters.AddWithValue("@PuertoDestino", $"%{puertoDestino}%");

            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TravesiaListItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    NombreBarco = reader["NombreBarco"].ToString() ?? "",
                    PuertoOrigen = reader["PuertoOrigen"].ToString() ?? "",
                    PuertoDestino = reader["PuertoDestino"].ToString() ?? "",
                    FechaPrevistaSalida = Convert.ToDateTime(reader["FechaPrevistaSalida"]),
                    FechaPrevistaLlegada = Convert.ToDateTime(reader["FechaPrevistaLlegada"]),
                    Estado = reader["Estado"].ToString() ?? "",
                    FechaCierreReal = reader["FechaCierreReal"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaCierreReal"]),
                    UsuarioCierre = reader["UsuarioCierre"] == DBNull.Value ? null : reader["UsuarioCierre"].ToString()
                });
            }

            return lista;
        }

        public Travesia? ObtenerPorId(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                SELECT Id, BarcoId, PuertoOrigen, PuertoDestino,
                       FechaPrevistaSalida, FechaPrevistaLlegada,
                       Estado, FechaCierreReal, UsuarioCierre
                FROM Travesias
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Travesia
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    PuertoOrigen = reader["PuertoOrigen"].ToString() ?? "",
                    PuertoDestino = reader["PuertoDestino"].ToString() ?? "",
                    FechaPrevistaSalida = Convert.ToDateTime(reader["FechaPrevistaSalida"]),
                    FechaPrevistaLlegada = Convert.ToDateTime(reader["FechaPrevistaLlegada"]),
                    Estado = reader["Estado"].ToString() ?? "",
                    FechaCierreReal = reader["FechaCierreReal"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaCierreReal"]),
                    UsuarioCierre = reader["UsuarioCierre"] == DBNull.Value ? null : reader["UsuarioCierre"].ToString()
                };
            }

            return null;
        }

        public void Crear(Travesia model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                INSERT INTO Travesias
                (BarcoId, PuertoOrigen, PuertoDestino, FechaPrevistaSalida, FechaPrevistaLlegada, Estado)
                VALUES
                (@BarcoId, @PuertoOrigen, @PuertoDestino, @FechaPrevistaSalida, @FechaPrevistaLlegada, @Estado)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BarcoId", model.BarcoId);
            command.Parameters.AddWithValue("@PuertoOrigen", model.PuertoOrigen);
            command.Parameters.AddWithValue("@PuertoDestino", model.PuertoDestino);
            command.Parameters.AddWithValue("@FechaPrevistaSalida", model.FechaPrevistaSalida);
            command.Parameters.AddWithValue("@FechaPrevistaLlegada", model.FechaPrevistaLlegada);
            command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(model.Estado) ? "Planeada" : model.Estado);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Editar(Travesia model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                UPDATE Travesias
                SET BarcoId = @BarcoId,
                    PuertoOrigen = @PuertoOrigen,
                    PuertoDestino = @PuertoDestino,
                    FechaPrevistaSalida = @FechaPrevistaSalida,
                    FechaPrevistaLlegada = @FechaPrevistaLlegada
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@BarcoId", model.BarcoId);
            command.Parameters.AddWithValue("@PuertoOrigen", model.PuertoOrigen);
            command.Parameters.AddWithValue("@PuertoDestino", model.PuertoDestino);
            command.Parameters.AddWithValue("@FechaPrevistaSalida", model.FechaPrevistaSalida);
            command.Parameters.AddWithValue("@FechaPrevistaLlegada", model.FechaPrevistaLlegada);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void CambiarEstado(int travesiaId, string nuevoEstado, string usuario)
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            var travesia = ObtenerPorIdInterno(travesiaId, connection);
            if (travesia == null)
                throw new Exception("La travesía no existe.");

            if (nuevoEstado == "En Curso")
            {
                if (!CumpleTripulacionMinima(travesia.BarcoId, connection))
                    throw new Exception("El barco no cuenta con la tripulación mínima requerida para iniciar la travesía.");
            }

            if (nuevoEstado == "Completada")
            {
                string queryCompleta = @"
                    UPDATE Travesias
                    SET Estado = @Estado,
                        FechaCierreReal = GETDATE(),
                        UsuarioCierre = @UsuarioCierre
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(queryCompleta, connection);
                cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@UsuarioCierre", usuario);
                cmd.Parameters.AddWithValue("@Id", travesiaId);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string query = @"
                    UPDATE Travesias
                    SET Estado = @Estado
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@Id", travesiaId);
                cmd.ExecuteNonQuery();
            }
        }

        private Travesia? ObtenerPorIdInterno(int id, SqlConnection connection)
        {
            string query = @"
                SELECT Id, BarcoId, PuertoOrigen, PuertoDestino,
                       FechaPrevistaSalida, FechaPrevistaLlegada,
                       Estado, FechaCierreReal, UsuarioCierre
                FROM Travesias
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var item = new Travesia
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    BarcoId = Convert.ToInt32(reader["BarcoId"]),
                    PuertoOrigen = reader["PuertoOrigen"].ToString() ?? "",
                    PuertoDestino = reader["PuertoDestino"].ToString() ?? "",
                    FechaPrevistaSalida = Convert.ToDateTime(reader["FechaPrevistaSalida"]),
                    FechaPrevistaLlegada = Convert.ToDateTime(reader["FechaPrevistaLlegada"]),
                    Estado = reader["Estado"].ToString() ?? ""
                };
                return item;
            }

            return null;
        }

        private bool CumpleTripulacionMinima(int barcoId, SqlConnection connection)
        {
            string query = @"
                SELECT
                    SUM(CASE WHEN P.RolPrimario = 'Capitan' THEN 1 ELSE 0 END) AS Capitanes,
                    SUM(CASE WHEN P.RolPrimario = 'PrimerOficial' THEN 1 ELSE 0 END) AS Oficiales,
                    SUM(CASE WHEN P.RolPrimario = 'Ingeniero' THEN 1 ELSE 0 END) AS Ingenieros,
                    SUM(CASE WHEN P.RolPrimario = 'Marinero' THEN 1 ELSE 0 END) AS Marineros
                FROM AsignacionesTripulacion A
                INNER JOIN Personal P ON A.PersonalId = P.Id
                WHERE A.BarcoId = @BarcoId
                  AND A.Activa = 1
                  AND P.Activo = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BarcoId", barcoId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                int capitanes = reader["Capitanes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Capitanes"]);
                int oficiales = reader["Oficiales"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Oficiales"]);
                int ingenieros = reader["Ingenieros"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Ingenieros"]);
                int marineros = reader["Marineros"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Marineros"]);

                return capitanes >= 1 &&
                       oficiales >= 1 &&
                       ingenieros >= 2 &&
                       marineros >= 5;
            }

            return false;
        }
    }
}