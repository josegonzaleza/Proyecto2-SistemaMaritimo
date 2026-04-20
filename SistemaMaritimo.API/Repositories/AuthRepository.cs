using Microsoft.Data.SqlClient;
using SistemaMaritimo.API.Data;
using SistemaMaritimo.API.Helpers;
using SistemaMaritimo.API.Models;

namespace SistemaMaritimo.API.Repositories
{
    public class AuthRepository
    {
        private readonly DbHelper _dbHelper;
        private readonly IConfiguration _configuration;

        public AuthRepository(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _configuration = configuration;
        }

        public void EditarUsuario(int id, string nombreUsuario, int rolId, bool activo, string realizadoPor)
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string updateUsuario = @"UPDATE Usuarios
                                 SET NombreUsuario = @NombreUsuario,
                                     Activo = @Activo
                                 WHERE Id = @Id";

                using var cmdUsuario = new SqlCommand(updateUsuario, connection, transaction);
                cmdUsuario.Parameters.AddWithValue("@Id", id);
                cmdUsuario.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                cmdUsuario.Parameters.AddWithValue("@Activo", activo);
                cmdUsuario.ExecuteNonQuery();

                using var deleteRoles = new SqlCommand("DELETE FROM UsuarioRoles WHERE UsuarioId = @UsuarioId", connection, transaction);
                deleteRoles.Parameters.AddWithValue("@UsuarioId", id);
                deleteRoles.ExecuteNonQuery();

                using var insertRol = new SqlCommand("INSERT INTO UsuarioRoles (UsuarioId, RolId) VALUES (@UsuarioId, @RolId)", connection, transaction);
                insertRol.Parameters.AddWithValue("@UsuarioId", id);
                insertRol.Parameters.AddWithValue("@RolId", rolId);
                insertRol.ExecuteNonQuery();

                using var bitacoraCmd = new SqlCommand(@"
            INSERT INTO BitacoraCambiosRoles (UsuarioAfectadoId, RolId, Accion, RealizadoPor)
            VALUES (@UsuarioAfectadoId, @RolId, 'Edición de usuario/rol', @RealizadoPor)", connection, transaction);

                bitacoraCmd.Parameters.AddWithValue("@UsuarioAfectadoId", id);
                bitacoraCmd.Parameters.AddWithValue("@RolId", rolId);
                bitacoraCmd.Parameters.AddWithValue("@RealizadoPor", realizadoPor);
                bitacoraCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public int ObtenerRolIdActualUsuario(int usuarioId)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT TOP 1 RolId
                     FROM UsuarioRoles
                     WHERE UsuarioId = @UsuarioId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            connection.Open();
            var result = command.ExecuteScalar();

            return result == null ? 0 : Convert.ToInt32(result);
        }

        public Usuario? ObtenerUsuarioPorId(int id)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT Id, NombreUsuario, ClaveHash, Activo, IntentosFallidos, BloqueadoHasta
                     FROM Usuarios
                     WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Usuario
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreUsuario = reader["NombreUsuario"].ToString() ?? "",
                    ClaveHash = reader["ClaveHash"].ToString() ?? "",
                    Activo = Convert.ToBoolean(reader["Activo"]),
                    IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                    BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["BloqueadoHasta"])
                };
            }

            return null;
        }

        public void CambiarPassword(int id, string nuevaClaveHash)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"UPDATE Usuarios
                     SET ClaveHash = @ClaveHash,
                         IntentosFallidos = 0,
                         BloqueadoHasta = NULL
                     WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ClaveHash", nuevaClaveHash);

            connection.Open();
            command.ExecuteNonQuery();
        }
        public Usuario? ObtenerUsuarioPorNombre(string nombreUsuario)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT Id, NombreUsuario, ClaveHash, Activo, IntentosFallidos, BloqueadoHasta
                             FROM Usuarios
                             WHERE NombreUsuario = @NombreUsuario";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Usuario
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreUsuario = reader["NombreUsuario"].ToString() ?? "",
                    ClaveHash = reader["ClaveHash"].ToString() ?? "",
                    Activo = Convert.ToBoolean(reader["Activo"]),
                    IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                    BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["BloqueadoHasta"])
                };
            }

            return null;
        }

        public List<UsuarioListItem> ObtenerUsuarios()
        {
            var lista = new List<UsuarioListItem>();

            using var connection = _dbHelper.GetConnection();
            string query = @"
        SELECT U.Id, U.NombreUsuario, U.Activo,
               STRING_AGG(R.Nombre, ', ') AS Roles
        FROM Usuarios U
        LEFT JOIN UsuarioRoles UR ON U.Id = UR.UsuarioId
        LEFT JOIN Roles R ON UR.RolId = R.Id
        GROUP BY U.Id, U.NombreUsuario, U.Activo
        ORDER BY U.NombreUsuario";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new UsuarioListItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NombreUsuario = reader["NombreUsuario"].ToString() ?? "",
                    Activo = Convert.ToBoolean(reader["Activo"]),
                    Roles = reader["Roles"] == DBNull.Value ? "" : reader["Roles"].ToString()!
                });
            }

            return lista;
        }

        public List<Rol> ObtenerRoles()
        {
            var lista = new List<Rol>();

            using var connection = _dbHelper.GetConnection();
            string query = "SELECT Id, Nombre FROM Roles ORDER BY Nombre";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Rol
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nombre = reader["Nombre"].ToString() ?? ""
                });
            }

            return lista;
        }

        public void EliminarUsuario(int id)
        {
            using var connection = _dbHelper.GetConnection();

            string query = @"
        UPDATE Usuarios
        SET Activo = 0
        WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void ActivarUsuario(int id)
        {
            using var connection = _dbHelper.GetConnection();

            string query = "UPDATE Usuarios SET Activo = 1 WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void CambiarRolUsuario(int usuarioId, int rolId, string realizadoPor)
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using var deleteCmd = new SqlCommand("DELETE FROM UsuarioRoles WHERE UsuarioId = @UsuarioId", connection, transaction);
                deleteCmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                deleteCmd.ExecuteNonQuery();

                using var insertCmd = new SqlCommand("INSERT INTO UsuarioRoles (UsuarioId, RolId) VALUES (@UsuarioId, @RolId)", connection, transaction);
                insertCmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                insertCmd.Parameters.AddWithValue("@RolId", rolId);
                insertCmd.ExecuteNonQuery();

                using var bitacoraCmd = new SqlCommand(@"
            INSERT INTO BitacoraCambiosRoles (UsuarioAfectadoId, RolId, Accion, RealizadoPor)
            VALUES (@UsuarioAfectadoId, @RolId, 'Cambio de rol', @RealizadoPor)", connection, transaction);

                bitacoraCmd.Parameters.AddWithValue("@UsuarioAfectadoId", usuarioId);
                bitacoraCmd.Parameters.AddWithValue("@RolId", rolId);
                bitacoraCmd.Parameters.AddWithValue("@RealizadoPor", realizadoPor);
                bitacoraCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void CrearRol(string nombre)
        {
            using var connection = _dbHelper.GetConnection();
            string query = "INSERT INTO Roles (Nombre) VALUES (@Nombre)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nombre", nombre);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void CrearUsuario(string nombreUsuario, string claveHash, int rolId)
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Insertar usuario
                string queryUsuario = @"
            INSERT INTO Usuarios (NombreUsuario, ClaveHash, Activo, IntentosFallidos)
            VALUES (@NombreUsuario, @ClaveHash, 1, 0);
            SELECT SCOPE_IDENTITY();
        ";

                using var cmdUsuario = new SqlCommand(queryUsuario, connection, transaction);
                cmdUsuario.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                cmdUsuario.Parameters.AddWithValue("@ClaveHash", claveHash);

                int usuarioId = Convert.ToInt32(cmdUsuario.ExecuteScalar());

                // 2. Asignar rol
                string queryRol = @"
            INSERT INTO UsuarioRoles (UsuarioId, RolId)
            VALUES (@UsuarioId, @RolId);
        ";

                using var cmdRol = new SqlCommand(queryRol, connection, transaction);
                cmdRol.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmdRol.Parameters.AddWithValue("@RolId", rolId);

                cmdRol.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<string> ObtenerRolesUsuario(int usuarioId)
        {
            var roles = new List<string>();

            using var connection = _dbHelper.GetConnection();
            string query = @"SELECT R.Nombre
                             FROM UsuarioRoles UR
                             INNER JOIN Roles R ON UR.RolId = R.Id
                             WHERE UR.UsuarioId = @UsuarioId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(reader["Nombre"].ToString() ?? "");
            }

            return roles;
        }

        public List<BitacoraCambioRolItem> ObtenerBitacoraCambiosRoles()
        {
            var lista = new List<BitacoraCambioRolItem>();

            using var connection = _dbHelper.GetConnection();
            string query = @"
        SELECT B.Id,
               U.NombreUsuario AS UsuarioAfectado,
               R.Nombre AS Rol,
               B.Accion,
               B.Fecha,
               B.RealizadoPor
        FROM BitacoraCambiosRoles B
        INNER JOIN Usuarios U ON B.UsuarioAfectadoId = U.Id
        INNER JOIN Roles R ON B.RolId = R.Id
        ORDER BY B.Fecha DESC";

            using var command = new SqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new BitacoraCambioRolItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    UsuarioAfectado = reader["UsuarioAfectado"].ToString() ?? "",
                    Rol = reader["Rol"].ToString() ?? "",
                    Accion = reader["Accion"].ToString() ?? "",
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    RealizadoPor = reader["RealizadoPor"].ToString() ?? ""
                });
            }

            return lista;
        }
        public void ActualizarIntentosFallidos(int usuarioId, int intentos, DateTime? bloqueadoHasta)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"UPDATE Usuarios
                             SET IntentosFallidos = @IntentosFallidos,
                                 BloqueadoHasta = @BloqueadoHasta
                             WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IntentosFallidos", intentos);
            command.Parameters.AddWithValue("@BloqueadoHasta", (object?)bloqueadoHasta ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", usuarioId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void ResetearIntentos(int usuarioId)
        {
            using var connection = _dbHelper.GetConnection();
            string query = @"UPDATE Usuarios
                             SET IntentosFallidos = 0,
                                 BloqueadoHasta = NULL
                             WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", usuarioId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}