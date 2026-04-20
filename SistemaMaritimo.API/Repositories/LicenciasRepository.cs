using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class LicenciasRepository
    {
        private readonly DbHelper _dbHelper;

        public LicenciasRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<LicenciaMaritima> ObtenerPorPersonal(int personalId)
        {
            var lista = new List<LicenciaMaritima>();

            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT Id, PersonalId, NombreLicencia, FechaVencimiento
                             FROM LicenciasMaritimas
                             WHERE PersonalId = @PersonalId
                             ORDER BY FechaVencimiento";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonalId", personalId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new LicenciaMaritima
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    PersonalId = Convert.ToInt32(reader["PersonalId"]),
                    NombreLicencia = reader["NombreLicencia"].ToString() ?? "",
                    FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"])
                });
            }

            return lista;
        }

        public void Crear(LicenciaMaritima model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"INSERT INTO LicenciasMaritimas
                             (PersonalId, NombreLicencia, FechaVencimiento)
                             VALUES
                             (@PersonalId, @NombreLicencia, @FechaVencimiento)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonalId", model.PersonalId);
            command.Parameters.AddWithValue("@NombreLicencia", model.NombreLicencia);
            command.Parameters.AddWithValue("@FechaVencimiento", model.FechaVencimiento);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = "DELETE FROM LicenciasMaritimas WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}