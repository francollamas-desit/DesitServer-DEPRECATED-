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

        public string ConnectCM(WebSocket socket, string id, string password)
        {
            return CentralMonitoreo.getCentralMonitoreo(id).Central_ID.ToString();
        }
    }
}
