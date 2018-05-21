using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Modules
{
    public class InterrogacionSecuencial
    {
        private long Intervalo { get; set; }
        private long TimeOut { get; set; }
        private int Reintentos { get; set; }

        public InterrogacionSecuencial()
        {
            Intervalo = 5000;
            TimeOut = 500;
            Reintentos = 3;
            
        }


    }
}
