using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JoePitt.Cards
{
    /// <summary>
    /// A Player's Vote of the best answer.
    /// </summary>
    [Serializable]
    public class Vote
    {
        /// <summary>
        /// Who submitted the vote.
        /// </summary>
        public Player Voter { get; set; }
        /// <summary>
        /// The Answer that was voted for.
        /// </summary>
        public Answer Choice { get; set; }

        /// <summary>
        /// Creates a vote.
        /// </summary>
        /// <param name="VoteFrom">Who's submitting the vote.</param>
        /// <param name="VoteFor">Which Answer did they vote for.</param>
        public Vote(Player VoteFrom, Answer VoteFor)
        {
            Voter = VoteFrom;
            Choice = VoteFor;
        }

        /// <summary>
        /// Exports the Vote to a byte array.
        /// </summary>
        /// <returns>The byte array of the Vote.</returns>
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
