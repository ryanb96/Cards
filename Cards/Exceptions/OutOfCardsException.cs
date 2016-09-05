using System;
using System.Runtime.Serialization;

namespace JoePitt.Cards.Exceptions
{
    /// <summary>
    /// Thrown when the dealer runs out of card while dealing.
    /// </summary>
    [Serializable]
    public class OutOfCardsException : Exception
    {
        /// <summary>
        /// Create the Exception
        /// </summary>
        public OutOfCardsException()
        {
        }

        /// <summary>
        /// Generates the exception with Message.
        /// </summary>
        /// <param name="message">The Message to set.</param>
        public OutOfCardsException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        /// <summary>
        /// Generate the exception with a message and inner exception.
        /// </summary>
        /// <param name="message">The Message to set.</param>
        /// <param name="innerException">the inner exception to set.</param>
        public OutOfCardsException(string message, Exception innerException) : base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        /// <summary>
        /// Deserialise an Exception?
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected OutOfCardsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }
}
