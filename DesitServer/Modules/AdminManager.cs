using DesitServer.Models.Admin;
using DesitServer.Models.Administrador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Modules
{
    public class AdminManager
    {
        private static AdminManager instance;

        public static AdminManager Instance {
            get {
                if (instance == null) instance = new AdminManager();
                return instance;
            }
        }

        // Admins conectados <socketId, admin>
        private Dictionary<string, Admin> admins;

        private AdminManager()
        {
            admins = new Dictionary<string, Admin>();
        }

        /**
         * Devuelve el socketId de un admin conectado.
         */
        public string ObtenerSocketId(string nombre, string contraseña)
        {
            Admin a = Admin.Get(nombre);
            if (a == null || !a.Contraseña.Equals(contraseña)) return null;

            // Si se puede identificar como admin, devuelve el socketId si hay una conexión, o null si el admin no está conectado
            KeyValuePair<string, Admin> conexion = admins.Where(s => s.Value.Nombre == a.Nombre).FirstOrDefault();
            return conexion.Key;
        }

        /**
         * Conecta un admin y lo agrega al diccionario.
         */
        public bool ConectarAdmin(string socketId, string nombre, string contraseña)
        {

            Admin a = Admin.Get(nombre);
            if (a == null || !a.Contraseña.Equals(contraseña)) return false;

            // TODO: posible logeo del log del admin?
            admins[socketId] = a;
            return true;
        }

        /**
         * Desconecta un admin
         */
        public bool DesconectarAdmin(string socketId)
        {
            if (admins.TryGetValue(socketId, out Admin c))
            {
                // TODO: posible logeo de desconexión del admin?

                admins.Remove(socketId);
                return true;
            }
            return false;
        }

    }
}
