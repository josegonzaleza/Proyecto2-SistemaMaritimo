using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class BarcosRepository
    {
        private readonly DbHelper _dbHelper;

        public BarcosRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Barco> ObtenerTodos()
        {
            var lista = new List<Barco>();

            using var connection = _dbHelper.GetConnection();
            string query = @"
                SELECT Id, NombreEmbarcacion, MatriculaUnica, CapacidadCarga,
                       PuertoBase, ModeloMotor, PotenciaMotor, HorasUsoMotor, Archivado
                FROM Barcos
                ORDER BY NombreEmbarcacion";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Barco
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreEmbarcacion = reader["NombreEmbarcacion"].ToString() ?? "",
                    MatriculaUnica = reader["MatriculaUnica"].ToString() ?? "",
                    CapacidadCarga = Convert.ToDecimal(reader["CapacidadCarga"]),
                    PuertoBase = reader["PuertoBase"].ToString() ?? "",
                    ModeloMotor = reader["ModeloMotor"].ToString() ?? "",
                    PotenciaMotor = reader["PotenciaMotor"].ToString() ?? "",
                    HorasUsoMotor = Convert.ToInt32(reader["HorasUsoMotor"]),
                    Archivado = Convert.ToBoolean(reader["Archivado"])
                });
            }

            return lista;
        }

        public Barco? ObtenerPorId(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                SELECT Id, NombreEmbarcacion, MatriculaUnica, CapacidadCarga,
                       PuertoBase, ModeloMotor, PotenciaMotor, HorasUsoMotor, Archivado
                FROM Barcos
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Barco
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreEmbarcacion = reader["NombreEmbarcacion"].ToString() ?? "",
                    MatriculaUnica = reader["MatriculaUnica"].ToString() ?? "",
                    CapacidadCarga = Convert.ToDecimal(reader["CapacidadCarga"]),
                    PuertoBase = reader["PuertoBase"].ToString() ?? "",
                    ModeloMotor = reader["ModeloMotor"].ToString() ?? "",
                    PotenciaMotor = reader["PotenciaMotor"].ToString() ?? "",
                    HorasUsoMotor = Convert.ToInt32(reader["HorasUsoMotor"]),
                    Archivado = Convert.ToBoolean(reader["Archivado"])
                };
            }

            return null;
        }

        public bool ExisteMatricula(string matricula, int? excluirId = null)
        {
            using var connection = _dbHelper.GetConnection();

            string query = @"
                SELECT COUNT(*)
                FROM Barcos
                WHERE MatriculaUnica = @MatriculaUnica";

            if (excluirId.HasValue)
            {
                query += " AND Id <> @Id";
            }

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MatriculaUnica", matricula);

            if (excluirId.HasValue)
            {
                command.Parameters.AddWithValue("@Id", excluirId.Value);
            }

            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        public void Crear(Barco model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                INSERT INTO Barcos
                (NombreEmbarcacion, MatriculaUnica, CapacidadCarga, PuertoBase, ModeloMotor, PotenciaMotor, HorasUsoMotor, Archivado)
                VALUES
                (@NombreEmbarcacion, @MatriculaUnica, @CapacidadCarga, @PuertoBase, @ModeloMotor, @PotenciaMotor, @HorasUsoMotor, 0)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NombreEmbarcacion", model.NombreEmbarcacion);
            command.Parameters.AddWithValue("@MatriculaUnica", model.MatriculaUnica);
            command.Parameters.AddWithValue("@CapacidadCarga", model.CapacidadCarga);
            command.Parameters.AddWithValue("@PuertoBase", model.PuertoBase);
            command.Parameters.AddWithValue("@ModeloMotor", model.ModeloMotor);
            command.Parameters.AddWithValue("@PotenciaMotor", model.PotenciaMotor);
            command.Parameters.AddWithValue("@HorasUsoMotor", model.HorasUsoMotor);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Editar(Barco model)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"
                UPDATE Barcos
                SET NombreEmbarcacion = @NombreEmbarcacion,
                    MatriculaUnica = @MatriculaUnica,
                    CapacidadCarga = @CapacidadCarga,
                    PuertoBase = @PuertoBase,
                    ModeloMotor = @ModeloMotor,
                    PotenciaMotor = @PotenciaMotor,
                    HorasUsoMotor = @HorasUsoMotor,
                    Archivado = @Archivado
                WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@NombreEmbarcacion", model.NombreEmbarcacion);
            command.Parameters.AddWithValue("@MatriculaUnica", model.MatriculaUnica);
            command.Parameters.AddWithValue("@CapacidadCarga", model.CapacidadCarga);
            command.Parameters.AddWithValue("@PuertoBase", model.PuertoBase);
            command.Parameters.AddWithValue("@ModeloMotor", model.ModeloMotor);
            command.Parameters.AddWithValue("@PotenciaMotor", model.PotenciaMotor);
            command.Parameters.AddWithValue("@HorasUsoMotor", model.HorasUsoMotor);
            command.Parameters.AddWithValue("@Archivado", model.Archivado);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Archivar(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = "UPDATE Barcos SET Archivado = 1 WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Activar(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = "UPDATE Barcos SET Archivado = 0 WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}