namespace ThrowItems.SerializableClasses
{
    using Exiled.API.Enums;

    /// <summary>
    /// Formats effects to be serialized into a config cleanly.
    /// </summary>
    public class EffectSettings
    {
        /// <summary>
        /// Gets or sets the effect to be applied.
        /// </summary>
        public EffectType EffectType { get; set; }

        /// <summary>
        /// Gets or sets the amount of seconds the effect lasts.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the strength of the effect.
        /// </summary>
        public byte Intensity { get; set; }
    }
}