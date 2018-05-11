using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models
{
    public class Barrio
    {
        // Diccionario de Barrios
        public static Dictionary<int, Barrio> Barrios = new Dictionary<int, Barrio>();
        
        public int? Barrio_ID { get; private set; }
        public String Nombre { get; set; }

        public static Barrio get(int id)
        {
            Barrio barrio;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM barrio WHERE barrio_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Si ya está en el Diccionario, obtiene el objeto, de lo contrario, lo crea
                        if (!Barrios.TryGetValue(id, out barrio)) {
                            barrio = new Barrio();
                            Barrios[id] = barrio;
                        }

                        // Actualiza los datos del barrio (sea nuevo o recien creado en memoria)
                        barrio.Barrio_ID = Convert.ToInt32(reader["barrio_ID"]);
                        barrio.Nombre = reader["nombre"].ToString();
                }
                    else return null;
                }
            }

            return barrio;
        }

        public void save()
        {
            // Si el barrio que queremos guardar ya existe en memoria y en DB, retorna.
            if (Barrio_ID != null) return;

            Barrio barrio = get(Barrio_ID);

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO barrios (nombre) VALUES (@Nombre)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Nombre", Nombre);

                connection.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}
