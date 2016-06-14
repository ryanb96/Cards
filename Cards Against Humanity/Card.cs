using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cards_Against_Humanity
{
    [Serializable]
    public class Card
    {
        public char Type { get; private set; }
        public string ID { get; private set; }
        public string Text { get; private set; }
        public int Needs { get; private set; }

        /// <summary>
        /// Create a Black Card
        /// </summary>
        /// <param name="text">The text of the Black Card.</param>
        /// <param name="needs">The number of White Cards the Black Card requires.</param>
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
        /// <param name="id"></param>
        /// <param name="text"></param>
        public Card(string id, string text)
        {
            ID = id;
            Type = 'W';
            Text = text;
        }

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
