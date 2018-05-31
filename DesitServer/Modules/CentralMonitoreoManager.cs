using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesitServer.Models;
using DesitServer.Models.Central.Log;
using DesitServer.Models.Central;

namespace DesitServer.Modules
{

    public class CentralMonitoreoManager
    {
        private static CentralMonitoreoManager instance;

        public static CentralMonitoreoManager Instance {
            get {
                if (instance == null) instance = new CentralMonitoreoManager();
                return instance;
            }
        }

        // Centrales conectadas <socketId, central>
        private Dictionary<string, CentralMonitoreo> centrales;

        private CentralMonitoreoManager()
        {
            centrales = new Dictionary<string, CentralMonitoreo>();
        }

        public CentralMonitoreo ObtenerCentral(string socketId)
        {
            if (centrales.TryGetValue(socketId, out CentralMonitoreo c)) return c;
            return null;
        }

        public bool ExisteCentral(string socketId)
        {
            return centrales.Keys.Contains(socketId);
        }

        /**
         * Devuelve el socketId de una central conectada.
         */
        public string ObtenerSocketId(string centralId, string contraseña)
        {
            CentralMonitoreo c = CentralMonitoreo.Get(centralId);
            if (c == null || !c.Contraseña.Equals(contraseña)) return null;

            // Si se puede identificar como central, devuelve el socketId si hay una conexión, o null si la central no está conectada
            KeyValuePair<string, CentralMonitoreo> conexion = centrales.Where(s => s.Value.CentralID == c.CentralID).FirstOrDefault();
            return conexion.Key;
        }

        /**
         * Conecta una central y la agrega al diccionario.
         */
        public bool ConectarCentral(string socketId, string centralId, string contraseña)
        {
            
            CentralMonitoreo c = CentralMonitoreo.Get(centralId);
            if (c == null || !c.Contraseña.Equals(contraseña)) return false;
            
            CentralLog log = new CentralLog(c, CentralLogTipo.Get(ECentralLogTipo.Conectado));
            log.Save();

            centrales[socketId] = c;
            
            return true;
        }

        /**
         * Desconecta una central
         */
        public bool DesconectarCentral(string socketId)
        {
            if (centrales.TryGetValue(socketId, out CentralMonitoreo c))
            {
                CentralLog log = new CentralLog(c, CentralLogTipo.Get(ECentralLogTipo.Desconectado));
                log.Save();
                centrales.Remove(socketId);
                return true;
            }
            return false;
        }

    }
}
