using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

namespace AssemblyCSharp
{
    [Serializable]
    public class PlayerInfo
    {
        public const int MONSTER_CARD = 0;
        public const int TRAP_CARD = 1;
        public const int MAGIC_CARD = 2;

        private static PlayerInfo instance = null;
        int typePlayer; // 0:Duelist   1:Viewer

        DateTime time = DateTime.Now;

        Socket playerSocket = null;
        private byte[] _recieveBuffer = new byte[256];

        public static PlayerInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerInfo();
                return instance;
            }
        }

        public void sendPlay()
        {
            TimeSpan elapsed = DateTime.Now() - time;
            if( elapsed.TotalMilliseconds > 1000 ){
                time = DateTime.Now();
                //this.sendString(playerSocket, )
            }
        }

        public bool startConnection(String ip, int port, int _typePlayer)
        {
            try{
                this.typePlayer = _typePlayer;

                playerSocket = new Socket( 
                    AddressFamily.InterNetwork, 
                    SocketType.Stream, 
                    ProtocolType.Tcp 
                );
                playerSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                playerSocket.BeginReceive( _recieveBuffer, 0,
                    _recieveBuffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    null
                );
            }
            catch (SocketException ex){
                Debug.Log(ex.Message);
                playerSocket = null;
                return false;
            }
            return true;
        }

        void sendString(Socket sock, String message)
        {
            try{
                byte[] a2 = System.Text.Encoding.ASCII.GetBytes(message);
                int bytes = sock.Send(a2);
                //Debug.LogFormat("Sending: {0} bytes", bytes);
                Debug.LogFormat("Sending: {0}", message.ToString());
            }
            catch (SocketException ex){
                Debug.Log(ex.Message);
            }
        }
    }
}

