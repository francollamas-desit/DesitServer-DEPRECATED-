using DesitServer.Models.Administrador;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models.Administrador.Log
{
    public class AdminLog : IModel
    {
        // Todos los set son privados, ya que no se puede permitir modificar un log.
        public int? LogId { get; private set; }
        public DateTime Fecha { get; private set; }
        public Admin Admin { get; private set; }
        public AdminLogTipo TipoLog { get; private set; }

        public AdminLog(Admin admin, AdminLogTipo tipoLog)
        {
            Admin = admin;
            TipoLog = tipoLog;
        }

        public static List<AdminLog> GetAll(string nombreAdmin)
        {
            List<AdminLog> logs = new List<AdminLog>();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM admin_log WHERE admin_nombre = @NombreAdmin";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@NombreAdmin", nombreAdmin);
                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["admin_log_ID"]);

                        AdminLog log = new AdminLog(Admin.Get(reader["admin_nombre"].ToString()), AdminLogTipo.Get((EAdminLogTipo)Convert.ToInt32(reader["admin_log_tipo_ID"])));
                        log.LogId = id;
                        log.Fecha = Convert.ToDateTime(reader["fecha"]);

                        // Agrego el log a la lista de retorno
                        logs.Add(log);
                    }
                }
            }

            return logs;
        }

        public static AdminLog Get(int id)
        {
            AdminLog log = null;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM admin_log WHERE admin_log_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        log = new AdminLog(Admin.Get(reader["admin_nombre"].ToString()), AdminLogTipo.Get((EAdminLogTipo)Convert.ToInt32(reader["admin_log_tipo_ID"])));
                        log.LogId = id;
                        log.Fecha = Convert.ToDateTime(reader["fecha"]);
                    }
                    else return null;
                }
            }

            return log;
        }

        public void Save()
        {
            // Si el log ya existe en memoria, o no se setearon los datos correspondientes, no lo guarda
            if (LogId.HasValue || Admin == null || TipoLog == null) return;

            // De lo contrario, guardamos.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO admin_log (admin_nombre, admin_log_tipo_ID) VALUES (@NombreAdmin, @TipoLog)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@NombreAdmin", Admin.Nombre);
                cmd.Parameters.AddWithValue("@TipoLog", TipoLog.TipoLogId);

                connection.Open();

                cmd.ExecuteNonQuery();
                LogId = (int)cmd.LastInsertedId;
            }
        }

        // Estos métodos no se usan ya que es una clase inmutable.
        public void Update() { }

        public void Delete() { }
    }
}
