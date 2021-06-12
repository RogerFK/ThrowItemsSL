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
    public class Plugin : Plugin<Config>
    {
        private static readonly Plugin InstanceValue = new Plugin();

        private Plugin()
        {
        }

        /// <summary>
        /// Gets the instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; } = InstanceValue;

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
        public override Version Version { get; } = new Version(1, 2, 2);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade += OnThrowingGrenade;
            Exiled.Events.Handlers.Player.UsingMedicalItem += OnUsingMedicalItem;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade -= OnThrowingGrenade;
            Exiled.Events.Handlers.Player.UsingMedicalItem -= OnUsingMedicalItem;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            base.OnDisabled();
        }

        private static void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            BlacklistedIds.Add(ev.Player.Id);
            Timing.CallDelayed(1.3f, () => BlacklistedIds.Remove(ev.Player.Id));
        }

        private static void OnUsingMedicalItem(UsingMedicalItemEventArgs ev)
        {
            BlacklistedIds.Add(ev.Player.Id);
            Timing.CallDelayed(3f, () => BlacklistedIds.Remove(ev.Player.Id));
        }

        private static void OnRoundStarted()
        {
            BlacklistedIds.Clear();
        }
    }
}