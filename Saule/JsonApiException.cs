using System;
using System.Runtime.Serialization;

namespace Saule
{
    /// <summary>
    /// The exception that is thrown when an the Json Api serializer misses necessary information
    /// </summary>
    [Serializable]
    public class JsonApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class.
        /// </summary>
        public JsonApiException()
            : base("An error occured while serializing or deserializing a Json Api document")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class.
        /// </summary>
        /// <param name="message">
        /// A description of the error. The content of message is intended to be understood by humans.
        /// The caller of this constructor is required to ensure that this string has been localized
        /// for the current system culture.
        /// </param>
        public JsonApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class.
        /// </summary>
        /// <param name="message">
        /// A description of the error. The content of message is intended to be understood by humans.
        /// The caller of this constructor is required to ensure that this string has been localized
        /// for the current system culture.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException parameter
        /// is not null, the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public JsonApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected JsonApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}