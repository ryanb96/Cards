using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cards_Against_Humanity
{
    [Serializable]
    public class Vote
    {
        public Player Voter;
        public Answer Choice;

        public Vote(Player VoteFrom, Answer VoteFor)
        {
            Voter = VoteFrom;
            Choice = VoteFor;
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
