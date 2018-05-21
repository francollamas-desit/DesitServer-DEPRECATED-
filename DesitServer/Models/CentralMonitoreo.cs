using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DesitServer.Models
{
    public class CentralMonitoreo : IModel
    {
        // Diccionario de Centrales de Monitoreo
        private static Dictionary<string, CentralMonitoreo> Centrales = new Dictionary<string, CentralMonitoreo>();

        public String CentralID { get; set; }
        public String Contraseña { get; set; }
        public Barrio Barrio { get; set; }

        public static List<CentralMonitoreo> GetAll()
        {
            List<CentralMonitoreo> centrales = new List<CentralMonitoreo>();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM central";
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String id = reader["central_ID"].ToString();

                        // Si existe la central en memoria, lo cargo
                        Centrales.TryGetValue(id, out CentralMonitoreo central);

                        // Actualiza los datos de la central (sea nueva o recien creada en memoria)
                        central.CentralID = id;
                        central.Contraseña = reader["contrasenia"].ToString();
                        central.Barrio = Barrio.Get(Convert.ToInt32(reader["barrio_ID"]));

                        // Agrego el barrio a la lista de retorno
                        centrales.Add(central);

                        // Cabe aclarar que para un getAll() no necesitamos guardarlo en el diccionario si no existía ya...
                    }
                }
            }

            return centrales;
        }

        public static CentralMonitoreo Get(String id)
        {
            CentralMonitoreo central;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM central WHERE central_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Si ya está en el Diccionario, obtiene el objeto, de lo contrario, lo crea
                        if (!Centrales.TryGetValue(id, out central))
                        {
                            central = new CentralMonitoreo();
                            Centrales[id] = central;
                        }

                        // Actualiza los datos de la central (sea nuevo o recien creado en memoria)
                        central.CentralID = id;
                        central.Contraseña = reader["contrasenia"].ToString();
                        central.Barrio = Barrio.Get(Convert.ToInt32(reader["barrio_ID"]));
                    }
                    else return null;
                }
            }

            return central;
        }

        public void Save()
        {
            if (Barrio == null) return;

            // Guardamos
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO central (central_ID, contrasenia, barrio_ID) VALUES (@Id, @Contrasenia, @Barrio)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", CentralID);
                cmd.Parameters.AddWithValue("@Contrasenia", Contraseña);
                cmd.Parameters.AddWithValue("@Barrio", Barrio.BarrioID);

                connection.Open();
                cmd.ExecuteNonQuery();
            }

            // Asignamos la central al Diccionario
            Centrales[CentralID] = this;
        }

        public void Update()
        {
            // Updateamos en la BD.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE central SET contrasenia = @Contrasenia, barrio_ID = @Barrio WHERE central_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", CentralID);
                cmd.Parameters.AddWithValue("@Contrasenia", Contraseña);
                cmd.Parameters.AddWithValue("@Barrio", Barrio.BarrioID);

                connection.Open();

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete()
        {
        
            // De lo contrario, borramos del la BD y el Diccionario.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "DELETE FROM central WHERE central_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", CentralID);

                connection.Open();

                cmd.ExecuteNonQuery();
            }

            Centrales.Remove(CentralID);
        }
    }
}
