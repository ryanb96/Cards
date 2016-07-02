using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace JoePitt.Cards
{
    /// <summary>
    /// A player's answer.
    /// </summary>
    [Serializable]
    public class Answer
    {
        /// <summary>
        /// The player who submitted the answer.
        /// </summary>
        public Player Submitter { get; private set; }
        /// <summary>
        /// The Black Card the answer is for.
        /// </summary>
        public Card BlackCard { get; private set; }
        /// <summary>
        /// The Submitted White Card.
        /// </summary>
        public Card WhiteCard { get; private set; }
        /// <summary>
        /// The Second White card, where required, that was submitted.
        /// </summary>
        public Card WhiteCard2 { get; private set; }
        /// <summary>
        /// The final text of the answer, after filling the blanks.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Create an answer for a black card which requires 1 White Card.
        /// </summary>
        /// <param name="submitter">The Player submitting the answer.</param>
        /// <param name="blackCard">The black card the answer is for.</param>
        /// <param name="whiteCard">The white card being submitted.</param>
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

        /// <summary>
        /// Create an answer for a black card which requires 2 White Card.
        /// </summary>
        /// <param name="submitter">The Player submitting the answer.</param>
        /// <param name="blackCard">The black card the answer is for.</param>
        /// <param name="whiteCard">The first white card being submitted.</param>
        /// <param name="whiteCard2">The second white card being submitted.</param>
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

        /// <summary>
        /// Export the answer into a byte array.
        /// </summary>
        /// <returns>A byte array of the answer.</returns>
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
