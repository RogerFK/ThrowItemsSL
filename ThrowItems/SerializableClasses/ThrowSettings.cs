namespace ThrowItems.SerializableClasses
{
    public class ThrowSettings
    {
        /// <summary>
        /// Gets or sets the settings to be applied when hitting a user on an enemy team.
        /// </summary>
        public EnemySettings EnemySettings { get; set; }

        /// <summary>
        /// Gets or sets the settings to be applied when hitting a user on the same team.
        /// </summary>
        public FriendlySettings FriendlySettings { get; set; }
    }
}