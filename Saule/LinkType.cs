using System;

namespace Saule
{
    /// <summary>
    /// Describes one or more link types to be generated.
    /// </summary>
    [Flags]
    public enum LinkType
    {
        /// <summary>
        /// No links
        /// </summary>
        None = 0,

        /// <summary>
        /// Only self links
        /// </summary>
        Self = 1,

        /// <summary>
        /// Only related links
        /// </summary>
        Related = 2,

        /// <summary>
        /// Generate all possible links
        /// </summary>
        All = ~None
    }
}