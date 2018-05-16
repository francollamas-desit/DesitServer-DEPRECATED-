using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DesitServer.Models
{
    public class Barrio : IModel
    {
        // Diccionario de Barrios
        private static Dictionary<int, Barrio> Barrios = new Dictionary<int, Barrio>();

        public int? Barrio_ID { get; private set; }
        public String Nombre { get; set; }

        public static List<Barrio> GetAll()
        {
            List<Barrio> barrios = new List<Barrio>();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM barrio";
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();
                
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["barrio_ID"]);

                        // Si existe el barrio en memoria, lo cargo
                        Barrios.TryGetValue(id, out Barrio barrio);

                        // Actualiza los datos del barrio (sea nuevo o recien creado en memoria)
                        barrio.Barrio_ID = id;
                        barrio.Nombre = reader["nombre"].ToString();

                        // Agrego el barrio a la lista de retorno
                        barrios.Add(barrio);

                        // Cabe aclarar que para un getAll() no necesitamos guardarlo en el diccionario si no existía ya...
                    }
                }
            }

            return barrios;
        }

        public static Barrio Get(int id)
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

        public void Save()
        {
            // Si el barrio que queremos guardar ya existe en memoria, no sigue.
            if (Barrio_ID.HasValue) return;

            // De lo contrario, guardamos.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO barrio (nombre) VALUES (@Nombre)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Nombre", Nombre);

                connection.Open();

                cmd.ExecuteNonQuery();
                Barrio_ID = (int)cmd.LastInsertedId;
            }

            // Asignamos el barrio al Diccionario
            Barrios[Barrio_ID.GetValueOrDefault()] = this;
        }

        public void Update()
        {
            // Si no existe todavía, no se puede actualizar.
            if (!Barrio_ID.HasValue) return;

            // De lo contrario, updateamos en la BD.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE barrio SET nombre = @Nombre WHERE barrio_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", Barrio_ID.GetValueOrDefault());
                cmd.Parameters.AddWithValue("@Nombre", Nombre);

                connection.Open();

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete()
        {
            // Si no existe todavía, no se puede borrar.
            if (!Barrio_ID.HasValue) return;

            // De lo contrario, borramos del la BD y el Diccionario.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Db.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "DELETE FROM barrio WHERE barrio_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", Barrio_ID.GetValueOrDefault());

                connection.Open();

                cmd.ExecuteNonQuery();
            }

            Barrios.Remove(Barrio_ID.GetValueOrDefault());
        }
    }
    


}
