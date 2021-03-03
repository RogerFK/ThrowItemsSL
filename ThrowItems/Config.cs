// <copyright file="Config.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    /// <inheritdoc/>
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
    }
}