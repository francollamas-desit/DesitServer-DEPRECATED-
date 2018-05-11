using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DesitServer.Models
{
    public class CentralMonitoreo
    {
        public String Central_ID { get; set; }
        public String Contraseña { get; set; }

        public static CentralMonitoreo getCentralMonitoreo(String id)
        {
            CentralMonitoreo central = new CentralMonitoreo();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM centrales WHERE central_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        central.Central_ID = reader["central_ID"].ToString();
                        central.Contraseña = reader["contraseña"].ToString();

                    }
                    else return null;
                }
            }
       
            return central;
        }
       

    }
}
