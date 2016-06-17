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
            string blankRegEx = "_{3,}";
            Regex regexr = new Regex(blankRegEx);
            if (regexr.IsMatch(Text))
            {
                Text = regexr.Replace(Text, WhiteCard.Text, 1);
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
            string blankRegEx = "_{3,}";
            Regex regexr = new Regex(blankRegEx);
            if (regexr.IsMatch(Text))
            {
                Text = regexr.Replace(Text, whiteCard.Text, 1);
                if (regexr.IsMatch(Text))
                {
                    Text = regexr.Replace(Text, whiteCard2.Text, 1);
                }
                else
                {
                    Text = Text + " " + WhiteCard2.Text;
                }
            }
            else
            {
                Text = Text + " " + WhiteCard.Text + " -- " + WhiteCard2.Text;
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
