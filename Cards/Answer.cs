using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace JoePitt.Cards
{
    [Serializable]
    public class Answer
    {
        public Player Submitter { get; private set; }
        public Card BlackCard { get; private set; }
        public Card WhiteCard { get; private set; }
        public Card WhiteCard2 { get; private set; }
        public string Text { get; private set; }

        public Answer(Player submitter, Card blackCard, Card whiteCard)
        {
            Submitter = submitter;
            BlackCard = blackCard;
            WhiteCard = whiteCard;
            Text = BlackCard.Text;
            if (Text.Contains("____________"))
            {
                Text = Text.Replace("____________", WhiteCard.Text);
            }
            else
            {
                Text = Text + " " + WhiteCard.Text;
            }
        }

        public Answer(Player submitter, Card blackCard, Card whiteCard, Card whiteCard2)
        {
            Submitter = submitter;
            BlackCard = blackCard;
            WhiteCard = whiteCard;
            WhiteCard2 = whiteCard2;
            Text = BlackCard.Text;
            if (blackCard.Text.Contains("____________"))
            {
                Regex regex = new Regex(Regex.Escape("____________"));
                Text = regex.Replace(Text, WhiteCard.Text, 1);
            }
            else
            {
                Text = Text + " " + WhiteCard.Text;
            }
            if (Text.Contains("____________"))
            {
                Text = Text.Replace("____________", WhiteCard2.Text);
            }
            else
            {
                Text = Text + " -- " + WhiteCard2.Text;
            }
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
