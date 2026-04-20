using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class DashboardRepository
    {
        private readonly DbHelper _dbHelper;

        public DashboardRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public DashboardDto ObtenerDashboard()
        {
            var dashboard = new DashboardDto();

            using var connection = _dbHelper.GetConnection();
            connection.Open();

            dashboard.Resumen = ObtenerResumen(connection);
            dashboard.LicenciasAlertas = ObtenerLicenciasAlertas(connection);
            dashboard.OrdenesPendientes = ObtenerOrdenesPendientes(connection);
            dashboard.TravesiasActivas = ObtenerTravesiasActivas(connection);

            return dashboard;
        }

        private DashboardResumenDto ObtenerResumen(SqlConnection connection)
        {
            var resumen = new DashboardResumenDto();

            string query = @"
                SELECT
                    (SELECT COUNT(*) FROM Barcos WHERE Archivado = 0) AS BarcosActivos,
                    (SELECT COUNT(*) FROM Barcos WHERE Archivado = 1) AS BarcosArchivados,
                    (SELECT COUNT(*) FROM Travesias WHERE Estado = 'Planeada') AS TravesiasPlaneadas,
                    (SELECT COUNT(*) FROM Travesias WHERE Estado = 'En Curso') AS TravesiasEnCurso,
                    (SELECT COUNT(*) FROM OrdenesServicio WHERE Estado = 'Abierta') AS OrdenesAbiertas,
                    (SELECT COUNT(*) FROM OrdenesServicio WHERE Estado = 'En Progreso') AS OrdenesEnProgreso,
                    (SELECT COUNT(*) FROM LicenciasMaritimas WHERE FechaVencimiento BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 60, CAST(GETDATE() AS DATE))) AS LicenciasPorVencer,
                    (SELECT COUNT(*) FROM LicenciasMaritimas WHERE FechaVencimiento < CAST(GETDATE() AS DATE)) AS LicenciasVencidas";

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                resumen.BarcosActivos = Convert.ToInt32(reader["BarcosActivos"]);
                resumen.BarcosArchivados = Convert.ToInt32(reader["BarcosArchivados"]);
                resumen.TravesiasPlaneadas = Convert.ToInt32(reader["TravesiasPlaneadas"]);
                resumen.TravesiasEnCurso = Convert.ToInt32(reader["TravesiasEnCurso"]);
                resumen.OrdenesAbiertas = Convert.ToInt32(reader["OrdenesAbiertas"]);
                resumen.OrdenesEnProgreso = Convert.ToInt32(reader["OrdenesEnProgreso"]);
                resumen.LicenciasPorVencer = Convert.ToInt32(reader["LicenciasPorVencer"]);
                resumen.LicenciasVencidas = Convert.ToInt32(reader["LicenciasVencidas"]);
            }

            return resumen;
        }

        private List<LicenciaAlertaDto> ObtenerLicenciasAlertas(SqlConnection connection)
        {
            var lista = new List<LicenciaAlertaDto>();

            string query = @"
                SELECT TOP 10
                    P.Id AS PersonalId,
                    P.NombreCompleto,
                    L.NombreLicencia,
                    L.FechaVencimiento
                FROM LicenciasMaritimas L
                INNER JOIN Personal P ON L.PersonalId = P.Id
                WHERE L.FechaVencimiento <= DATEADD(DAY, 60, CAST(GETDATE() AS DATE))
                ORDER BY L.FechaVencimiento ASC";

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var fecha = Convert.ToDateTime(reader["FechaVencimiento"]);
                var estado = fecha.Date < DateTime.Today ? "Vencida" : "Por vencer";

                lista.Add(new LicenciaAlertaDto
                {
                    PersonalId = Convert.ToInt32(reader["PersonalId"]),
                    NombreCompleto = reader["NombreCompleto"].ToString() ?? "",
                    NombreLicencia = reader["NombreLicencia"].ToString() ?? "",
                    FechaVencimiento = fecha,
                    Estado = estado
                });
            }

            return lista;
        }

        private List<DashboardOrdenItemDto> ObtenerOrdenesPendientes(SqlConnection connection)
        {
            var lista = new List<DashboardOrdenItemDto>();

            string query = @"
                SELECT TOP 10
                    O.Id,
                    O.CodigoOrden,
                    B.NombreEmbarcacion AS NombreBarco,
                    O.Prioridad,
                    O.Estado,
                    O.FechaLimite
                FROM OrdenesServicio O
                INNER JOIN Barcos B ON O.BarcoId = B.Id
                WHERE O.Estado IN ('Abierta', 'Asignada', 'En Progreso', 'Pendiente de Aprobación')
                ORDER BY O.FechaLimite ASC";

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new DashboardOrdenItemDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CodigoOrden = reader["CodigoOrden"].ToString() ?? "",
                    NombreBarco = reader["NombreBarco"].ToString() ?? "",
                    Prioridad = reader["Prioridad"].ToString() ?? "",
                    Estado = reader["Estado"].ToString() ?? "",
                    FechaLimite = Convert.ToDateTime(reader["FechaLimite"])
                });
            }

            return lista;
        }

        private List<DashboardTravesiaItemDto> ObtenerTravesiasActivas(SqlConnection connection)
        {
            var lista = new List<DashboardTravesiaItemDto>();

            string query = @"
                SELECT TOP 10
                    T.Id,
                    B.NombreEmbarcacion AS NombreBarco,
                    T.PuertoOrigen,
                    T.PuertoDestino,
                    T.Estado,
                    T.FechaPrevistaSalida
                FROM Travesias T
                INNER JOIN Barcos B ON T.BarcoId = B.Id
                WHERE T.Estado IN ('Planeada', 'En Curso')
                ORDER BY T.FechaPrevistaSalida ASC";

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new DashboardTravesiaItemDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreBarco = reader["NombreBarco"].ToString() ?? "",
                    PuertoOrigen = reader["PuertoOrigen"].ToString() ?? "",
                    PuertoDestino = reader["PuertoDestino"].ToString() ?? "",
                    Estado = reader["Estado"].ToString() ?? "",
                    FechaPrevistaSalida = Convert.ToDateTime(reader["FechaPrevistaSalida"])
                });
            }

            return lista;
        }
    }
}