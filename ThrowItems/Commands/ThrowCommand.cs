// <copyright file="ThrowCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems.Commands
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using MEC;
    using RemoteAdmin;
    using UnityEngine;
    using Random = UnityEngine.Random;

    /// <inheritdoc/>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ThrowCommand : ICommand
    {
        private static readonly Vector3 InitialPosVec3 = new Vector3(0f, 0.5f, 0f);
        private static readonly Vector3 AddLaunchForce = new Vector3(0f, 0.25f, 0f);

        /// <inheritdoc/>
        public string Command { get; } = "throw";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "drop" };

        /// <inheritdoc/>
        public string Description { get; } = "The command to throw a held item.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as PlayerCommandSender)?.ReferenceHub);
            if (player == null || ThrowItems.BlacklistedIds.Contains(player.Id))
            {
                response = string.Empty;
                return false;
            }

            Inventory.SyncItemInfo item = player.CurrentItem;
            if (item == default)
            {
                response = "Sorry, but you can't throw air. yet.";
                return false;
            }

            Pickup pickup = player.Inventory.SetPickup(item.id, item.durability, player.Position, player.Inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
            player.Inventory.items.Remove(item);
            Timing.RunCoroutine(ThrowWhenRigidbody(pickup, (player.CameraTransform.forward + AddLaunchForce).normalized));
            response = "yeet";
            return true;
        }

        private static IEnumerator<float> ThrowWhenRigidbody(Pickup pickup, Vector3 dir)
        {
            Config config = ThrowItems.Instance.Config;
            Log.Debug("Starting the coroutine, waiting until the thrown Pickup has a RigidBody (has physics).", config.Debug);
            yield return Timing.WaitUntilFalse(() => pickup != null && pickup.Rb == null);
            Log.Debug($"Rigidbody instantiated. Translating its position to {InitialPosVec3}, then throwing with a force of {dir * config.ThrowForce}.");
            pickup.Rb.transform.Translate(InitialPosVec3, Space.Self);
            pickup.Rb.AddForce(dir * config.ThrowForce, ForceMode.Impulse);
            Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f)).normalized;
            pickup.Rb.angularVelocity = rand.normalized * config.RandomSpinForce;
        }
    }
}