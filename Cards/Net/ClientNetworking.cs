using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace JoePitt.Cards.Net
{
    /// <summary>
    /// Provies Client Networking to join and play a game over a network.
    /// </summary>
    internal class ClientNetworking
    {
        private string Address;
        private int Port;
        private Thread ClientThread;

        /// <summary>
        /// The player who's Connection this is.
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
        /// Sets up the Player's Networking.
        /// </summary>
        /// <param name="ownerIn">The Player the networking is for.</param>
        /// <param name="serverAddress">The IP Address of the hosted game.</param>
        /// <param name="serverPort">The port of the hosted game.</param>
        public ClientNetworking(Player ownerIn, string serverAddress, int serverPort)
        {
            Owner = ownerIn;
            Address = serverAddress;
            Port = serverPort;
            ClientThread = new Thread(new ThreadStart(RunSession));
            ClientThread.Name = "Cards Client";
            ClientThread.Start();
        }

        private void RunSession()
        {
            TcpClient tcpClient;
            NetworkStream clientStream;
            try
            {
                tcpClient = new TcpClient(Address, Port);
                clientStream = tcpClient.GetStream();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Unable to connect to game (" + ex.Message + ")", "Unable to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Restart();
                return;
            }
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("");
            byte[] message = new byte[4];
            int bytesRead;

            while (Program.CurrentGame.Playable)
            {
                message = new byte[4];
                if (NewCommand)
                {
                    buffer = encoder.GetBytes(NextCommand);
                    clientStream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    NewCommand = false;
                    bytesRead = 0;
                    try
                    {
                        //blocks until the server sends a message
                        //bytesRead = clientStream.Read(message, 0, 5243000);
                        bytesRead = clientStream.Read(message, 0, 4);
                        int messageLength = BitConverter.ToInt32(message, 0);
                        message = new byte[messageLength];
                        bytesRead = clientStream.Read(message, 0, messageLength);
                    }
                    catch (System.IO.IOException ex)
                    {
                        //a socket error has occured
                        MessageBox.Show("A socket error has occurred on the client connection (" + ex.Message + ")", "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Restart();
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        //the server has disconnected
                        break;
                    }

                    //message has successfully been received
                    string ServerText = encoder.GetString(message, 0, bytesRead);
                    LastResponse = ServerText.Replace(Environment.NewLine, "");
                    NewResponse = true;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
