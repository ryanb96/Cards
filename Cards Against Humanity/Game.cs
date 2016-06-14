using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Cards_Against_Humanity
{
    [Serializable]
    /// <summary>
    /// A Game of Cards Against Humanity.
    /// </summary>
    public class Game
    {
        public bool Playable;
        /* Game Type
         * L = Local  (No Network)
         * H = Host   (Hosting Game)
         * C = Client (Joining Game)
         */
        [NonSerialized]
        public char GameType;

        //Networking
        [NonSerialized]
        public ServerNetworking HostNetwork;
        [NonSerialized]
        public List<ClientNetworking> LocalPlayers;
        [NonSerialized]
        public bool NetworkUp;

        //Settings
        public bool Cheats;
        public int CardsPerUser;
        public int Rounds;
        public bool NeverHaveI;
        public bool RebootingTheUniverse;
        public int PlayerCount;
        public int BotCount;
        public Player[] Players;
        public List<CardSet> CardSets;

        //Progress
        public char Stage;
        public CardSet GameSet;
        public int CurrentBlackCard;
        public int CurrentWhiteCard;
        public int Round;

        public List<Answer> Answers;
        public List<Vote> Votes;
        public List<Answer> Winners;

        /// <summary>
        /// Initalise a game including the global settings.
        /// </summary>
        /// <param name="PlayerNames">List of player names.</param>
        public Game(char Type, List<string> PlayerNames)
        {
            GameType = Type;
            // Settings
            Round = 0;
            Rounds = Properties.Settings.Default.Rounds;
            CardsPerUser = Properties.Settings.Default.Cards;
            NeverHaveI = Properties.Settings.Default.NeverHaveI;
            RebootingTheUniverse = Properties.Settings.Default.Rebooting;
            Cheats = Properties.Settings.Default.Cheats;

            // Deal First Hand
            PrepareDeck();
            List<Tuple<int, Card>> hands = new List<Tuple<int, Card>>();
            try
            {
                hands = Dealer.Deal(GameSet, PlayerNames.Count, CardsPerUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, game not started. " + ex.Message, "Game Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Playable = false;
                return;
            }

            // Setup Players
            Players = new Player[PlayerNames.Count];
            int player = 0;
            foreach (string playerName in PlayerNames)
            { 
                List<Card> hand = new List<Card>();
                foreach (Tuple<int, Card> card in hands.GetRange(player * CardsPerUser, CardsPerUser))
                {
                    hand.Add(card.Item2);
                }
                Players[player] = new Player(playerName, hand);
                if (Players[player].Name.ToLower().StartsWith("[bot]"))
                {
                    BotCount++;
                }
                else
                {
                    PlayerCount++;
                }
                player++;
            }

            //get game ready
            CurrentBlackCard = 0;

            if (Type == 'L' || Type == 'H')
            {
                try
                {
                    HostNetwork = new ServerNetworking(this);
                    HostNetwork.Start();
                    LocalPlayers = new List<ClientNetworking>();
                    Stage = 'W';
                    Playable = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to start game. " + ex.Message, "Game Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Playable = false;
                }
            }

            NextRound();
        }

        /// <summary>
        /// Shutdown the currently hosted game.
        /// </summary>
        /// <returns>If the game was stopped.</returns>
        public bool Stop()
        {
            Playable = false;
            HostNetwork.Stop();
            foreach (ClientNetworking playerNetwork in LocalPlayers)
            {
                playerNetwork.NextCommand = "EXIT";
                playerNetwork.NewCommand = true;
            }
            return true;
        }

        /// <summary>
        /// Join a Remote Game.
        /// </summary>
        /// <param name="Address">Hostname, IPv4 or IPv6 Address of game.</param>
        /// <param name="Port">TCP Port of Game.</param>
        /// <returns>If Game was joined.</returns>
        public bool Join(string Address, int Port)
        {
            StartJoin:
            LocalPlayers = new List<ClientNetworking>();
            ClientNetworking playerNetwork;
            try
            {
                playerNetwork = new ClientNetworking(this, Players[0], Address, Port);
            }
            catch
            {
                MessageBox.Show("Failed to Connect", "Game Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            LocalPlayers.Add(playerNetwork);
            playerNetwork.NextCommand = "HELLO";
            playerNetwork.NewCommand = true;
            while (!playerNetwork.NewResponse)
            {
                Application.DoEvents();
            }
            string ServerHello = playerNetwork.LastResponse;
            playerNetwork.NewResponse = false;
            string[] ServerDetails = ServerHello.Split(' ');

            if (ServerDetails[3] != "0")
            {
                playerNetwork.NextCommand = "JOIN " + Players[0].Name.Replace(' ', '_') + " " + Program.SessionKey;
                playerNetwork.NewCommand = true;
                while (!playerNetwork.NewResponse)
                {
                    Application.DoEvents();
                }
                string ServerResponse = playerNetwork.LastResponse;
                playerNetwork.NewResponse = false;
                if (ServerResponse == "SUCCESS")
                {
                    Program.CurrentPlayer = LocalPlayers[0];
                    goto SetupGame;
                }
                else
                {
                    string reason = "Unknown Error";
                    if (ServerResponse.StartsWith("FAILED "))
                    {
                        switch (ServerResponse.Split(' ')[1])
                        {
                            case "NoSpace":
                                if (MessageBox.Show("Failed  to Join Game: No space in game.", "Join Failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry)
                                {
                                    goto StartJoin;
                                }
                                else
                                {
                                    playerNetwork.NextCommand = "EXIT";
                                    playerNetwork.NewCommand = true;
                                    while (!playerNetwork.NewResponse)
                                    {
                                        Application.DoEvents();
                                    }
                                    return false;
                                }
                            case "BadPass":
                                reason = "Name in use or Password incorrect";
                                break;
                        }
                    }
                    MessageBox.Show("Failed to Join Game: " + reason, "Failed to Join", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    playerNetwork.NextCommand = "EXIT";
                    playerNetwork.NewCommand = true;
                    while (!playerNetwork.NewResponse)
                    {
                        Application.DoEvents();
                    }
                    return false;
                }
                SetupGame:
                Program.CurrentPlayer.NextCommand = "GETRULES";
                Program.CurrentPlayer.NewCommand = true;
                while (!Program.CurrentPlayer.NewResponse)
                {
                    Application.DoEvents();
                }
                string[] responses = Program.CurrentPlayer.LastResponse.Split(' ');
                Program.CurrentPlayer.NewResponse = false;
                foreach (string setting in responses)
                {
                    string[] settingPair = setting.Split(':');
                    switch (settingPair[0])
                    {
                        case "CardsPerPlayer":
                            CardsPerUser = Convert.ToInt32(settingPair[1]);
                            break;
                        case "Rounds":
                            Rounds = Convert.ToInt32(settingPair[1]);
                            break;
                        case "NeverHaveI":
                            NeverHaveI = Convert.ToBoolean(settingPair[1]);
                            break;
                        case "RebootingTheUniverse":
                            RebootingTheUniverse = Convert.ToBoolean(settingPair[1]);
                            break;
                        case "Cheats":
                            Cheats = Convert.ToBoolean(settingPair[1]);
                            break;
                    }
                }

                Program.CurrentPlayer.NextCommand = "GETCARDS";
                Program.CurrentPlayer.NewCommand = true;
                while (!Program.CurrentPlayer.NewResponse)
                {
                    Application.DoEvents();
                }
                string response = Program.CurrentPlayer.LastResponse;
                Program.CurrentPlayer.NewResponse = false;
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(response)))
                {
                    GameSet = (CardSet)formatter.Deserialize(stream);
                }
                Playable = true;
                return true;
            }
            else
            {
                if (MessageBox.Show("Failed to Join Game: No space in game.", "Join Failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry)
                {
                    goto StartJoin;
                }
                else
                {
                    playerNetwork.NextCommand = "EXIT";
                    playerNetwork.NewCommand = true;
                    while (!playerNetwork.NewResponse)
                    {
                        Application.DoEvents();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Prepare the Card Deck for the game.
        /// </summary>
        /// <returns>If the deck was successfully prepared.</returns>
        private bool PrepareDeck()
        {
            CardSets = new List<CardSet>();
            int sets = 0;
            List<Guid> MergeList = new List<Guid>();
            foreach (string[] set in Dealer.GetCardSets())
            {
                // GUID, Name, Version, Path, Hash Status
                foreach (string enabledSet in Properties.Settings.Default.CardSets)
                {
                    string enabledGuid = enabledSet.Substring(enabledSet.IndexOf('_') + 1);
                    if (set[0] == enabledGuid && set[4] == "OK")
                    {
                        CardSets.Add(new CardSet(new Guid(set[0])));
                        if (sets == 0)
                        {
                            GameSet = new CardSet(new Guid(set[0]));
                            sets++;
                        }
                        else
                        {
                            MergeList.Add(new Guid(set[0]));
                        }
                        break;
                    }
                }
            }
            if (sets > 0)
            {
                GameSet.Merge(MergeList);
            }

            if (GameSet.BlackCards.Count > 0 && GameSet.WhiteCards.Count > 0)
            {
                CurrentWhiteCard = ((PlayerCount + BotCount) * CardsPerUser) + 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NextRound()
        {
            if (Round + 1 > Rounds)
            {
                Stage = 'E';
                return false;
            }
            else
            {
                if (Stage != 'W')
                { 
                    Stage = 'P';
                }
                if (Round == 0)
                {
                    Winners = new List<Answer>();
                }
                Round++;
                if (CurrentBlackCard + 1 > GameSet.BlackCards.Count)
                {
                    CurrentBlackCard = 1;
                }
                else
                {
                    CurrentBlackCard++;
                }
                if (Round > 1)
                {
                    foreach (Answer answer in Answers)
                    {
                        NextWhiteCard();
                        string remove = answer.WhiteCard.ID;
                        int i = 0;
                        foreach (Card card in answer.Submitter.WhiteCards)
                        {
                            if (card.ID == remove)
                            {
                                answer.Submitter.WhiteCards.RemoveAt(i);
                                break;
                            }
                            i++;
                        }
                        answer.Submitter.WhiteCards.Add(GameSet.WhiteCards[GameSet.WhiteCardIndex[CurrentWhiteCard]]);
                        i = 0;
                        foreach (Player player in Program.CurrentGame.Players)
                        {
                            if (player.Name == answer.Submitter.Name)
                            {
                                Program.CurrentGame.Players[i].WhiteCards = answer.Submitter.WhiteCards;
                                break;
                            }
                            i++;
                        }

                        if (answer.BlackCard.Needs == 2)
                        {
                            NextWhiteCard();
                            remove = answer.WhiteCard2.ID;
                            i = 0;
                            foreach (Card card in answer.Submitter.WhiteCards)
                            {
                                if (card.ID == remove)
                                {
                                    answer.Submitter.WhiteCards.RemoveAt(i);
                                    break;
                                }
                                i++;
                            }
                            answer.Submitter.WhiteCards.Add(GameSet.WhiteCards[GameSet.WhiteCardIndex[CurrentWhiteCard]]);
                            i = 0;
                            foreach (Player player in Program.CurrentGame.Players)
                            {
                                if (player.Name == answer.Submitter.Name)
                                {
                                    Program.CurrentGame.Players[i].WhiteCards = answer.Submitter.WhiteCards;
                                    break;
                                }
                                i++;
                            }
                        }
                    }
                }
                Answers = new List<Answer>();

                foreach (Player player in Players)
                {
                    if (player.IsBot)
                    {
                        Answer botAnswer;
                        if (GameSet.BlackCards[GameSet.BlackCardIndex[CurrentBlackCard]].Needs == 1)
                        {
                            botAnswer = new Answer(player, GameSet.BlackCards[GameSet.BlackCardIndex[CurrentBlackCard]], player.WhiteCards[0]);
                        }
                        else
                        {
                            botAnswer = new Answer(player, GameSet.BlackCards[GameSet.BlackCardIndex[CurrentBlackCard]], player.WhiteCards[0], player.WhiteCards[1]);
                        }
                        Answers.Add(botAnswer);
                        player.Submitted = Round;
                    }
                }

                Votes = new List<Vote>();
                return true;
            }
        }

        private void NextWhiteCard()
        {
            // Prevent Double Dealing.
            bool CardUsed = true;
            while (CardUsed)
            {
                bool found = false;
                foreach (Player player in Players)
                {
                    Card whiteCard = GameSet.WhiteCards[GameSet.WhiteCardIndex[CurrentWhiteCard]];
                    foreach (Card card in player.WhiteCards)
                    {
                        if (card.ID == whiteCard.ID)
                        {
                            found = true;
                            if (CurrentWhiteCard + 1 > GameSet.WhiteCardCount)
                            {
                                CurrentWhiteCard = 1;
                            }
                            else
                            {
                                CurrentWhiteCard++;
                            }
                            break;
                        }
                    }
                    if (found) break;
                }
                if (!found) CardUsed = false;
            }
        }

        public byte[] ExportGameSet()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, GameSet);
                return stream.ToArray();
            }
        }

        public byte[] ExportAnswers()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, Answers);
                return stream.ToArray();
            }
        }

        public byte[] ExportPlayers()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, Players);
                return stream.ToArray();
            }
        }

    }
}