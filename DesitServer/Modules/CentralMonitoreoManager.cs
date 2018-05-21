﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesitServer.Models;

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

        /**
         * Conecta una central y la agrega al diccionario.
         */
        public bool ConectarCentral(string socketId, string centralId, string contraseña)
        {
            CentralMonitoreo c = CentralMonitoreo.Get(centralId);
            if (c == null || !c.Contraseña.Equals(contraseña)) return false;

            Log log = new Log(c, TipoLog.Get(ETipoLog.Conectado));
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
                Log log = new Log(c, TipoLog.Get(ETipoLog.Desconectado));
                log.Save();
                centrales.Remove(socketId);
                return true;
            }
            return false;
        }

    }
}