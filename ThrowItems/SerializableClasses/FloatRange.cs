namespace ThrowItems.SerializableClasses
{
    /// <summary>
    /// Formats a min/max value set to be serialized in a config cleanly.
    /// </summary>
    public class FloatRange
    {
        /// <summary>
        /// Gets or sets the minimum bound.
        /// </summary>
        public float Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum bound.
        /// </summary>
        public float Maximum { get; set; }
    }
}