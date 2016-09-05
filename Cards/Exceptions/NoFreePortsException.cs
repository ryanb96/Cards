using System;
using System.Runtime.Serialization;

namespace JoePitt.Cards.Exceptions
{
    /// <summary>
    /// Thrown when the range of supported TCP Ports are all in use.
    /// </summary>
    [Serializable]
    public class NoFreePortsException : Exception
    {
        /// <summary>
        /// Create the Exception
        /// </summary>
        public NoFreePortsException()
        {
        }

        /// <summary>
        /// Generates the exception with Message.
        /// </summary>
        /// <param name="message">The Message to set.</param>
        public NoFreePortsException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        /// <summary>
        /// Generate the exception with a message and inner exception.
        /// </summary>
        /// <param name="message">The Message to set.</param>
        /// <param name="innerException">the inner exception to set.</param>
        public NoFreePortsException(string message, Exception innerException) : base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        /// <summary>
        /// Deserialise an Exception?
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NoFreePortsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }
}
