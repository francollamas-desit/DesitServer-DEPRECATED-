using DesitServer.Messages;
using DesitServer.Models;
using DesitServer.Modules.IntSecuencial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesitServer.Modules
{
    public class InterrogacionSecuencial
    {
        private static InterrogacionSecuencial instance;

        public static InterrogacionSecuencial Instance {
            get {
                if (instance == null) instance = new InterrogacionSecuencial();
                return instance;
            }
        }

        private long Intervalo { get; set; }
        private long TimeOut { get; set; }
        private int Reintentos { get; set; }

        private Dictionary<string, Timer> centralesSinRespuesta;

        public InterrogacionSecuencial()
        {
            Intervalo = 20000;
            TimeOut = 2000;
            Reintentos = 3;

            centralesSinRespuesta = new Dictionary<string, Timer>();
        }

        public void IniciarProceso()
        {
            // Inicia el proceso...
            Timer timer = new Timer(InterrogarCentrales, null, 5000, Intervalo);
        }

        private async void InterrogarCentrales(object state)
        {
            List<string> conexiones = MessagesHandler.Instance.ObtenerSocketIdCentrales();
            if (conexiones == default(List<string>)) return;

            // Manda el mensaje a todas las centrales
            foreach (string socketId in conexiones)
            {
                // Si justo se desconectó una de las centrales, la saltea
                if (CentralMonitoreoManager.Instance.ExisteCentral(socketId))
                {

                    CentralReintento reintento = new CentralReintento(socketId);
                    Timer timer = new Timer(ReintentarInterrogacion, reintento, TimeOut, TimeOut);
                    centralesSinRespuesta[socketId] = timer;
                    await MessagesHandler.Instance.InvokeClientMethodOnlyAsync(socketId, "intSecuencial", 0);
                }
            }
        }

        public async void CentralResponde(string socketId)
        {
            // Destruyo el timer
            if (centralesSinRespuesta.TryGetValue(socketId, out Timer timer))
            {
                timer.Dispose();
                centralesSinRespuesta.Remove(socketId);
            }
        }

        public async void ReintentarInterrogacion(object state) {
            CentralReintento reintento = (CentralReintento)state;
            reintento.SiguienteReintento();

            // Chequeamos que siga existiendo
            if (CentralMonitoreoManager.Instance.ExisteCentral(reintento.SocketID))
            {
                // Si se puede reintentar
                if (reintento.Reintentos < Reintentos) {
                    await MessagesHandler.Instance.InvokeClientMethodOnlyAsync(reintento.SocketID, "intSecuencial", reintento.Reintentos);
                }

                // Si llegó al límite de reintentos
                else
                {
                    // Destruyo el timer
                    if (centralesSinRespuesta.TryGetValue(reintento.SocketID, out Timer timer))
                    {
                        timer.Dispose();
                        centralesSinRespuesta.Remove(reintento.SocketID);
                    }

                    // Logeo la pérdida de conexión...
                    CentralLog log = new CentralLog(CentralMonitoreoManager.Instance.ObtenerCentral(reintento.SocketID), CentralLogTipo.Get(ECentralLogTipo.PerdidaConexion));
                    log.Save();

                    // Me desconecto de la central...
                    await MessagesHandler.Instance.OnDisconnected(reintento.SocketID);
                }
            }
        }

    }
}
