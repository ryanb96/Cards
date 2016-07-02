using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace JoePitt.Cards
{
    /// <summary>
    /// A collection of Cards which make up a Set.
    /// </summary>
    [Serializable]
    public class CardSet
    {
        /// <summary>
        /// The Globally Unique Identifier of the Card Set.
        /// </summary>
        public Guid GUID { get; private set; }
        /// <summary>
        /// The Display Name of the Card Set.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The version of the Card Set, in x.y format.
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// The location of the xml file which defines the Cad Set.
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The SHA256 Hash of the Card Set.
        /// </summary>
        public string Hash { get; private set; }
        /// <summary>
        /// If the hash was successfully verified.
        /// </summary>
        public bool Verified { get; private set; }
        /// <summary>
        /// The Number of White Cards.
        /// </summary>
        public int WhiteCardCount { get; private set; }
        /// <summary>
        /// All the White Cards in the Set.
        /// </summary>
        public Dictionary<string, Card> WhiteCards { get; private set; }
        /// <summary>
        /// The current order of the White Cards.
        /// </summary>
        public Dictionary<int, string> WhiteCardIndex { get; private set; }
        /// <summary>
        /// The number of Black Cards in the Set.
        /// </summary>
        public int BlackCardCount { get; private set; }
        /// <summary>
        /// All the Black Cards in the Set.
        /// </summary>
        public Dictionary<string, Card> BlackCards { get; private set; }
        /// <summary>
        /// The current order of the Black Cards.
        /// </summary>
        public Dictionary<int, string> BlackCardIndex { get; private set; }

        /// <summary>
        /// Loads a Card Set from its XML File.
        /// </summary>
        /// <param name="guid">The GUID of the Card Set to load.</param>
        public CardSet(Guid guid)
        {
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            string[] files = Directory.GetFiles(path, "*" + guid.ToString() + ".cahc");
            if (files == null)
            {
                MessageBox.Show("ERROR: A card pack you are trying to use has gone missing, application will restart.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Restart();
            }
            XmlDocument cardSetDoc = new XmlDocument();
            cardSetDoc.Load(files[0]);
            XmlElement cardSetInfo = (XmlElement)cardSetDoc.GetElementsByTagName("CardSet")[0];
            string[] setInfo = new string[5];
            GUID = new Guid(cardSetInfo.GetAttribute("GUID"));
            Name = setInfo[1] = cardSetInfo.GetAttribute("Name");
            Version = setInfo[2] = cardSetInfo.GetAttribute("Version");
            Path = setInfo[3] = files[0];

            XmlElement xmlBlackCards = (XmlElement)cardSetDoc.GetElementsByTagName("BlackCards")[0];
            XmlElement xmlWhiteCards = (XmlElement)cardSetDoc.GetElementsByTagName("WhiteCards")[0];
            SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
            byte[] allCards = Encoding.Default.GetBytes(xmlBlackCards.InnerXml + xmlWhiteCards.InnerXml);
            byte[] hash = hasher.ComputeHash(allCards);
            Hash = Convert.ToBase64String(hash);

            if (cardSetInfo.GetAttribute("Hash") == Hash)
            {
                Verified = true;
            }
            else
            {
                Verified = false;
                MessageBox.Show(Name + " (" + Version + ") has failed it's integrity check and not been loaded, Please reinstall it.", "Card Set Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BlackCardCount = 0;
            BlackCards = new Dictionary<string, Card>();
            BlackCardIndex = new Dictionary<int, string>();
            XmlNodeList CardBlock = cardSetDoc.GetElementsByTagName("CardPack");
            CardBlock = cardSetDoc.GetElementsByTagName("BlackCards");
            XmlNodeList Cards = CardBlock[0].ChildNodes;
            foreach (XmlElement Card in Cards)
            {
                string cardID = GUID.ToString() + "/" + Card.Attributes["ID"].Value;
                BlackCards.Add(cardID, new Card(cardID, Card.InnerText, Convert.ToInt32(Card.Attributes["Needs"].Value)));
                BlackCardCount++;
                BlackCardIndex.Add(BlackCardCount, cardID);
            }

            WhiteCardCount = 0;
            WhiteCards = new Dictionary<string, Card>();
            WhiteCardIndex = new Dictionary<int, string>();
            CardBlock = cardSetDoc.GetElementsByTagName("WhiteCards");
            Cards = CardBlock[0].ChildNodes;
            foreach (XmlElement Card in Cards)
            {
                string cardID = guid.ToString() + "/" + Card.Attributes["ID"].Value;
                WhiteCards.Add(cardID, new Card(cardID, Card.InnerText));
                WhiteCardCount++;
                WhiteCardIndex.Add(WhiteCardCount, cardID);
            }
        }
        
        /// <summary>
        /// Merges 2 or more Card Sets into the Single Game Set.
        /// </summary>
        /// <param name="GUIDs">GUIDs for all the Card Sets to be added.</param>
        public void Merge(List<Guid> GUIDs)
        {
            GUID = new Guid();
            Name = "GameSet";
            Version = "0.0";
            Path = "MEMORY";
            foreach (Guid guid in GUIDs)
            {
                string path = Application.StartupPath + "\\Resources\\CardSets";
                if (path.Contains("TESTWINDOW"))
                {
                    path = "C:\\Users\\Public\\CardSets";
                }
                string[] files = Directory.GetFiles(path, "*" + guid.ToString() + ".cahc");
                if (files == null)
                {
                    MessageBox.Show("ERROR: A card pack you are trying to use has gone missing, application will restart.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Restart();
                }
                XmlDocument cardSetDoc = new XmlDocument();
                cardSetDoc.Load(files[0]);
                XmlElement cardSetInfo = (XmlElement)cardSetDoc.GetElementsByTagName("CardSet")[0];
                XmlElement xmlBlackCards = (XmlElement)cardSetDoc.GetElementsByTagName("BlackCards")[0];
                XmlElement xmlWhiteCards = (XmlElement)cardSetDoc.GetElementsByTagName("WhiteCards")[0];
                SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                byte[] allCards = Encoding.Default.GetBytes(xmlBlackCards.InnerXml + xmlWhiteCards.InnerXml);
                byte[] hash = hasher.ComputeHash(allCards);
                Hash = Convert.ToBase64String(hash);

                if (cardSetInfo.GetAttribute("Hash") == Hash)
                {
                    Verified = true;
                }
                else
                {
                    Verified = false;
                    continue;
                }

                XmlNodeList CardBlock = cardSetDoc.GetElementsByTagName("CardPack");
                CardBlock = cardSetDoc.GetElementsByTagName("BlackCards");
                XmlNodeList Cards = CardBlock[0].ChildNodes;
                foreach (XmlElement Card in Cards)
                {
                    string cardID = guid.ToString() + "/" + Card.Attributes["ID"].Value;
                    BlackCards.Add(cardID, new Card(cardID, Card.InnerText, Convert.ToInt32(Card.Attributes["Needs"].Value)));
                    BlackCardCount++;
                    BlackCardIndex.Add(BlackCardCount, cardID);
                }

                CardBlock = cardSetDoc.GetElementsByTagName("WhiteCards");
                Cards = CardBlock[0].ChildNodes;
                foreach (XmlElement Card in Cards)
                {
                    string cardID = guid.ToString() + "/" + Card.Attributes["ID"].Value;
                    WhiteCards.Add(cardID, new Card(cardID, Card.InnerText));
                    WhiteCardCount++;
                    WhiteCardIndex.Add(WhiteCardCount, cardID);
                }
            }
            Dealer.ShuffleCards(BlackCardIndex);
            Dealer.ShuffleCards(WhiteCardIndex);
        }

        /// <summary>
        /// Exports the Card Set as a byte array.
        /// </summary>
        /// <returns>A byte array of the Card Set.</returns>
        public byte[] ToByteArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }
    }
}