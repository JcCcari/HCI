using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Net;
using System.Net.Sockets;

namespace AssemblyCSharp
{
    [Serializable]
    public class PlayerInfo
    {
        public const int CLIENT_TYPE_PLAYER = 0;
        public const int CLIENT_TYPE_SPECTATOR = 1;

        public const int OPTION_CONNECTION = 1;
        public const int OPTION_LOGIN = 2;
        public const int OPTION_WAIT_PLAYERS = 3;
        public const int OPTION_SET_GAME = 4;
        public const int OPTION_SET_MAP = 5;
        public const int OPTION_PLAYER_POSITION = 6;
        public const int OPTION_POSITION_VALIDATION = 7;
        public const int OPTION_START = 8;

        public const int OPTION_SUMMON_MONSTER = 9;
        public const int OPTION_ATTACK_MONSTER = 10;
        public const int OPTION_DESTROY_MONSTER = 11;
        public const int OPTION_PUT_CARD_FACE_DOWN = 12; // carta boca abajo
        public const int OPTION_ACTIVATE_CARD_FACE_DOWN = 13;
      
        public const int OPTION_NOT_VALID = 14;
        public const int OPTION_GAME_FINISHED = 20;
        public const int OPTION_VIEW_AS_SPECTATOR = 30;
        public const int OPTION_DISCONNECT = 99;

        private static PlayerInfo instance = null;
        int typePlayer; // 0:Duelist   1:Viewer

        Socket playerSocket = null;
        private byte[] _recieveBuffer = new byte[256];

        //private List<String> objectsTouched;
        private HashSet<String> objectsTouched;

        public int currentOption;

        public PlayerInfo()
        {
            //objectsTouched = new List<String>();
            objectsTouched = new HashSet<String>();
        }

        public static PlayerInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerInfo();
                return instance;
            }
        }



        public HashSet<String> ObjectsTouched { get { return objectsTouched; } }
        //public List<String> ObjectsTouched { get { return objectsTouched; } }

        public void sendPlay()
        {
            
        }

        public bool startConnection(String ip, int port, int _typePlayer)
        {
            Debug.Log("#Trying to connect ...");
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
                Debug.Log("#[ERROR] Wrong Connection ");
                return false;
            }
            Debug.Log("#[OK] Connection Succesfull");
            return true;
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            //Check how much bytes are recieved and call EndRecieve to finalize handshake
            int recieved = playerSocket.EndReceive(AR);
            if (recieved <= 0){  return; }
            //Copy the recieved data into new buffer , to avoid null bytes
            byte[] recData = new byte[recieved];
            Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

            Debug.Log(recData.ToString() );
            
            /*if (receiveString(recData, recieved)){ // Saving data in playerInfo
                workData();//workData
            }
            if (option != OPTION_GAME_FINISHED && option != OPTION_DISCONNECT){
                socket.BeginReceive(
                    _recieveBuffer,
                    0,
                    _recieveBuffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    null
                );
            }
            */
        }

        public void sendString(Socket sock, String message)
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

        public void select(String objname)
        {
            Debug.Log("#After " + objectsTouched.Count);
            objectsTouched.Add(objname);
            Debug.Log("#Before " + objectsTouched.Count);
        }

        public void attack()
        {

        }
    }
}

