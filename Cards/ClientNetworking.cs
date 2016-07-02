using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JoePitt.Cards
{
    /// <summary>
    /// Provies Client Networking to join and play a game over a network.
    /// </summary>
    public class ClientNetworking
    {
        /// <summary>
        /// The IP Address the Client is connected to.
        /// </summary>
        public string Address { get; private set; }
        /// <summary>
        /// The Port the Client is connected to.
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// The history of the Clients communications.
        /// </summary>
        public List<string> Log { get; private set; } = new List<string>();
        /// <summary>
        /// The Game the Client is connected to.
        /// </summary>
        public Game Game { get; private set; }
        /// <summary>
        /// The Player who's Client this is.
        /// </summary>
        public Player Owner { get; private set; }
        /// <summary>
        /// If there is a command waiting to be sent.
        /// </summary>
        public bool NewCommand { get; set; } = false;
        /// <summary>
        /// The next command to be sent.
        /// </summary>
        public string NextCommand { get; set; } = "";
        /// <summary>
        /// If there is a new response waiting to be processed.
        /// </summary>
        public bool NewResponse { get; set; } = false;
        /// <summary>
        /// The last response from the server.
        /// </summary>
        public string LastResponse { get; private set; } = "";

        /// <summary>
        /// The thread which runs all communications.
        /// </summary>
        private Thread ClientThread;

        /// <summary>
        /// Sets up the Player's Networking.
        /// </summary>
        /// <param name="GameIn">The Game the networking is for.</param>
        /// <param name="OwnerIn">The Player the networking is for.</param>
        /// <param name="ServerAddress">The IP Address of the hosted game.</param>
        /// <param name="ServerPort">The port of the hosted game.</param>
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
        /// Handles communications with the host, run by ClientThread.
        /// </summary>
        private void RunSession()
        {
            TcpClient tcpClient;
            NetworkStream clientStream;
            Log.Add(DateTime.Now.ToString() + ": Attempting to connect to " + Address + ":" + Port + "...");
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
