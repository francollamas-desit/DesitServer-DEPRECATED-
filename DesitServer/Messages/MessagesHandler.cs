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
    public enum ETipoConexion
    {
        Admin = 69,
        Central = 0
    }

    /**
     * Mensajes del servidor al cliente
     */
    public class MessagesHandler : WebSocketHandler
    {
        public const int WS_TIMEOUT = 5000; // TODO: poner en 60000 (1 minuto)

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
            
            // Intenta desconectar admin o central
            if (socketId != null)
            {
                // Intentamos remover de ambos grupos.. en alguno tiene que existir
                WebSocketConnectionManager.RemoveFromGroup(socketId, ETipoConexion.Central.ToString());
                WebSocketConnectionManager.RemoveFromGroup(socketId, ETipoConexion.Admin.ToString());

                if (!AdminManager.Instance.DesconectarAdmin(socketId)) CentralMonitoreoManager.Instance.DesconectarCentral(socketId);
            }

            await base.OnDisconnected(socket);
        }


        public bool Handshake(WebSocket socket, int tipoConexionNum, string identificador, string contraseña)
        {
            ETipoConexion tipoConexion = (ETipoConexion)tipoConexionNum;

            string socketId = WebSocketConnectionManager.GetId(socket);

            // Chequeamos si hay una conexión anterior a la central o al administrador, y la desconectamos...
            string oldSocketId = null;
            if (tipoConexion == ETipoConexion.Central)
            {
                oldSocketId = CentralMonitoreoManager.Instance.ObtenerSocketId(identificador, contraseña);

            }
            else if (tipoConexion == ETipoConexion.Admin)
            {
                oldSocketId = AdminManager.Instance.ObtenerSocketId(identificador, contraseña);
            }

            if (oldSocketId != null)
            {
                try
                {
                    OnDisconnected(WebSocketConnectionManager.GetSocketById(oldSocketId));
                }
                catch (WebSocketException)
                {

                }
            }
            
            // Conectamos la central o admin
            bool conectado = false;
            if (tipoConexion == ETipoConexion.Central) conectado = CentralMonitoreoManager.Instance.ConectarCentral(socketId, identificador, contraseña);
            else if (tipoConexion == ETipoConexion.Admin) conectado = AdminManager.Instance.ConectarAdmin(socketId, identificador, contraseña);

            // Si no se realizó el handshake
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
            else
            {
                WebSocketConnectionManager.AddToGroup(socketId, tipoConexion.ToString());
            }
            
            conexionesSinAutenticar[socketId].Dispose();
            conexionesSinAutenticar.Remove(socketId);

            return conectado;
        }

    }
}
