using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models.InterrogacionSecuencial
{
    public class IntSecuencial
    {
        public long Intervalo { get; set; }
        public long TimeOut { get; set; }
        public int Reintentos { get; set; }
        
        public static IntSecuencial Get()
        {
            IntSecuencial intSecuencial;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM int_secuencial";
                cmd.CommandType = System.Data.CommandType.Text;

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        intSecuencial = new IntSecuencial();
                        
                        intSecuencial.Intervalo = Convert.ToInt32(reader["intervalo"]);
                        intSecuencial.TimeOut = Convert.ToInt32(reader["timeout"]);
                        intSecuencial.Reintentos = Convert.ToInt32(reader["reintentos"]);
                    }
                    else return null;
                }
            }

            return intSecuencial;
        }
        
        public void Update()
        {
            // De lo contrario, updateamos en la BD.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE int_secuencial SET intervalo = @Intervalo, timeout = @TimeOut, reintentos = @Reintentos";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Intervalo", Intervalo);
                cmd.Parameters.AddWithValue("@TimeOut", TimeOut);
                cmd.Parameters.AddWithValue("@Reintentos", Reintentos);

                connection.Open();

                cmd.ExecuteNonQuery();
            }
        }
        
    }
}