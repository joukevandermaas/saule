namespace Saule
{
    /// <summary>
    /// Represents ways in which resources can be related.
    /// </summary>
    public enum RelationshipKind
    {
        /// <summary>
        /// A to-one relationship
        /// </summary>
        Single = 0,

        /// <summary>
        /// A to-many relationship
        /// </summary>
        Many = 1
    }
}
