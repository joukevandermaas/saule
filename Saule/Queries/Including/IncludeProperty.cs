﻿namespace Saule.Queries.Including
{
    /// <summary>
    /// Property for including
    /// </summary>
    public class IncludeProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeProperty"/> class.
        /// </summary>
        /// <param name="name">property name</param>
        public IncludeProperty(string name)
        {
            Name = name.ToPascalCase();
        }

        /// <summary>
        /// Gets property name
        /// </summary>
        public string Name { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}