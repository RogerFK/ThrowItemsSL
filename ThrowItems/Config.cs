// <copyright file="Config.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using ThrowItems.SerializableClasses;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be shown.
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the amount of force used when throwing an item.
        /// </summary>
        [Description("The amount of force on an item on throw. Can be negative to throw it backwards.")]
        public float ThrowForce { get; set; } = 17f;

        /// <summary>
        /// Gets or sets the amount of random spin an item will have when thrown.
        /// </summary>
        [Description("Setting this to 0 \"disables\" the random spin, otherwise the items will randomly spin.")]
        public float RandomSpinForce { get; set; } = 20f;

        /// <summary>
        /// Gets or sets the amount of damage a thrown item will do if it hits a player.
        /// </summary>
        [Description("The amount of damage a thrown item will do if it hits a player.")]
        public Dictionary<ItemType, ThrowSettings> DamageAmounts { get; set; } = new Dictionary<ItemType, ThrowSettings>
        {
            [ItemType.Adrenaline] = new ThrowSettings
            {
                EnemySettings = new EnemySettings
                {
                    Damage = new FloatRange
                    {
                        Minimum = 5,
                        Maximum = 10,
                    },
                    EffectSettings = new[]
                    {
                        new EffectSettings
                        {
                            EffectType = EffectType.Invigorated, Duration = 5, Intensity = 1,
                        },
                        new EffectSettings
                        {
                            EffectType = EffectType.Scp207, Duration = 5, Intensity = 2,
                        },
                    },
                    ShouldDelete = true,
                    HitMultiple = false,
                },
                FriendlySettings = new FriendlySettings
                {
                    Heal = new FloatRange
                    {
                        Minimum = 5,
                        Maximum = 10,
                    },
                    AhpHeal = new FloatRange
                    {
                        Minimum = 15,
                        Maximum = 30,
                    },
                    EffectSettings = new[]
                    {
                        new EffectSettings
                        {
                            EffectType = EffectType.Invigorated, Duration = 5, Intensity = 1,
                        },
                        new EffectSettings
                        {
                            EffectType = EffectType.Scp207, Duration = 5, Intensity = 2,
                        },
                    },
                    ShouldDelete = true,
                    HitMultiple = false,
                },
            },
            [ItemType.Medkit] = new ThrowSettings
            {
                EnemySettings = new EnemySettings
                {
                    Damage = new FloatRange
                    {
                        Minimum = 10,
                        Maximum = 20,
                    },
                    EffectSettings = new EffectSettings[0],
                    ShouldDelete = true,
                    HitMultiple = false,
                },
                FriendlySettings = new FriendlySettings
                {
                    Heal = new FloatRange
                    {
                        Minimum = 10,
                        Maximum = 30,
                    },
                    AhpHeal = new FloatRange
                    {
                        Minimum = 5,
                        Maximum = 10,
                    },
                    EffectSettings = new[]
                    {
                        new EffectSettings
                        {
                            EffectType = EffectType.Invigorated,
                            Duration = 5,
                            Intensity = 1,
                        },
                    },
                    ShouldDelete = true,
                    HitMultiple = false,
                },
            },
        };
    }
}