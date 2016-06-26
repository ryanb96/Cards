using NATUPNPLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace JoePitt.Cards
{
    /// <summary>
    /// Provides Server Networking for a new game.
    /// </summary>
    public class ServerNetworking
    {
        Guid GameGUID = new Guid();
        const int DefaultPort = 60069;
        public int Port = DefaultPort;
        public bool IPv4UPnP = false;
        public bool IPv6UPnP = false;
        public List<IPAddress> LocalIPAddresses = new List<IPAddress>();
        public IPAddress PublicIP = IPAddress.Parse("0.0.0.0");
        public List<string> Log = new List<string>();
        public TcpListener tcpListener;
        public Thread listenThread;
        public List<Thread> ClientThreads = new List<Thread>();
        private List<TcpClient> ClientTCPClients = new List<TcpClient>();
        private Game Game;
        private bool ListernerUp = false;

        /// <summary>
        /// Initialise networking for the specified Game.
        /// </summary>
        /// <param name="game">The Game the networking is for.</param>
        public ServerNetworking(Game game)
        {
            Game = game;
        }

        //Start of Discovery & UPnP Functions.

        /// <summary>
        /// Use ifconfig.me to get the Public IP Address.
        /// </summary>
        /// <returns>If the Public IP was acquired.</returns>
        public bool GetPublicIP()
        {
            PublicIP = IPAddress.Parse("0.0.0.0");
            var request = (HttpWebRequest)WebRequest.Create("http://ifconfig.me");
            request.UserAgent = "curl"; // this simulate curl linux command
            request.Method = "GET";
            try
            {
                string externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
                PublicIP = IPAddress.Parse(externalIP);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to UPnP Bind to the Public IP, if found, then, identifies all Internal IP addresses.
        /// </summary>
        private void AutoBind()
        {
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && Interface.GetIPProperties().GatewayAddresses.Count > 0)
                        {
                            LocalIPAddresses.Add(ip.Address);
                        }
                        else if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6 && Interface.GetIPProperties().GatewayAddresses.Count > 0)
                        {
                            LocalIPAddresses.Add(ip.Address);
                        }
                    }
                }
            }

            if (GetPublicIP())
            {
                Log.Add("Main: Detected " + PublicIP.ToString() + "as the Public IP.");
                UPnPNAT upnpnat = new UPnPNAT();
                IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
                foreach (NetworkInterface Interface in Interfaces)
                {
                    if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork && Interface.GetIPProperties().GatewayAddresses.Count > 0)
                            {
                                IPAddress IP = ip.Address;
                                GatewayIPAddressInformation Gateway = Interface.GetIPProperties().GatewayAddresses[0];
                                if (Gateway.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    retry:
                                    try
                                    {
                                        IPv4UPnP = true;
                                        Log.Add("Main: UPnP'd " + PublicIP.ToString() + ":" + Port + " (Public)");
                                    }
                                    catch
                                    {
                                        if (Port - DefaultPort < 8)
                                        {
                                            Port++;
                                            goto retry;
                                        }
                                        else
                                        {
                                            Log.Add("Main: UPnP failed, tried 8 ports.");
                                            Port = DefaultPort;
                                        }
                                    }
                                }
                            }
                            else if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6 && Interface.GetIPProperties().GatewayAddresses.Count > 0)
                            {
                                IPAddress IP = ip.Address;
                                GatewayIPAddressInformation Gateway = Interface.GetIPProperties().GatewayAddresses[0];
                                if (Gateway.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                {
                                    retry:
                                    try
                                    {
                                        IPv6UPnP = true;
                                        Log.Add("Main: UPnP'd " + PublicIP.ToString() + ":" + Port + " (Public)");
                                    }
                                    catch
                                    {
                                        if (Port - DefaultPort < 8)
                                        {
                                            Port++;
                                            goto retry;
                                        }
                                        else
                                        {
                                            Log.Add("Main: UPnP failed, tried 8 ports.");
                                            Port = DefaultPort;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!IPv4UPnP && !IPv6UPnP)
                {
                    Log.Add("Main: UPnP failed, no UPnP RootDevice found.");
                }
            }
            else
            {
                Log.Add("Main: Failed to detect Public IP.");
            }
        }

        /// <summary>
        /// Removes any UPnP Binding.
        /// </summary>
        /// <returns></returns>
        public bool DropBind()
        {
            UPnPNAT upnpnat = new UPnPNAT();
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            if (PublicIP != IPAddress.Parse("0.0.0.0"))
            {
                NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in Interfaces)
                {
                    if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork && Interface.GetIPProperties().GatewayAddresses.Count > 0)
                            {
                                IPAddress IP = ip.Address;
                                GatewayIPAddressInformation Gateway = Interface.GetIPProperties().GatewayAddresses[0];
                                if (Gateway.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    try
                                    {
                                        mappings.Remove(Port, "TCP");
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        //End of Discovery & UPnP Functions.

        //Start of Client Commnication Functions.

        /// <summary>
        /// AutoBind and start Listener.
        /// </summary>
        public void Start()
        {
            GameGUID = Guid.NewGuid();
            Port = DefaultPort;
            IPv4UPnP = false;
            LocalIPAddresses = new List<IPAddress>();
            PublicIP = IPAddress.Parse("0.0.0.0");
            Log = new List<string>();
            ClientThreads = new List<Thread>();

            Game.NetworkUp = true;
            Log.Add("Main: Starting Connectivity Discovery...");
            if (Game.GameType != 'L')
            {
                AutoBind();
            }
            Log.Add("Main: Finished Connectivity Discovery.");
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Name = "Listener";
            listenThread.Start();
        }

        /// <summary>
        /// Stops the Server, Drops all connections and Unbinds UPnP
        /// </summary>
        public void Stop()
        {
            Log.Add("Stopping Lisener");
            tcpListener.Stop();
            ListernerUp = false;
            Log.Add("Stopping TCP Clients");
            foreach (TcpClient Client in ClientTCPClients)
            {
                Client.Close();
            }
            foreach (Thread ClientThread in ClientThreads)
            {
                ClientThread.Interrupt();
            }
            Log.Add("Main: Dropping an UPnP Mappings.");
            DropBind();
        }

        /// <summary>
        /// Listens for Client Connections and hands them off to their own thread.
        /// </summary>
        private void ListenForClients()
        {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            try
            {
                tcpListener.Start();
                ListernerUp = true;
                Log.Add("Listener: Waiting for Connection...");
            }
            catch
            {
                Log.Add("Listener: Local binding failed.");
            }

            while (ListernerUp)
            {
                try
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        //create a thread to handle communication 
                        //with connected client
                        ClientTCPClients.Add(client);
                        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                        clientThread.Name = "ClientThread";
                        clientThread.Start(client);
                        ClientThreads.Add(clientThread);
                    }
                    else
                    {
                        Thread.Sleep(3000);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Handles communication with a client.
        /// </summary>
        /// <param name="client">the TcpClient handed off by the Listener.</param>
        private void HandleClientComm(object client)
        {
            string ThreadName = "ClientThread[" + Thread.CurrentThread.ManagedThreadId + "]";
            TcpClient tcpClient = (TcpClient)client;
            Log.Add(ThreadName + ": Connected to: " + tcpClient.Client.RemoteEndPoint.ToString());
            NetworkStream clientStream = tcpClient.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("");
            //byte[] message = new byte[4096];
            byte[] message = new byte[1048576];
            int bytesRead;
            BinaryFormatter formatter = new BinaryFormatter();
            Player thisPlayer = new Player("FAKE", new List<Card>());

            while (true)
            {
                bytesRead = 0;
                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 1048576);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                string clientText = encoder.GetString(message, 0, bytesRead);
                string serverText = "";
                if (clientText != "\r\n")
                {
                    Log.Add(ThreadName + ": < " + clientText);
                    string[] ClientTexts = clientText.Split(' ');
                    switch (ClientTexts[0])
                    {
                        /* Get Game Details
                         * EXPECTS
                         * [0]Command
                         * 
                         * RESPONSE
                         * HosterName PlayerInGame FreeSeats GUID_Version:GUID_Version
                         */
                        case "HELLO":
                            int players_H = 0;
                            int maxPlayers_H = Game.PlayerCount;
                            foreach (Player player in Game.Players)
                            {
                                if (player.Name != "FREESLOT" && !player.Name.ToLower().StartsWith("[bot]"))
                                {
                                    players_H++;
                                }
                            }
                            string cardSets = "";
                            foreach (CardSet set in Game.CardSets)
                            {
                                cardSets = cardSets + set.GUID.ToString() + "_" + set.Version + ":";
                            }
                            cardSets = cardSets.Substring(0, cardSets.Length - 1);

                            serverText = Game.Players[0].Name.Replace(' ', '_') + " " + players_H + " " + (maxPlayers_H - players_H) + " " + cardSets;

                            if ((maxPlayers_H - players_H) == 0 && Game.Stage == 'W')
                            {
                                Game.Stage = 'P';
                            }
                            break;

                        /* Try and (Re)Join the game.
                         * EXPECTS:
                         * [0]Command
                         * [1]Username 
                         * [2]Hash of Password
                         * 
                         * RESPONSE:
                         * SUCCESS
                         * FAILED [NoSpace/BadPass]
                         */
                        case "JOIN":
                            int playersJoin = 0;
                            int maxPlayersJoin = Game.PlayerCount;
                            foreach (Player player in Game.Players)
                            {
                                if (player.Name != "FREESLOT" && !player.Name.ToLower().StartsWith("[bot]"))
                                {
                                    playersJoin++;
                                }
                            }

                            if (ClientTexts.Length != 3)
                            {
                                serverText = "Usage: JOIN Username HashOfPassword";
                            }
                            else
                            {
                                bool found = false;
                                foreach (Player player in Game.Players)
                                {
                                    if (player.Name == ClientTexts[1].Replace('_', ' '))
                                    {
                                        found = true;
                                        if (player.Verify(ClientTexts[2]))
                                        {
                                            thisPlayer = player;
                                            serverText = "SUCCESS";
                                        }
                                        else
                                        {
                                            serverText = "FAILED BadPass";
                                        }
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    foreach (Player player in Game.Players)
                                    {
                                        if (player.Name == "FREESLOT")
                                        {
                                            player.Name = ClientTexts[1].Replace('_', ' ');
                                            player.Verify(ClientTexts[2]);
                                            player.Connection = tcpClient;
                                            serverText = "SUCCESS";
                                            thisPlayer = player;
                                            found = true;
                                            playersJoin++;
                                            break;
                                        }
                                    }
                                }
                                if (!found)
                                {
                                    serverText = "FAILED NoSpace";
                                }
                            }
                            if ((maxPlayersJoin - playersJoin) == 0 && Game.Stage == 'W')
                            {
                                Game.Stage = 'P';
                            }
                            break;

                        /*
                         * Gets the GameSet.
                         * EXPECTS
                         * [0]Command
                         * 
                         * RESPONSE:
                         * Base64 Represenation of GameSet
                         * SETERROR
                         */
                        case "GETCARDS":
                            using (MemoryStream stream = new MemoryStream())
                            {
                                formatter.Serialize(stream, Game.GameSet);
                                byte[] cardsArray = stream.ToArray();
                                serverText = Convert.ToBase64String(cardsArray);
                            }
                            break;

                        /*
                         * Exits the game
                         * EXPECTS:
                         * [0]Command
                         * 
                         * RESPONSE:
                         * RuleName:Value RuleName:Value RuleName:Value
                         */
                        case "GETRULES":
                            serverText = "CardsPerPlayer:" + Game.CardsPerUser;
                            serverText = serverText + " Rounds:" + Game.Rounds;
                            serverText = serverText + " NeverHaveI" + Game.NeverHaveI;
                            serverText = serverText + " RebootingTheUniverse" + Game.RebootingTheUniverse;
                            if (Game.Cheats)
                            {
                                serverText = serverText + " Cheats:true";
                            }
                            break;

                        /*
                         * Updates the status of the game
                         * EXPECTS:
                         * [0]Command
                         * 
                         * RESPONSE:
                         * WAITING (PreGame)
                         * PLAYING BlackCardBase64 Round
                         * VOTING AnswersBase64
                         * END
                         */
                        case "GAMEUPDATE":
                            if (Game.Stage == 'W')
                            {
                                serverText = "WAITING";
                            }
                            else if (Game.Stage == 'P')
                            {
                                serverText = "PLAYING " + Convert.ToBase64String(Game.GameSet.BlackCards[Game.GameSet.BlackCardIndex[Game.CurrentBlackCard]].ToArray()) + " " + Game.Round;
                            }
                            else if (Game.Stage == 'V')
                            {
                                serverText = "VOTING " + Convert.ToBase64String(Game.ExportAnswers());
                            }
                            else if (Game.Stage == 'E')
                            {
                                serverText = "END";
                            }
                            break;

                        /*
                         * Updates the leaderboard
                         * EXPECTS:
                         * [0]Command
                         * 
                         * RESPONSE:
                         * PlayerName:Score,PlayerName:Score,PlayerName:Score
                         */
                        case "GAMESCORES":
                            serverText = "";
                            foreach (Player player in Game.Players)
                            {
                                serverText = serverText + player.Name.Replace(' ', '_') + ":" + player.Score + ",";
                            }
                            serverText = serverText.Substring(0, serverText.Length - 1);
                            break;

                        /*
                         * Gets the players White Cards
                         * EXPECTS:
                         * [0]Command
                         * 
                         * RESPONSE
                         * CardID,CardID,CardID,CardID,CardID,CardID,CardID,CardID,CardID,CardID
                         * JoinFirst
                         */
                        case "MYCARDS":
                            if (thisPlayer.Name == "FAKE")
                            {
                                serverText = "JoinFirst";
                            }
                            else
                            {

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    formatter.Serialize(stream, thisPlayer.WhiteCards);
                                    byte[] cardsArray = stream.ToArray();
                                    serverText = Convert.ToBase64String(cardsArray);
                                }
                            }
                            break;

                        /* Find out if an action is required
                         * EXPECTS
                         * [0]Command
                         * 
                         * RESPONSE
                         * YES
                         * NO
                         */
                        case "NEED":
                            if (ClientTexts.Length != 2)
                            {
                                serverText = "Usage: NEED [ANSWER|VOTE]";
                            }
                            else
                            {
                                switch (ClientTexts[1])
                                {
                                    case "ANSWER":
                                        serverText = "YES";
                                        foreach (Answer answer in Game.Answers)
                                        {
                                            if (answer.Submitter.Name == thisPlayer.Name)
                                            {
                                                serverText = "NO";
                                            }
                                        }
                                        break;
                                    case "VOTE":
                                        serverText = "YES";
                                        foreach (Vote vote in Game.Votes)
                                        {
                                            if (vote.Voter.Name == thisPlayer.Name)
                                            {
                                                serverText = "NO";
                                            }
                                        }
                                        break;
                                    default:
                                        serverText = "Usage: NEED [ANSWER|VOTE]";
                                        break;
                                }
                            }
                            break;

                        /*
                         * Submits the players answer
                         * EXPECTS:
                         * [0]Command
                         * [1]Base64 Encoded Answer
                         * 
                         * RESPONSE:
                         * SUBMITTED
                         * ERROR 
                         */
                        case "SUBMIT":
                            if (ClientTexts.Length != 2)
                            {
                                serverText = "Usage: SUBMIT WhiteCardID [2ndWhiteCardID]";
                            }
                            else
                            {
                                int i = 0;
                                foreach (Answer answer in Game.Answers)
                                {
                                    if (answer.Submitter.Name == thisPlayer.Name)
                                    {
                                        using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(ClientTexts[1])))
                                        {
                                            Game.Answers[i] = (Answer)formatter.Deserialize(stream);
                                        }
                                        serverText = "SUBMITTED";
                                        break;
                                    }
                                    i++;
                                }
                                if (serverText != "SUBMITTED")
                                {
                                    using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(ClientTexts[1])))
                                    {
                                        Game.Answers.Add((Answer)formatter.Deserialize(stream));
                                        serverText = "SUBMITTED";
                                    }
                                }
                            }
                            if (serverText != "SUBMITTED")
                            {
                                serverText = "ERROR";
                            }
                            else
                            {
                                if (Game.Answers.Count == (Game.PlayerCount + Game.BotCount))
                                {
                                    Game.Stage = 'V';
                                }
                            }
                            break;

                        /*
                         * Submits the players vote
                         * EXPECTS:
                         * [0]Command
                         * [1]Base64 Vote
                         * 
                         * RESPONSE:
                         * SUBMITTED
                         * ERROR 
                         * 
                         */
                        case "VOTE":
                            if (ClientTexts.Length != 2)
                            {
                                serverText = "Usage: VOTE Base64Vote";
                            }
                            else
                            {
                                if (Game.Votes.Count == Game.PlayerCount && Game.Votes[0].Choice.BlackCard != Game.GameSet.BlackCards[Game.GameSet.BlackCardIndex[Game.CurrentBlackCard]])
                                {
                                    Game.Winners = new List<Answer>();
                                }
                                else if (Game.Winners == null)
                                {
                                    Game.Winners = new List<Answer>();
                                }
                                int i = 0;
                                foreach (Vote vote in Game.Votes)
                                {
                                    if (vote.Voter.Name == thisPlayer.Name)
                                    {
                                        byte[] voteIn = Convert.FromBase64String(ClientTexts[1]);
                                        using (MemoryStream stream = new MemoryStream(voteIn))
                                        {
                                            Game.Votes[i] = (Vote)formatter.Deserialize(stream);
                                        }
                                        serverText = "SUBMITTED";
                                        break;
                                    }
                                    i++;
                                }
                                if (serverText != "SUBMITTED")
                                {
                                    byte[] test = Convert.FromBase64String(ClientTexts[1]);
                                    using (MemoryStream stream = new MemoryStream(test))
                                    {
                                        Game.Votes.Add((Vote)formatter.Deserialize(stream));
                                        serverText = "SUBMITTED";
                                    }
                                }
                            }
                            if (serverText != "SUBMITTED")
                            {
                                serverText = "ERROR";
                            }
                            else
                            {
                                if (Game.Votes.Count == Game.PlayerCount)
                                {
                                    Game.Winners = new List<Answer>();
                                    Dictionary<string, int> results = new Dictionary<string, int>();
                                    foreach (Answer answer in Game.Answers)
                                    {
                                        results.Add(answer.Submitter.Name + "/" + answer.Text, 0);
                                    }
                                    foreach (Vote vote in Game.Votes)
                                    {
                                        results[vote.Choice.Submitter.Name + "/" + vote.Choice.Text]++;
                                    }
                                    List<KeyValuePair<string, int>> myList = results.ToList();
                                    myList.Sort(delegate (KeyValuePair<string, int> pair1, KeyValuePair<string, int> pair2)
                                    {
                                        return pair2.Value.CompareTo(pair1.Value);
                                    });
                                    int winningVotes = myList[0].Value;
                                    foreach (KeyValuePair<string, int> answer in myList)
                                    {
                                        if (answer.Value == winningVotes)
                                        {
                                            string[] winningAnswer = answer.Key.Split('/');
                                            foreach (Answer winner in Game.Answers)
                                            {
                                                if (winner.Submitter.Name == winningAnswer[0] && winner.Text == winningAnswer[1])
                                                {
                                                    Game.Winners.Add(winner);
                                                    foreach (Player player in Game.Players)
                                                    {
                                                        if (player.Name == winner.Submitter.Name)
                                                        {
                                                            player.Score++;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    Game.NextRound();
                                }
                            }
                            break;

                        /*
                         * Updates the winners of the last round.
                         * 
                         * EXPECTS:
                         * [0]Command
                         * 
                         * RETURNS:
                         * [Binary Representation of Game.Winners]
                         */
                        case "GETWINNER":
                            serverText = Convert.ToBase64String(Game.ExportWinners());
                            break;

                        /*
                         * Performs a Special Action (e.g. Rebooting the Universe)
                         * EXPECTS:
                         * [0]Command
                         * [1]SpecialAbility
                         * 
                         * RESPONSE:
                         * SUCCESS
                         * FAILURE
                         * 
                         */
                        case "SPECIAL":
                            if (ClientTexts.Length != 2)
                            {
                                serverText = "Usage: SPECIAL Action";
                            }
                            else
                            {
                                serverText = "NOT IMPLEMENTED YET!";
                            }
                            break;

                        /*
                         * Performs a Cheating Action 
                         * EXPECTS:
                         * [0]Command
                         * [1]SpecialAbility
                         * 
                         * RESPONSE:
                         * SUCCESS
                         * FAILURE
                         * 
                         */
                        case "CHEAT":
                            if (ClientTexts.Length != 2)
                            {
                                serverText = "Usage: CHEAT Action";
                            }
                            else
                            {
                                serverText = "NOT IMPLEMENTED YET!";
                            }
                            break;

                        /*
                         * Exits the game
                         * EXPECTS:
                         * [0]Command
                         */
                        case "EXIT":
                            goto drop;

                        //Reject HTTP Connections
                        case "GET":
                            goto drop;

                        // Catch All
                        default:
                            serverText = "Shut up meg!";
                            break;
                    }

                    serverText = serverText + Environment.NewLine;
                    buffer = encoder.GetBytes(serverText);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    Log.Add(ThreadName + ": > " + serverText.Substring(0, serverText.IndexOf(Environment.NewLine)));
                }
            }
            drop:
            tcpClient.Close();
            Log.Add(ThreadName + ": > Connection Lost!");
        }

    }
}