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
            

            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=desitserver;user=root;password=251436;sslmode=none");

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM centrales WHERE central_ID ='" + id + "'";
            cmd.CommandType = System.Data.CommandType.Text;

            connection.Open();

            String idd = "";
            String contraseña = "";

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                idd = reader["central_ID"].ToString();
                contraseña = reader["contraseña"].ToString();
            }

            reader.Close();
            connection.Close();

            return new CentralMonitoreo() { Central_ID = idd, Contraseña = contraseña };
        }
       

    }
}
