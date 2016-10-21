using JoePitt.Cards.Exceptions;
using NATUPNPLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace JoePitt.Cards.Net
{
    /// <summary>
    /// Provides Server Networking for a new game.
    /// </summary>
    internal class ServerNetworking : IDisposable
    {
        /// <summary>
        /// The Default port for games of Cards.
        /// </summary>
        public const int DefaultPort = 60069;
        /// <summary>
        /// The Port this game is being played on.
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// The IP:Port combinations that can be used to connect to this game.
        /// </summary>
        public List<string> ConnectionStrings { get; private set; }
        /// <summary>
        /// The TCP Port used by UPNP for external IPv4 Traffic.
        /// </summary>
        private Game Game;
        private int IPV4ExternalPort;
        private IPAddress IPV4ExternalIPAddress;
        private TcpListener tcpListener;
        private Thread listenThread;
        private List<Thread> ClientThreads;
        private List<TcpClient> ClientTcpClients;
        private bool ListenerUp;
        private BinaryFormatter formatter;

        /// <summary>
        /// Initialise networking for the specified Game.
        /// </summary>
        /// <param name="game">The Game the networking is for.</param>
        public ServerNetworking(Game game)
        {
            Game = game;
            Port = DefaultPort;
            ConnectionStrings = new List<string>();
            ClientThreads = new List<Thread>();
            ClientTcpClients = new List<TcpClient>();
            formatter = new BinaryFormatter();
            IPV4ExternalPort = DefaultPort;
            if (!DiscoverLocalPort())
            {
                throw new NoFreePortsException();
            }
            DiscoverLocalIPV4Addresses();
            if (Game.GameType != 'L')
            {
                if (DiscoverExternalIPV4Address())
                {
                    if (SetupIPV4UPNP())
                    {
                        ConnectionStrings.Add(IPV4ExternalIPAddress.ToString() + ":" + IPV4ExternalPort + " (Internet)");
                    }
                    else
                    {
                        ConnectionStrings.Add(IPV4ExternalIPAddress.ToString() + ":" + Port + " (Internet - Manual Forward Needed!)");
                    }
                }
            }
            DiscoverIPV6Addresses();
            ConnectionStrings.Add("127.0.0.1:" + Port + " (Local Machine)");
            listenThread = new Thread(Listen);
            listenThread.Name = "Cards Listener";
            listenThread.Start();
            Game.NetworkUp = true;
        }

        private bool DiscoverLocalPort()
        {
            Port = DefaultPort;
            bool usablePort = false;
            while (!usablePort)
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();
                foreach (TcpConnectionInformation tcpConnection in tcpConnections)
                {
                    if (tcpConnection.LocalEndPoint.Port == Port)
                    {
                        Port++;
                        if (Port > 61069)
                        {
                            usablePort = false;
                            return usablePort;
                        }
                        break;
                    }
                }
                usablePort = true;
            }
            return usablePort;
        }

        private void DiscoverLocalIPV4Addresses()
        {
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && Interface.GetIPProperties().GatewayAddresses.Count > 0)
                        {
                            ConnectionStrings.Add(ip.Address.ToString() + ":" + Port);
                        }
                    }
                }
            }
        }

        private bool DiscoverExternalIPV4Address()
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri("http://ifconfig.me"));
            request.UserAgent = "curl"; // this simulate curl linux command
            request.Method = "GET";
            try
            {
                string externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
                IPV4ExternalIPAddress = IPAddress.Parse(externalIP);
                return true;
            }
            catch (WebException)
            {
                MessageBox.Show("Unable to reach checkip.dyndns.org to get IP Address", "Error Detecting External IPv4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Unexpected format received from checkip.dyndns.org", "Error Detecting External IPv4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private bool SetupIPV4UPNP()
        {
            UPnPNAT upnpnat = new UPnPNAT();
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            if (mappings == null)
            {
                return false;
            }
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                            Interface.GetIPProperties().GatewayAddresses.Count > 0)
                        {
                            foreach (GatewayIPAddressInformation thisGateway in Interface.GetIPProperties().GatewayAddresses)
                            {
                                if (thisGateway.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                retry:
                                    try
                                    {
                                        mappings.Add(IPV4ExternalPort, "TCP", Port, ip.Address.ToString(), true, "Cards-IPv4");
                                        return true;
                                    }
                                    catch (NullReferenceException)
                                    {
                                        if (IPV4ExternalPort < 60074)
                                        {
                                            IPV4ExternalPort++;
                                            goto retry;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return false;
        }

        private void DiscoverIPV6Addresses()
        {
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6 &&
                            Interface.GetIPProperties().GatewayAddresses.Count > 0)
                        {
                            if (Game.GameType != 'L')
                            {
                                if (!ip.Address.IsIPv6LinkLocal && !ip.Address.IsIPv6SiteLocal)
                                {
                                    if (!SetupIPV6UPNP(ip.Address))
                                    {
                                        ConnectionStrings.Add(ip.Address.ToString() + ":" + Port + " (Internet - Manual Forward Required!)");
                                    }
                                }
                                else if (ip.Address.IsIPv6SiteLocal)
                                {
                                    ConnectionStrings.Add(ip.Address.ToString() + ":" + Port + " (Local Network)");
                                }
                            }
                            else
                            {
                                ConnectionStrings.Add(ip.Address.ToString() + ":" + Port);
                            }
                        }
                    }
                }
            }
        }

        private bool SetupIPV6UPNP(IPAddress LocalIP)
        {
            int IPV6ExternalPort = Port;
            UPnPNAT upnpnat = new UPnPNAT();
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            if (mappings == null)
            {
                return false;
            }
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.ToString() == LocalIP.ToString() &&
                            Interface.GetIPProperties().GatewayAddresses.Count > 0)
                        {
                            GatewayIPAddressInformation Gateway = Interface.GetIPProperties().GatewayAddresses[0];
                            if (Gateway.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                            retry:
                                try
                                {
                                    mappings.Add(IPV6ExternalPort, "TCP", Port, LocalIP.ToString(), true, "Cards-IPv6");
                                    ConnectionStrings.Add(LocalIP.ToString() + ":" + IPV6ExternalPort + " (Internet)");
                                    return true;
                                }
                                catch (System.Runtime.InteropServices.COMException)
                                {
                                    if (IPV6ExternalPort < 60074)
                                    {
                                        IPV6ExternalPort++;
                                        goto retry;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void Listen()
        {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            try
            {
                tcpListener.Start();
                ListenerUp = true;
            }
            catch (SocketException)
            {
                MessageBox.Show("Unable to start Game Networking, Restarting Cards.", "Unable to Bind Networking", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Restart();
            }

            while (ListenerUp)
            {
                try
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        ClientTcpClients.Add(client);
                        Thread clientThread = new Thread(new ParameterizedThreadStart(ClientLink));
                        clientThread.Name = "ClientLink_" + ClientTcpClients.Count;
                        ClientThreads.Add(clientThread);
                        clientThread.Start(client);
                    }
                    else
                    {
                        Thread.Sleep(3000);
                    }
                }
                catch (SocketException) { }
            }
        }

        private void ClientLink(object myClientObj)
        {
            TcpClient myClient = (TcpClient)myClientObj;
            Player thisPlayer = new Player("FAKE", new List<Card>());

            while (myClient.Connected)
            {
                string clientText = SharedNetworking.ReceiveString(myClient);
                if (string.IsNullOrEmpty(clientText))
                {
                    goto drop;
                }
                string serverText = "";
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
                        serverText = HandleHello();
                        break;

                    /* Try and (Re)Join the game.
                     * EXPECTS:
                     * [0]Command
                     * [1]Username 
                     * [2]Hash of Password
                     * 
                     * RESPONSE:
                     * SUCCESS
                     * FAILED NoSpace
                     */
                    case "JOIN":
                        Player newPlayer = HandleJoin(thisPlayer, ClientTexts);
                        if (newPlayer.Name.StartsWith("FAILED "))
                        {
                            serverText = newPlayer.Name;
                        }
                        else
                        {
                            thisPlayer = newPlayer;
                            serverText = "SUCCESS";
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
                        serverText = HandleGetCards();
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
                        serverText = HandleGetRules();
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
                        serverText = HandleGameUpdate();
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
                        serverText = HandleGameScores();
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
                        serverText = HandleMyCards(thisPlayer);
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
                        serverText = HandleNeed(thisPlayer, ClientTexts);
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
                        serverText = HandleSubmit(thisPlayer, ClientTexts);
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
                        serverText = HandleVote(thisPlayer, ClientTexts);
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
                        serverText = HandleGetWinner();
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
                        serverText = HandleSpecial(thisPlayer, ClientTexts);
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
                        serverText = HandleCheat(thisPlayer, ClientTexts);
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
            retry:
                if (!SharedNetworking.Send(myClient, serverText))
                {
                    if (MessageBox.Show("A network error has occurred while sending the last response", "Network Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        goto retry;
                    }
                    else
                    {
                        Application.Restart();
                    }
                }
            }
        drop:
            myClient.Close();
        }

        private string HandleHello()
        {
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
                cardSets = cardSets + set.CardSetGuid.ToString() + "_" + set.Version + ":";
            }
            cardSets = cardSets.Substring(0, cardSets.Length - 1);

            if ((maxPlayers_H - players_H) == 0 && Game.Stage == 'W')
            {
                Game.Stage = 'P';
            }
            return Game.Players[0].Name.Replace(' ', '_') + " " + players_H + " " + (maxPlayers_H - players_H) + " " + cardSets;
        }

        private Player HandleJoin(Player thisPlayer, string[] ClientTexts)
        {
            int playersJoin = 0;
            int maxPlayersJoin = Game.PlayerCount;
            foreach (Player player in Game.Players)
            {
                if (player.Name != "FREESLOT" && !player.Name.ToLower().StartsWith("[bot]"))
                {
                    playersJoin++;
                }
            }

            if (ClientTexts.Length == 3)
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
                            break;
                        }
                        else
                        {
                            thisPlayer.Name = "FAILED BadPass";
                            break;
                        }
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
                            found = true;
                            playersJoin++;
                            thisPlayer = player;
                            break;
                        }
                    }
                }
            }
            if ((maxPlayersJoin - playersJoin) == 0 && Game.Stage == 'W')
            {
                Game.Stage = 'P';
            }
            return thisPlayer;
        }

        private string HandleGetCards()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, Game.GameSet);
                byte[] cardsArray = stream.ToArray();
                return Convert.ToBase64String(cardsArray);
            }
        }

        private string HandleGetRules()
        {
            string serverText = "CardsPerPlayer:" + Game.CardsPerUser;
            serverText = serverText + " Rounds:" + Game.Rounds;
            serverText = serverText + " NeverHaveI" + Game.NeverHaveI;
            serverText = serverText + " RebootingTheUniverse" + Game.RebootingTheUniverse;
            if (Game.Cheats)
            {
                serverText = serverText + " Cheats:true";
            }
            return serverText;
        }

        private string HandleGameUpdate()
        {
            switch (Game.Stage)
            {
                case 'W':
                    return "WAITING";
                case 'P':
                    return "PLAYING " + Convert.ToBase64String(Game.GameSet.BlackCards[Game.GameSet.BlackCardIndex[Game.CurrentBlackCard]].ToArray()) + " " + Game.Round;
                case 'V':
                    return "VOTING " + Convert.ToBase64String(Game.AnswersToByteArray());
                case 'E':
                    return "END";
                default:
                    return "X";
            }
        }

        private string HandleGameScores()
        {
            string serverText = "";
            foreach (Player player in Game.Players)
            {
                serverText = serverText + player.Name.Replace(' ', '_') + ":" + player.Score + ",";
            }
            serverText = serverText.Substring(0, serverText.Length - 1);
            return serverText;
        }

        private string HandleMyCards(Player thisPlayer)
        {
            if (thisPlayer.Name == "FAKE")
            {
                return "JoinFirst";
            }
            else
            {

                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, thisPlayer.WhiteCards);
                    byte[] cardsArray = stream.ToArray();
                    return Convert.ToBase64String(cardsArray);
                }
            }
        }

        private string HandleNeed(Player thisPlayer, string[] ClientTexts)
        {
            string serverText = "";
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
                            if (answer.Submitter != null && answer.Submitter.Name == thisPlayer.Name)
                            {
                                serverText = "NO";
                            }
                        }
                        break;
                    case "VOTE":
                        serverText = "YES";
                        foreach (Vote vote in Game.Votes)
                        {
                            if (vote.Voter != null && vote.Voter.Name == thisPlayer.Name)
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
            return serverText;
        }

        private string HandleSubmit(Player thisPlayer, string[] ClientTexts)
        {
            string serverText = "";
            if (ClientTexts.Length != 2)
            {
                serverText = "Usage: SUBMIT Base64Answer";
            }
            else
            {
                int i = 0;
                foreach (Answer answer in Game.Answers)
                {
                    if (answer.Submitter.Name == thisPlayer.Name)
                    {
                        Game.Answers[i] = new Answer(Convert.FromBase64String(ClientTexts[1]));
                        serverText = "SUBMITTED";
                        break;
                    }
                    i++;
                }
                if (serverText != "SUBMITTED")
                {
                    Game.Answers.Add(new Answer(Convert.FromBase64String(ClientTexts[1])));
                    serverText = "SUBMITTED";
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
                    Game.Answers = Dealer.ShuffleAnswers(Game.Answers);
                    Game.Stage = 'V';
                }
            }
            return serverText;
        }

        private string HandleVote(Player thisPlayer, string[] ClientTexts)
        {
            string serverText = "";
            if (ClientTexts.Length != 2)
            {
                return "Usage: VOTE Base64Vote";
            }
            else
            {
                if (Game.Votes.Count == Game.PlayerCount &&
                    Game.Votes[0].Choice.BlackCard != Game.GameSet.BlackCards[Game.GameSet.BlackCardIndex[Game.CurrentBlackCard]])
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
                            stream.Position = 0;
                            Vote thisVote = (Vote)formatter.Deserialize(stream);
                            if (thisVote.Choice.BlackCard != Game.GameSet.BlackCards[Game.GameSet.BlackCardIndex[Game.CurrentBlackCard]])
                            {
                                serverText = "ERROR";
                            }
                            else
                            {
                                Game.Votes[i] = thisVote;
                                serverText = "SUBMITTED";
                            }
                        }

                        break;
                    }
                    i++;
                }
                if (serverText != "SUBMITTED")
                {
                    byte[] test = Convert.FromBase64String(ClientTexts[1]);
                    using (MemoryStream stream = new MemoryStream(test))
                    {
                        stream.Position = 0;
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
                        results.Add(answer.Submitter.Name + "/" + answer.ToString, 0);
                    }
                    foreach (Vote vote in Game.Votes)
                    {
                        results[vote.Choice.Submitter.Name + "/" + vote.Choice.ToString]++;
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
                                if (winner.Submitter.Name == winningAnswer[0] && winner.ToString == winningAnswer[1])
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
            return serverText;
        }

        private string HandleGetWinner()
        {
            return Convert.ToBase64String(Game.WinnersToByteArray());
        }

        private string HandleSpecial(Player thisPlayer, string[] ClientTexts)
        {
            string test = thisPlayer.Name + Game.Stage;
            if (ClientTexts.Length != 2)
            {
                return "Usage: SPECIAL Action" + test;
            }
            else
            {
                return "NOT IMPLEMENTED YET!" + test;
            }
        }

        private string HandleCheat(Player thisPlayer, string[] ClientTexts)
        {
            string test = thisPlayer.Name + Game.Stage;
            if (ClientTexts.Length != 2)
            {
                return "Usage: CHEAT Action" + test;
            }
            else
            {
                return "NOT IMPLEMENTED YET!" + test;
            }
        }

        public void Close()
        {
            ListenerUp = false;
            foreach (TcpClient thisClient in ClientTcpClients)
            {
                thisClient.Close();
            }
            foreach (Thread ClientThread in ClientThreads)
            {
                ClientThread.Interrupt();
            }
            Game.NetworkUp = false;
            DropIPV6UPNP();
            DropIPV4UPNP();
            Dispose();
        }

        private void DropIPV6UPNP()
        {
            foreach (string ConnectionString in ConnectionStrings)
            {
                string IPString = ConnectionString.Substring(0, ConnectionString.LastIndexOf(":"));
                int portStart = ConnectionString.LastIndexOf(":") + 1;
                string portString = ConnectionString.Substring(portStart);
                if (portString.Contains("("))
                {
                    portString = portString.Substring(0, portString.LastIndexOf(" ("));
                }
                int IPV6UPNPPort = Convert.ToInt32(portString);
                IPAddress IP = IPAddress.Parse(IPString);
                if (IP.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    if (!ConnectionString.EndsWith("!)"))
                    {
                        UPnPNAT upnpnat = new UPnPNAT();
                        IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
                        if (IPV4ExternalIPAddress != null)
                        {
                            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
                            foreach (NetworkInterface Interface in Interfaces)
                            {
                                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                    Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                {
                                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                                    {
                                        if (ip.Address.ToString() == IP.ToString() &&
                                            Interface.GetIPProperties().GatewayAddresses.Count > 0)
                                        {
                                            GatewayIPAddressInformation Gateway = Interface.GetIPProperties().GatewayAddresses[0];
                                            if (Gateway.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                            {
                                                try
                                                {
                                                    mappings.Remove(IPV6UPNPPort, "TCP");
                                                }
                                                catch (System.Runtime.InteropServices.COMException ex)
                                                {
                                                    MessageBox.Show(ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DropIPV4UPNP()
        {
            UPnPNAT upnpnat = new UPnPNAT();
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            if (IPV4ExternalIPAddress != null)
            {
                NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in Interfaces)
                {
                    if (Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                                Interface.GetIPProperties().GatewayAddresses.Count > 0)
                            {
                                GatewayIPAddressInformation Gateway = Interface.GetIPProperties().GatewayAddresses[0];
                                if (Gateway.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    try
                                    {
                                        mappings.Remove(IPV4ExternalPort, "TCP");
                                    }
                                    catch (System.Runtime.InteropServices.COMException ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ConnectionStrings.Clear();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ServerNetworking() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
