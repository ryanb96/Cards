using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace JoePitt.Cards
{
    [Serializable]
    /// <summary>
    /// Defines a player of the game.
    /// </summary>
    public class Player
    {
        public string Name;
        public int Score;
        public List<Card> WhiteCards;
        public bool IsBot = false;
        [NonSerialized]
        public TcpClient Connection;
        public int Submitted;
        public int Voted;
        private string PasswordHash;

        public Player(string name, List<Card> whiteCardsIn)
        {
            Name = name;
            if (Name.ToLower().StartsWith("[bot]"))
            {
                IsBot = true;
            }
            Score = 0;
            WhiteCards = whiteCardsIn;
            Submitted = 0;
            Voted = 0;
        }

        public bool Verify(string Hash)
        {
            if (PasswordHash == Hash)
            {
                return true;
            }
            else if (string.IsNullOrEmpty(PasswordHash))
            {
                PasswordHash = Hash;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}