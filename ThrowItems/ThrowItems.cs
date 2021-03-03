// <copyright file="ThrowItems.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;

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

        /// <summary>
        /// Gets the list of users who are currently blacklisted from throwing items.
        /// </summary>
        public static List<int> BlacklistedIds { get; } = new List<int>();

        /// <inheritdoc/>
        public override string Author { get; } = "RogerFK";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 3, 4);

        /// <inheritdoc/>
        public override string Name { get; } = "ThrowItems";

        /// <inheritdoc/>
        public override Version Version { get; } = new Version();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade += OnThrowingGrenade;
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade -= OnThrowingGrenade;
            base.OnDisabled();
        }

        private static void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            BlacklistedIds.Add(ev.Player.Id);
            Timing.CallDelayed(1.3f, () => BlacklistedIds.Remove(ev.Player.Id));
        }
    }
}