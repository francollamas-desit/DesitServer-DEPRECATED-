using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;
using WebSocketManager.Common;
using DesitServer.Models;

namespace DesitServer.Messages
{
    /**
     * Mensajes del servidor al cliente
     */
    public class MessagesHandler : WebSocketHandler
    {
        public MessagesHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager, new ControllerMethodInvocationStrategy())
        {
            ((ControllerMethodInvocationStrategy)MethodInvocationStrategy).Controller = this;
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
        }
        
        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);

            await base.OnDisconnected(socket);
        }

        /**
         * Conecta una central de monitoreo.
         */
        public bool ConnectCM(WebSocket socket, string id, string contraseña)
        {
            /*CentralMonitoreo central = CentralMonitoreo.getCentralMonitoreo(id);
            if (central != null && id.Equals(central.Central_ID) && contraseña.Equals(central.Contraseña))
            {
                return true;
            }
            
            WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));

            return false;*/

            Barrio b = new Barrio() { Nombre = "Barrio Prueba 2"};

            return true;
        }
    }
}
