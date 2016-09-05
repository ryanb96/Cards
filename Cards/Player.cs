using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace JoePitt.Cards
{
    /// <summary>
    /// Defines a player of the game.
    /// </summary>
    [Serializable]
    internal class Player
    {
        /// <summary>
        /// The name of the player, prefixed by [bot] for automated players.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The player's current score.
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// The player's current White Cards.
        /// </summary>
        public List<Card> WhiteCards { get; set; }
        /// <summary>
        /// Defines if the player is automated.
        /// </summary>
        public bool IsBot { get; private set; } = false;
        /// <summary>
        /// Indicates if the user is connected or not.
        /// </summary>
        public bool IsConnected { get; set; }
        private string SessionKey;

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="whiteCardsIn">The player's initial White Cards.</param>
        public Player(string name, List<Card> whiteCardsIn)
        {
            Name = name;
            if (Name.ToLower().StartsWith("[bot]"))
            {
                IsBot = true;
            }
            Score = 0;
            WhiteCards = whiteCardsIn;
        }

        /// <summary>
        /// Verifies that the player (re)joining has the same session key.
        /// </summary>
        /// <param name="sessionKeyIn">The provided Session Key.</param>
        /// <returns></returns>
        public bool Verify(string sessionKeyIn)
        {
            if (SessionKey == sessionKeyIn)
            {
                return true;
            }
            else if (string.IsNullOrEmpty(SessionKey))
            {
                SessionKey = sessionKeyIn;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}