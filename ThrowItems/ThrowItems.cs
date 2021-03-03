// <copyright file="ThrowItems.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems
{
    using System;
    using Exiled.API.Features;

    /// <inheritdoc/>
    public class ThrowItems : Plugin<Config>
    {
        private static readonly ThrowItems InstanceValue = new ThrowItems();

        private ThrowItems()
        {
        }

        /// <summary>
        /// Gets the instance of the <see cref="ThrowItems"/> class.
        /// </summary>
        public static ThrowItems Instance { get; } = InstanceValue;

        /// <inheritdoc/>
        public override string Author { get; } = "RogerFK";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 3, 4);

        /// <inheritdoc/>
        public override string Name { get; } = "ThrowItems";

        /// <inheritdoc/>
        public override Version Version { get; } = new Version();
    }
}