﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using DesitServer.Models;
using WebSocketManager;
using WebSocketManager.Common;
using DesitServer.Modules;

namespace DesitServer.Messages
{
    /**
     * Mensajes del servidor al cliente
     */
    public class MessagesHandler : WebSocketHandler
    {
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

            Timer timer = new Timer(BorrarSocket, socket, 5000, Timeout.Infinite); // TODO: poner el TimeOut en una constante
            conexionesSinAutenticar[socketId] = timer;
        }

        /**
         * Elimina el Socket cuando pasado un tiempo no se autenticó.
         */
        private void BorrarSocket(Object o)
        {
            WebSocket ws = (WebSocket)o;
            string socketId = WebSocketConnectionManager.GetId(ws);
            
            conexionesSinAutenticar[socketId].Dispose();
            conexionesSinAutenticar.Remove(socketId);

            OnDisconnected(ws);
        }


        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);

            // Intenta desconectar la central
            if (!CentralMonitoreoManager.Instance.DesconectarCentral(socketId))
            {
                // TODO: desconectar ADMIN.
            }

            await base.OnDisconnected(socket);
        }

        public bool Handshake(WebSocket socket, string centralID, string contraseña)
        {
            string socketId = WebSocketConnectionManager.GetId(socket);

            bool conectado = CentralMonitoreoManager.Instance.ConectarCentral(socketId, centralID, contraseña);
            if (!conectado) OnDisconnected(socket);

            conexionesSinAutenticar[socketId].Dispose();
            conexionesSinAutenticar.Remove(socketId);
            return conectado;
        }

    }
}
