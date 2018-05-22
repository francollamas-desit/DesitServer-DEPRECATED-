using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using DesitServer.Models;
using DesitServer.Modules;
using WebSocketManager;
using WebSocketManager.Common;

namespace DesitServer.Messages
{
    /**
     * Mensajes del servidor al cliente
     */
    public class MessagesHandler : WebSocketHandler
    {
        public const int WS_TIMEOUT = 10000;

        // Referencia a los Mensajes
        public static MessagesHandler instance { get; private set; }

        // Conexiónes entrantes que todavía no se logearon
        private Dictionary<string, Timer> conexionesSinAutenticar;

        // Constructor
        public MessagesHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager, new ControllerMethodInvocationStrategy())
        {
            instance = this;
            conexionesSinAutenticar = new Dictionary<string, Timer>();
            ((ControllerMethodInvocationStrategy)MethodInvocationStrategy).Controller = this;
        }

        // Cuando un cliente se conectó
        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            string socketId = WebSocketConnectionManager.GetId(socket);

            Timer timer = new Timer(BorrarSocket, socket, WS_TIMEOUT, Timeout.Infinite);
            conexionesSinAutenticar[socketId] = timer;
        }

        /**
         * Elimina el Socket cuando pasado un tiempo no se autenticó.
         */
        private async void BorrarSocket(Object o)
        {
            WebSocket ws = (WebSocket)o;

            if (!(ws.State == WebSocketState.Open)) return;

            string socketId = WebSocketConnectionManager.GetId(ws);
            
            conexionesSinAutenticar[socketId].Dispose();
            conexionesSinAutenticar.Remove(socketId);

            try
            {
                await OnDisconnected(ws);
            }
            catch (WebSocketException)
            {

            }
        }


        public override async Task OnDisconnected(WebSocket socket)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);

            // Intenta desconectar la central
            if (socketId != null && !CentralMonitoreoManager.Instance.DesconectarCentral(socketId))
            {
                // TODO: desconectar ADMIN.
            }

            await base.OnDisconnected(socket);
        }

        public bool Handshake(WebSocket socket, string centralID, string contraseña)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);

            bool conectado = CentralMonitoreoManager.Instance.ConectarCentral(socketId, centralID, contraseña);
            if (!conectado)
            {
                try
                {
                    OnDisconnected(socket);
                }
                catch (WebSocketException)
                {

                }
            }

            conexionesSinAutenticar[socketId].Dispose();
            conexionesSinAutenticar.Remove(socketId);
            return conectado;
           
        }

    }
}
