using System;
using SuperWebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace TaskLeader.Server
{
    class GUIServer
    {
        private IBootstrap bootstrap;
        private WebSocketServer appServer { get { return bootstrap.GetServerByName("GUIserver") as WebSocketServer; } }

        public GUIServer()
        {
            bootstrap = BootstrapFactory.CreateBootstrap();
            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize!");
                return;
            }
        }

        public void Start()
        {
            var result = bootstrap.Start();

            Console.WriteLine("Start result: {0}!", result);
            if (result == StartResult.Failed)
            {
                Console.WriteLine("Failed to start!");
                return;
            }

            appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
            appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);

            Console.WriteLine("Press key 'q' to stop it!");
            Console.WriteLine();
        }

        public void Stop()
        {
            //Stop the appServer
            bootstrap.Stop();

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
        }

        private void appServer_NewSessionConnected(WebSocketSession session)
        {
            if (appServer.Logger.IsDebugEnabled)
                appServer.Logger.Debug("Client connected from: " + session.RemoteEndPoint);
        }

        private void appServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            if (appServer.Logger.IsDebugEnabled)
                appServer.Logger.Debug("Client disconnected: " + session.RemoteEndPoint);
        }

        public void SendToAll(string message)
        {
            foreach (var s in appServer.GetAllSessions())
            {
                s.Send(message);
            }
        }
    }
}