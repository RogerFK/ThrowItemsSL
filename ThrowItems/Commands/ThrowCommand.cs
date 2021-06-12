// <copyright file="ThrowCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems.Commands
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using MEC;
    using RemoteAdmin;
    using ThrowItems.Components;
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

        private static Config Config { get; } = Plugin.Instance.Config;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as PlayerCommandSender)?.ReferenceHub);
            if (player == null || Plugin.BlacklistedIds.Contains(player.Id) || player.SessionVariables.ContainsKey("IsGhostSpectator"))
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

            player.ReferenceHub.weaponManager.scp268.ServerDisable();
            if (CustomItem.TryGet(item, out CustomItem customItem))
            {
                Pickup p = item.Spawn(player.Position);
                player.Inventory.items.Remove(item);
                player.ReferenceHub.weaponManager.NetworkcurWeapon = -1;
                player.ReferenceHub.inventory.RefreshWeapon();
                player.ReferenceHub.inventory.UpdateUniqChange();
                if (p.itemId == ItemType.MicroHID)
                {
                    var scale = player.Scale;
                    Timing.CallDelayed(0.5f, () =>
                    {
                        player.Scale = new Vector3(scale.x, scale.y * 0.99f, scale.z);
                        player.Scale = scale;
                    });
                }

                customItem.Spawned.Add(p);
                Timing.RunCoroutine(ThrowWhenRigidbody(player, p, (player.CameraTransform.forward + AddLaunchForce).normalized));
                response = "custom yeet";
                return true;
            }

            Pickup pickup = player.Inventory.SetPickup(item.id, item.durability, player.Position, player.Inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
            player.Inventory.items.Remove(item);
            player.ReferenceHub.weaponManager.NetworkcurWeapon = -1;
            player.ReferenceHub.inventory.RefreshWeapon();
            player.ReferenceHub.inventory.UpdateUniqChange();
            if (pickup.itemId == ItemType.MicroHID)
            {
                var scale = player.Scale;
                Timing.CallDelayed(0.5f, () =>
                {
                    player.Scale = new Vector3(scale.x, scale.y * 0.99f, scale.z);
                    player.Scale = scale;
                });
            }

            Timing.RunCoroutine(ThrowWhenRigidbody(player, pickup, (player.CameraTransform.forward + AddLaunchForce).normalized));
            response = "yeet";
            return true;
        }

        private static IEnumerator<float> ThrowWhenRigidbody(Player thrower, Pickup pickup, Vector3 dir)
        {
            Log.Debug("Starting the coroutine, waiting until the thrown Pickup has a RigidBody (has physics).", Config.Debug);
            yield return Timing.WaitUntilFalse(() => pickup != null && pickup.Rb == null);
            Log.Debug($"Rigidbody instantiated. Translating its position to {InitialPosVec3}, then throwing with a force of {dir * Config.ThrowForce}.", Config.Debug);
            pickup.gameObject.AddComponent<ItemDamageComponent>().AwakeFunc(thrower, pickup);
            pickup.Rb.transform.Translate(InitialPosVec3, Space.Self);
            pickup.Rb.AddForce(dir * Config.ThrowForce, ForceMode.Impulse);
            Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f)).normalized;
            pickup.Rb.angularVelocity = rand.normalized * Config.RandomSpinForce;
        }
    }
}