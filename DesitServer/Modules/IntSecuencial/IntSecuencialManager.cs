using DesitServer.Messages;
using DesitServer.Models;
using DesitServer.Models.InterrogacionSecuencial;
using DesitServer.Modules.InterrogacionSecuencial;
using DesitServer.Models.Central.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesitServer.Modules
{
    public class IntSecuencialManager
    {
        private static IntSecuencialManager instance;

        public static IntSecuencialManager Instance {
            get {
                if (instance == null) instance = new IntSecuencialManager();
                return instance;
            }
        }

        public IntSecuencial config { get; private set; }

        private Dictionary<string, Timer> centralesSinRespuesta;
        private Timer mainTimer;

        public IntSecuencialManager()
        {
            config = IntSecuencial.Get();

            centralesSinRespuesta = new Dictionary<string, Timer>();
        }

        public void IniciarProceso(object state)
        {
            // TODO: mandar nuevos datos a la int secuencial.

            // Inicia el proceso...
            mainTimer = new Timer(InterrogarCentrales, null, 5000, config.Intervalo);
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
                    Timer timer = new Timer(ReintentarInterrogacion, reintento, config.TimeOut, config.TimeOut);
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
                if (reintento.Reintentos < config.Reintentos) {
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

        public void CambiarDatos(int? intervalo, int? timeout, int? reintentos)
        {
            long oldIntervalo = config.Intervalo;
            
            if (intervalo.HasValue) config.Intervalo = intervalo.GetValueOrDefault();
            if (timeout.HasValue) config.TimeOut = timeout.GetValueOrDefault();
            if (reintentos.HasValue) config.Reintentos = reintentos.GetValueOrDefault();
            config.Update();
            mainTimer.Dispose();

            // Reinicio el proceso en un momento en donde sepa que ya no se está mandando más nada.
            new Timer(IniciarProceso, null, (int)(oldIntervalo * 1.5), Timeout.Infinite);
            

            // TODO: decir a todas las centrales que paren el chequeo
        }
    }
}
