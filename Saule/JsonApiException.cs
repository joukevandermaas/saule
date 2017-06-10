using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Saule
{
    internal enum ErrorType
    {
        /// <summary>
        /// An error that is the server (application)'s fault.
        /// </summary>
        Server,

        /// <summary>
        /// An error that is the client (application user)'s fault.
        /// </summary>
        Client
    }

    /// <summary>
    /// The exception that is thrown when an the Json Api serializer misses necessary information.
    /// </summary>
    [Serializable]
    public class JsonApiException : Exception
    {
        internal JsonApiException(ErrorType type, string message)
            : base(message)
        {
            ErrorType = type;
        }

        internal JsonApiException(ErrorType type, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected JsonApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorType = (ErrorType)info.GetInt32(nameof(ErrorType));
        }

        internal ErrorType ErrorType { get; }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(ErrorType), ErrorType);

            base.GetObjectData(info, context);
        }
    }
}