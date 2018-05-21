using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models
{
    public class Log : IModel
    {
        // Todos los set son privados, ya que no se puede permitir modificar un log.
        public int? LogId { get; private set; }
        public DateTime Fecha { get; private set; }
        public CentralMonitoreo Central { get; private set; }
        public TipoLog TipoLog { get; private set; }

        public Log(CentralMonitoreo central, TipoLog tipoLog)
        {
            Central = central;
            TipoLog = tipoLog;
        }

        public static List<Log> GetAll()
        {
            List<Log> logs = new List<Log>();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM central_log";
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["central_log_ID"]);

                        Log log = new Log(CentralMonitoreo.Get(reader["central_ID"].ToString()), TipoLog.Get((ETipoLog)Convert.ToInt32(reader["central_log_tipo_ID"])));
                        log.LogId = id;
                        log.Fecha = Convert.ToDateTime(reader["fecha"]);

                        // Agrego el log a la lista de retorno
                        logs.Add(log);
                    }
                }
            }

            return logs;
        }

        public static Log Get(int id)
        {
            Log log = null;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM central_log WHERE central_log_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        log = new Log(CentralMonitoreo.Get(reader["central_ID"].ToString()), TipoLog.Get((ETipoLog)Convert.ToInt32(reader["central_log_tipo_ID"])));
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
            if (LogId.HasValue || Central == null || TipoLog == null) return;

            // De lo contrario, guardamos.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO central_log (central_ID, central_log_tipo_ID) VALUES (@Central, @TipoLog)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Central", Central.CentralID);
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
