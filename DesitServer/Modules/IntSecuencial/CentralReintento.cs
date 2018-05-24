using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Modules.IntSecuencial
{
    public class CentralReintento
    {
        public string SocketID { get; private set; }
        public int Reintentos { get; private set; }

        public CentralReintento(string socketId)
        {
            SocketID = socketId;
        }

        public void SiguienteReintento()
        {
            Reintentos += 1;
        }
    }
}
