using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace JoePitt.Cards
{
    [Serializable]
    public class CardSet
    {
        public Guid GUID { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string Path { get; private set; }
        public string Hash { get; private set; }
        public bool Verified { get; private set; }

        public int WhiteCardCount { get; private set; }
        public Dictionary<string, Card> WhiteCards { get; private set; }
        public Dictionary<int, string> WhiteCardIndex { get; private set; }
        public int BlackCardCount { get; private set; }
        public Dictionary<string, Card> BlackCards { get; private set; }
        public Dictionary<int, string> BlackCardIndex { get; private set; }

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

        public string GetXML()
        {
            string path = Application.StartupPath + "\\Resources\\CardSets";
            if (path.Contains("TESTWINDOW"))
            {
                path = "C:\\Users\\Public\\CardSets";
            }
            string[] files = Directory.GetFiles(path, "*" + GUID + ".cahc");
            if (files == null)
            {
                return "SETERROR";
            }
            return File.ReadAllText(files[0]);
        }

        public void Merge(List<Guid> GUIDs)
        {
            GUID = new Guid();
            Name = "Merged GameSet";
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
                    BlackCards.Add(guid.ToString() + "/" + Card.Attributes["ID"].Value, new Card(Card.Attributes["ID"].Value, Card.InnerText, Convert.ToInt32(Card.Attributes["Needs"].Value)));
                    BlackCardCount++;
                    BlackCardIndex.Add(BlackCardCount, guid.ToString() + "/" + Card.Attributes["ID"].Value);
                }

                CardBlock = cardSetDoc.GetElementsByTagName("WhiteCards");
                Cards = CardBlock[0].ChildNodes;
                foreach (XmlElement Card in Cards)
                {
                    WhiteCards.Add(guid.ToString() + "/" + Card.Attributes["ID"].Value, new Card(Card.Attributes["ID"].Value, Card.InnerText));
                    WhiteCardCount++;
                    WhiteCardIndex.Add(WhiteCardCount, guid.ToString() + "/" + Card.Attributes["ID"].Value);
                }
            }
            Dealer.ShuffleCards(BlackCardIndex);
            Dealer.ShuffleCards(WhiteCardIndex);
        }

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