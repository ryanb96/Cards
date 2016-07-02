using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JoePitt.Cards
{
    /// <summary>
    /// A playing card.
    /// </summary>
    [Serializable]
    public class Card
    {
        /// <summary>
        /// If the card is a Black or White Card.
        /// </summary>
        public char Type { get; private set; }
        /// <summary>
        /// The Unique Identified of the card, [GUID]/[Type][Number]
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// The text of the card.
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// For black cards, the number of white cards required by this card.
        /// </summary>
        public int Needs { get; private set; }

        /// <summary>
        /// Create a Black Card.
        /// </summary>
        /// <param name="id">The Unique Identifier of the card.</param>
        /// <param name="text">The Text of the card.</param>
        /// <param name="needs">How many white cards, this black card requires.</param>
        public Card(string id, string text, int needs)
        {
            ID = id;
            Type = 'B';
            Text = text;
            Needs = needs;
        }

        /// <summary>
        /// Create a White Card.
        /// </summary>
        /// <param name="id">The Unique Identifier of the card.</param>
        /// <param name="text">The Text of the card.</param>
        public Card(string id, string text)
        {
            ID = id;
            Type = 'W';
            Text = text;
        }

        /// <summary>
        /// Exports the card as a byte array.
        /// </summary>
        /// <returns>A byte array of the Card.</returns>
        public byte[] ToArray()
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
