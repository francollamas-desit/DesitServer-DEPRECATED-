﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models.Administrador.Log
{
    public enum EAdminLogTipo
    {
        // TODO: completar con algún tipo de log
    }

    public class AdminLogTipo : IModel
    {
        // Diccionario de Tipos de Log
        private static Dictionary<int, AdminLogTipo> TiposLog = new Dictionary<int, AdminLogTipo>();

        public int? TipoLogId { get; private set; }
        public String Nombre { get; set; }
        public String Descripción { get; set; }

        public static List<AdminLogTipo> GetAll()
        {
            List<AdminLogTipo> tiposLog = new List<AdminLogTipo>();

            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM admin_log_tipo";
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["admin_log_tipo_ID"]);

                        // Si existe el tipo de log en memoria, lo cargo
                        TiposLog.TryGetValue(id, out AdminLogTipo tipoLog);

                        // Actualiza los datos del tipo de log (sea nuevo o recien creado en memoria)
                        tipoLog.TipoLogId = id;
                        tipoLog.Nombre = reader["nombre"].ToString();
                        tipoLog.Descripción = reader["descripcion"].ToString();

                        // Agrego el tipo de log a la lista de retorno
                        tiposLog.Add(tipoLog);

                        // Cabe aclarar que para un getAll() no necesitamos guardarlo en el diccionario si no existía ya...
                    }
                }
            }

            return tiposLog;
        }

        public static AdminLogTipo Get(EAdminLogTipo idLog)
        {
            int id = (int)idLog;
            AdminLogTipo tipoLog;
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM admin_log_tipo WHERE admin_log_tipo_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Si ya está en el Diccionario, obtiene el objeto, de lo contrario, lo crea
                        if (!TiposLog.TryGetValue(id, out tipoLog))
                        {
                            tipoLog = new AdminLogTipo();
                            TiposLog[id] = tipoLog;
                        }

                        // Actualiza los datos del tipo de log (sea nuevo o recien creado en memoria)
                        tipoLog.TipoLogId = id;
                        tipoLog.Nombre = reader["nombre"].ToString();
                        tipoLog.Descripción = reader["descripcion"].ToString();
                    }
                    else return null;
                }
            }

            return tipoLog;
        }

        public void Save()
        {
            // Si el tipo de log que queremos guardar ya existe en memoria, no sigue.
            if (TipoLogId.HasValue) return;

            // De lo contrario, guardamos.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO admin_log_tipo (nombre, descripcion) VALUES (@Nombre, @Descripcion)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Nombre", Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", Descripción);

                connection.Open();

                cmd.ExecuteNonQuery();
                TipoLogId = (int)cmd.LastInsertedId;
            }

            // Asignamos el tipo de log al Diccionario
            TiposLog[TipoLogId.GetValueOrDefault()] = this;
        }

        public void Update()
        {
            // Si no existe todavía, no se puede actualizar.
            if (!TipoLogId.HasValue) return;

            // De lo contrario, updateamos en la BD.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE admin_log_tipo SET nombre = @Nombre, descripcion = @Descripcion WHERE admin_log_tipo_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", TipoLogId.GetValueOrDefault());
                cmd.Parameters.AddWithValue("@Nombre", Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", Descripción);

                connection.Open();

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete()
        {
            // Si no existe todavía, no se puede borrar.
            if (!TipoLogId.HasValue) return;

            // De lo contrario, borramos del la BD y el Diccionario.
            using (MySqlConnection connection = new MySqlConnection(DbAccess.Instance.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "DELETE FROM admin_log_tipo WHERE admin_log_tipo_ID = @Id";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", TipoLogId.GetValueOrDefault());

                connection.Open();

                cmd.ExecuteNonQuery();
            }

            TiposLog.Remove(TipoLogId.GetValueOrDefault());
        }
    }
}
