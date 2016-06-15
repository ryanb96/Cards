using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Management;
using System.Security.Cryptography;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace JoePitt.Cards
{
    /// <summary>
    /// Provies Client Networking to join and play a game over a network.
    /// </summary>
    public class ClientNetworking
    {
        private string Address;
        private int Port;
        public List<string> Log = new List<string>();
        private Game Game;
        public Player Owner;
        private Thread ClientThread;

        public bool NewCommand = false;
        public string NextCommand = "";
        public bool NewResponse = false;
        public string LastResponse = "";

        /// <summary>
        /// Initialise networking for the specified Game.
        /// </summary>
        /// <param name="GameIn">The Game the networking is for.</param>
        public ClientNetworking(Game GameIn, Player OwnerIn, string ServerAddress, int ServerPort)
        {
            Game = GameIn;
            Owner = OwnerIn;
            Address = ServerAddress;
            Port = ServerPort;
            Log.Add(DateTime.Now.ToString() + ": Client Networking Module Initalised " + Address + ":" + Port);
            ClientThread = new Thread(new ThreadStart(RunSession));
            ClientThread.Name = "Client";
            ClientThread.Start();
        }

        /// <summary>
        /// Connects to the remove server and works a a messenger between local and remote game.
        /// </summary>
        private void RunSession()
        {
            TcpClient tcpClient;
            NetworkStream clientStream;
            Log.Add(DateTime.Now.ToString() + ": Attempting to connect to " + Address + ":" + Port +"...");
            try
            {
                tcpClient = new TcpClient(Address, Port);
                clientStream = tcpClient.GetStream();
            }
            catch
            {
                Log.Add(DateTime.Now.ToString() + ": ! Failed to Connect !");
                return;
            }
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("");
            //byte[] message = new byte[4096];
            byte[] message = new byte[1048576];
            int bytesRead;

            while (true)
            {
                if (NewCommand)
                {
                    buffer = encoder.GetBytes(NextCommand);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    NewCommand = false;
                    Log.Add(DateTime.Now.ToString() + ": > " + NextCommand);
                    bytesRead = 0;
                    try
                    {
                        //blocks until the server sends a message
                        bytesRead = clientStream.Read(message, 0, 1048576);
                    }
                    catch
                    {
                        //a socket error has occured
                        Log.Add(DateTime.Now.ToString() + ": ! SOCKET ERROR !");
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        //the server has disconnected
                        Log.Add(DateTime.Now.ToString() + ": ! Server Diconnnected !");
                        break;
                    }

                    //message has successfully been received
                    string ServerText = encoder.GetString(message, 0, bytesRead);
                    LastResponse = ServerText.Replace(Environment.NewLine, "");
                    NewResponse = true;
                    Log.Add(DateTime.Now.ToString() + ": < " + LastResponse);
                    while (NewResponse)
                    {
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
