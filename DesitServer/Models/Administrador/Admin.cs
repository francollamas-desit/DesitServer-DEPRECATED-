using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models.Administrador
{
    public class Admin : IModel
    {
        // Diccionario de Admins
        private static Dictionary<string, Admin> Admins = new Dictionary<string, Admin>();

        public String Nombre { get; set; }
        public String Contraseña { get; set; }

        public static List<Admin> GetAll()
        {
            List<Admin> admins = new List<Admin>();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM admin";
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String nombre = reader["admin_nombre"].ToString();

                        // Si existe el admin en memoria, lo cargo
                        Admins.TryGetValue(nombre, out Admin admin);

                        // Actualiza los datos del admin (sea nueva o recien creada en memoria)
                        admin.Nombre = nombre;
                        admin.Contraseña = reader["contrasenia"].ToString();

                        // Agrego el admin a la lista de retorno
                        admins.Add(admin);

                        // Cabe aclarar que para un getAll() no necesitamos guardarlo en el diccionario si no existía ya...
                    }
                }
            }

            return admins;
        }

        public static Admin Get(String nombre)
        {
            Admin admin;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM admin WHERE admin_nombre = @NombreAdmin";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@NombreAdmin", nombre);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Si ya está en el Diccionario, obtiene el objeto, de lo contrario, lo crea
                        if (!Admins.TryGetValue(nombre, out admin))
                        {
                            admin = new Admin();
                            Admins[nombre] = admin;
                        }

                        // Actualiza los datos del admin (sea nuevo o recien creado en memoria)
                        admin.Nombre = nombre;
                        admin.Contraseña = reader["contrasenia"].ToString();
                    }
                    else return null;
                }
            }

            return admin;
        }

        public void Save()
        {

            // Guardamos
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO admin (admin_nombre, contrasenia) VALUES (@NombreAdmin, @Contrasenia)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@NombreAdmin", Nombre);
                cmd.Parameters.AddWithValue("@Contrasenia", Contraseña);

                connection.Open();
                cmd.ExecuteNonQuery();
            }

            // Asignamos el admin al Diccionario
            Admins[Nombre] = this;
        }

        public void Update()
        {
            // Updateamos en la BD.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE admin SET contrasenia = @Contrasenia WHERE admin_nombre = @NombreAdmin";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@NombreAdmin", Nombre);
                cmd.Parameters.AddWithValue("@Contrasenia", Contraseña);

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
                cmd.CommandText = "DELETE FROM admin WHERE admin_nombre = @NombreAdmin";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@NombreAdmin", Nombre);

                connection.Open();

                cmd.ExecuteNonQuery();
            }

            Admins.Remove(Nombre);
        }
    }

}
