using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;
using WebSocketManager.Common;

namespace DesitServer.Messages
{
    /**
     * Mensajes del servidor al cliente
     */
    public class MessagesHandler : WebSocketHandler
    {
        public MessagesHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        /**
         * Evento que sucede cuando se conecta un Dispositivo
         */
        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var socketId = WebSocketConnectionManager.GetId(socket);

            var message = new Message() {
                MessageType = MessageType.Text,
                Data = $"Cliente: {socketId} conectado!"
            };

            await SendMessageToAllAsync(message);
        }

        /**
         * Evento que sucede cuando se desconecta un Dispositivo
         */
        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            await base.OnDisconnected(socket);

            var message = new Message() {
                MessageType = MessageType.Text,
                Data = $"Cliente: {socketId} desconectado."
            };
            await SendMessageToAllAsync(message);
        }


        public async Task SendMessage(string socketId, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            await InvokeClientMethodToAllAsync("receiveMessage", socketId, message);
        }
    }
}
