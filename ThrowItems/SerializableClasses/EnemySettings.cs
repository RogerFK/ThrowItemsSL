namespace ThrowItems.SerializableClasses
{
    public class EnemySettings
    {
        /// <summary>
        /// Gets or sets the damage to be applied on hit.
        /// </summary>
        public FloatRange Damage { get; set; }

        /// <summary>
        /// Gets or sets the health to be applied on hit.
        /// </summary>
        public FloatRange Heal { get; set; }

        /// <summary>
        /// Gets or sets the amount of artificial health to be applied on hit.
        /// </summary>
        public FloatRange AhpHeal { get; set; }

        /// <summary>
        /// Gets or sets the effects to be applied on hit.
        /// </summary>
        public EffectSettings[] EffectSettings { get; set; }

        public bool ClearEffects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item should be deleted when it hits a user.
        /// </summary>
        public bool ShouldDelete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether items can hit multiple people.
        /// </summary>
        public bool HitMultiple { get; set; }
    }
}