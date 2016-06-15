using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;
using System.Security.Principal;
using System.Management;
using System.Linq;

namespace JoePitt.Cards
{
    /// <summary>
    /// Carries out Card Set Management and Dealer Functions.
    /// </summary>
    public class Dealer
    {
        static public string IDPlayer()
        {
            string targetUser = WindowsIdentity.GetCurrent().Name;
            try
            {
                return UserPrincipal.Current.DisplayName;
            }
            catch
            {
                ManagementObjectSearcher usersSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_UserAccount");
                ManagementObjectCollection users = usersSearcher.Get();

                var localUsers = users.Cast<ManagementObject>().Where(
                    u => (bool)u["LocalAccount"] == true &&
                         (bool)u["Disabled"] == false &&
                         (bool)u["Lockout"] == false &&
                         int.Parse(u["SIDType"].ToString()) == 1 &&
                         u["Name"].ToString() != "HomeGroupUser$");

                foreach (ManagementObject user in users)
                {
                    string thisUser = user["Domain"].ToString() + "\\" + user["Name"].ToString();
                    if (thisUser == targetUser)
                    {
                        return user["FullName"].ToString();
                    }
                }
            }
            return targetUser.Split('\\')[1];
        }

        /// <summary>
        /// Get a list of installed Card Sets.
        /// </summary>
        /// <returns>Details of Installed Card Sets</returns>
        static public List<string[]> GetCardSets()
        {
            Console.WriteLine(Application.StartupPath + "\\Resources\\CardSets");
            List<string[]> cardSets = new List<string[]>();
            // Find Card Sets
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            if (!Directory.Exists(path)) Directory.CreateDirectory(path); 
            string[] cardSetFiles = Directory.GetFiles(path, "*.cahc");
            foreach (string cardSet in cardSetFiles)
            {
                // Try to Open Card Set
                XmlDocument cardSetDoc = new XmlDocument();
                try
                {
                    cardSetDoc.Load(cardSet);
                }
                catch
                {
                    MessageBox.Show(cardSet + " is not a valid Card Set", "Bad Card Set", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
                // Load Card Set Details
                XmlElement cardSetDetail = (XmlElement)cardSetDoc.GetElementsByTagName("CardSet")[0];
                // GUID, Name, Version, Path, Hash Status
                string[] packInfo = new string[5];
                packInfo[0] = cardSetDetail.GetAttribute("GUID");
                packInfo[1] = cardSetDetail.GetAttribute("Name");
                packInfo[2] = cardSetDetail.GetAttribute("Version");
                packInfo[3] = cardSet;
                // Verify Hash
                XmlElement xmlBlackCards = (XmlElement)cardSetDoc.GetElementsByTagName("BlackCards")[0];
                XmlElement xmlWhiteCards = (XmlElement)cardSetDoc.GetElementsByTagName("WhiteCards")[0];
                SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                byte[] allCards = Encoding.Default.GetBytes(xmlBlackCards.InnerXml + xmlWhiteCards.InnerXml);
                byte[] hash = hasher.ComputeHash(allCards);
                string computedHash = Convert.ToBase64String(hash);

                if (cardSetDetail.GetAttribute("Hash") == computedHash)
                {
                    packInfo[4] = "OK";
                    cardSets.Add(packInfo);
                }
                else
                {
                    packInfo[4] = "Corrupt";
                    MessageBox.Show(cardSetDetail.GetAttribute("Name") + " (" + cardSetDetail.GetAttribute("Version") + ") has failed it's integrity check and not been loaded, Please reinstall it.", "Card Set Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return cardSets;
        }

        /// <summary>
        /// Install a Card Set.
        /// </summary>
        /// <param name="file">The path to the Card Set to be installed.</param>
        /// <returns>If the Card Set was installed.</returns>
        static public bool InstallCardSet(string CardSetFile)
        {
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            bool installed = false;
            bool skipped = false;
            List<string[]> installedSets = GetCardSets();

            XmlDocument cardSetDoc = new XmlDocument();
            try
            {
                cardSetDoc.Load(CardSetFile);
            }
            catch
            {
                MessageBox.Show(CardSetFile + " is not a valid Card Set", "Bad Card Set", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return installed;
            }
            XmlElement cardSetInfo = (XmlElement)cardSetDoc.GetElementsByTagName("CardSet")[0];
            /*
             * [0] = GUID
             * [1] = Name
             * [2] = Version
             * [3] = file
             * [4] = OK/Corrupt
             */
            foreach (string[] installedSet in installedSets)
            {
                // If GUID found.
                if (installedSet[0] == cardSetInfo.GetAttribute("GUID"))
                {
                    // If GUID found and version matches.
                    if (installedSet[2] == cardSetInfo.GetAttribute("Version"))
                    {
                        if (MessageBox.Show(cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + ") is already installed. Reinstall?", "Reinstall Pack?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Delete(installedSet[3]);
                            File.Copy(CardSetFile, installedSet[3]);
                            installed = true;
                            break;
                        }
                        else
                        {
                            skipped = true;
                            break;
                        }
                    }
                    // If GUID found and version doesn't match.
                    string[] oldVersion = installedSet[2].Split('.');
                    int oldMajVersion = Convert.ToInt32(oldVersion[0]);
                    int oldMinVersion = Convert.ToInt32(oldVersion[1]);
                    string[] newVersion = cardSetInfo.GetAttribute("Version").Split('.');
                    int newMajVersion = Convert.ToInt32(newVersion[0]);
                    int newMinVersion = Convert.ToInt32(newVersion[1]);

                    if (newMajVersion > oldMajVersion)
                    {
                        if (MessageBox.Show(installedSet[1] + " (" + installedSet[2] + ") will be upgraded to " + cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + "). Continue?", "Upgrade Card Pack?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Delete(installedSet[3]);
                            File.Copy(CardSetFile, installedSet[3]);
                            installed = true;
                            break;
                        }
                        else
                        {
                            skipped = true;
                            break;
                        }
                    }
                    else if (newMajVersion == oldMajVersion)
                    {
                        if (newMinVersion > oldMinVersion)
                        {
                            if (MessageBox.Show(installedSet[1] + " (" + installedSet[2] + ") will be upgraded to " + cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + "). Continue?", "Upgrade Card Pack?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                File.Delete(installedSet[3]);
                                File.Copy(CardSetFile, installedSet[3]);
                                installed = true;
                                break;
                            }
                            else
                            {
                                skipped = true;
                                break;
                            }
                        }
                        else
                        {
                            if (MessageBox.Show(installedSet[1] + " (" + installedSet[2] + ") will be DOWNGRADED to " + cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + "). Continue?", "Downgrade Card Pack?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                File.Delete(installedSet[3]);
                                File.Copy(CardSetFile, installedSet[3]);
                                installed = true;
                                break;
                            }
                            else
                            {
                                skipped = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (MessageBox.Show(installedSet[1] + " (" + installedSet[2] + ") will be DOWNGRADED to " + cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + "). Continue?", "Downgrade Card Pack?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            File.Delete(installedSet[3]);
                            File.Copy(CardSetFile, installedSet[3]);
                            installed = true;
                            break;
                        }
                        else
                        {
                            skipped = true;
                            break;
                        }
                    }
                }
            }
            if (!installed && !skipped)
            {
                if (MessageBox.Show(cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + ") will be installed. Continue?", "Install Card Pack?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (File.Exists(path + "\\" + cardSetInfo.GetAttribute("Name").Replace(' ', '-') + "_" + cardSetInfo.GetAttribute("Version") + "_" + cardSetInfo.GetAttribute("GUID") + ".cahc"))
                    { File.Delete(path + "\\" + cardSetInfo.GetAttribute("Name").Replace(' ', '-') + "_" + cardSetInfo.GetAttribute("Version") + "_" + cardSetInfo.GetAttribute("GUID") + ".cahc"); }
                    File.Copy(CardSetFile, path + "\\" + cardSetInfo.GetAttribute("Name").Replace(' ', '-') + "_" + cardSetInfo.GetAttribute("Version") + "_" + cardSetInfo.GetAttribute("GUID") + ".cahc");
                    installed = true;
                }
            }

            if (installed)
            { MessageBox.Show(cardSetInfo.GetAttribute("Name") + " (" + cardSetInfo.GetAttribute("Version") + ") has been installed.", "Card Set Installed", MessageBoxButtons.OK, MessageBoxIcon.Information); }

            return installed;
        }

        /// <summary>
        /// Get the details about a Card Set.
        /// </summary>
        /// <param name="CardSetGUID">The GUID of the Card Set.</param>
        /// <returns>Details of the Card Set</returns>
        static public string[] GetCardSetInfo(Guid CardSetGUID)
        {
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            string[] files = Directory.GetFiles(path, "*" + CardSetGUID.ToString() + ".cahc");
            if (files == null)
            {
                MessageBox.Show("ERROR: A card pack you are trying to use has gone missing, application will restart.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Restart();
            }
            XmlDocument cardSetDoc = new XmlDocument();
            cardSetDoc.Load(files[0]);
            XmlElement cardSetInfo = (XmlElement)cardSetDoc.GetElementsByTagName("CardSet")[0];
            string[] setInfo = new string[5];
            setInfo[0] = cardSetInfo.GetAttribute("GUID");
            setInfo[1] = cardSetInfo.GetAttribute("Name");
            setInfo[2] = cardSetInfo.GetAttribute("Version");
            setInfo[3] = files[0];
            return setInfo;
        }

        /// <summary>
        /// Get the value of a Card.
        /// </summary>
        /// <param name="CardSet">The GUID of the Card Set the Card belongs to.</param>
        /// <param name="CardType">If the Card is a Black or White Card.</param>
        /// <param name="CardID">The ID of the Card.</param>
        /// <returns>The text of the Card.</returns>
        static public string GetCard(Guid CardSet, char CardType, string CardID)
        {
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            string[] files = Directory.GetFiles(path, "*" + CardSet.ToString() + ".cahc");
            if (files == null)
            {
                MessageBox.Show("ERROR: A card pack you are trying to use has gone missing, application will restart.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Restart();
            }
            XmlDocument cardSetDoc = new XmlDocument();
            cardSetDoc.Load(files[0]);
            XmlNodeList CardBlock = cardSetDoc.GetElementsByTagName("CardPack");
            if (CardType == 'B')
            { CardBlock = cardSetDoc.GetElementsByTagName("BlackCards"); }
            else if (CardType == 'W')
            { CardBlock = cardSetDoc.GetElementsByTagName("WhiteCards"); }
            XmlNodeList Cards = CardBlock[0].ChildNodes;
            foreach (XmlElement Card in Cards)
            {
                if (Card.GetAttribute("ID") == CardID)
                {
                    return Card.InnerText;
                }
            }
            return "ERROR!";
        }

        /// <summary>
        /// Get all the White or Black Cards in a Card Set.
        /// </summary>
        /// <param name="CardSet">The Card Set GUID.</param>
        /// <param name="CardType">The type of Cards to get.</param>
        /// <returns>All the requested Cards.</returns>
        static public List<string> GetCards(Guid CardSet, char CardType)
        {
            if (CardType != 'B' && CardType != 'W')
            { throw new Exception("Bad CardType param"); }
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            string[] files = Directory.GetFiles(path, "*" + CardSet.ToString() + ".cahc");
            if (files == null)
            {
                MessageBox.Show("ERROR: A card pack you are trying to use has gone missing, application will restart.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Restart();
            }
            XmlDocument cardSetDoc = new XmlDocument();
            cardSetDoc.Load(files[0]);
            XmlNodeList CardBlock = cardSetDoc.GetElementsByTagName("CardPack");
            if (CardType == 'B')
            { CardBlock = cardSetDoc.GetElementsByTagName("BlackCards"); }
            else if (CardType == 'W')
            { CardBlock = cardSetDoc.GetElementsByTagName("WhiteCards"); }
            XmlNodeList Cards = CardBlock[0].ChildNodes;
            List<string> CardIDs = new List<string>();
            foreach (XmlElement Card in Cards)
            {
                if (CardType == 'W')
                { CardIDs.Add(CardSet.ToString() + "/" + CardType + "/" + Card.Attributes["ID"].Value); }
                else
                { CardIDs.Add(CardSet.ToString() + "/" + CardType + "/" + Card.Attributes["ID"].Value + "/" + Card.Attributes["Needs"].Value); }
            }

            return CardIDs;
        }

        /// <summary>
        /// Shuffle a Deck of Cards.
        /// </summary>
        /// <param name="CardSetGUIDs">A list of the Card Set GUIDs to be shuffled.</param>
        /// <param name="CardType">The type of Cards to shuffle.</param>
        /// <returns>A Shuffled Card Deck.</returns>
        static public Dictionary<int, string> ShuffleCards(Dictionary<int, string> Cards)
        {
            int moveFrom = 1;
            RNGCryptoServiceProvider seeder = new RNGCryptoServiceProvider();
            byte[] seed = new byte[4];
            seeder.GetBytes(seed);
            Random shuffler = new Random(BitConverter.ToInt32(seed, 0));
            int shuffleCount = 0;
            int shuffles = shuffler.Next(10000, 50000);
            Console.WriteLine("Shuffles: " + shuffles);
            while (shuffleCount < shuffles)
            {
                while (moveFrom < Cards.Count)
                {
                    int moveTo = shuffler.Next(1, Cards.Count + 1);
                    string x = Cards[moveFrom];
                    string y = Cards[moveTo];
                    Cards[moveFrom] = y;
                    Cards[moveTo] = x;
                    moveFrom++;
                }
                moveFrom = 1;
                shuffleCount++;
            }
            return Cards;
        }

        /// <summary>
        /// Deal the White Cards for players.
        /// </summary>
        /// <param name="WhiteCards">Pre-shuffled deck of White Cards.</param>
        /// <param name="NoOfPlayers">The total number of Players.</param>
        /// <param name="CardsPerPlayer">The number of Cards to deal to each Player.</param>
        /// <returns>The Dealt Hands</returns>
        static public List<Tuple<int, Card>> Deal(CardSet Set, int NoOfPlayers, int CardsPerPlayer)
        {
            List<Tuple<int, Card>> hands = new List<Tuple<int, Card>>();
            int card = 1;
            int player = 1;
            int dealtEach = 0;
            while (dealtEach < CardsPerPlayer)
            {
                while (player <= NoOfPlayers)
                {
                    try
                    {
                        hands.Add(new Tuple<int, Card>(player, Set.WhiteCards[Set.WhiteCardIndex[card]]));
                        card++;
                        player++;
                    }
                    catch
                    {
                        throw new Exception("Out of Cards");
                    }
                }
                player = 1;
                dealtEach++;
            }
            hands.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return hands;
        }
    }
}