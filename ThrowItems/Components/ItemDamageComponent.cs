// <copyright file="ItemDamageComponent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ThrowItems.Components
{
#pragma warning disable SA1101
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ThrowItems.SerializableClasses;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ItemDamageComponent : MonoBehaviour
    {
        private readonly List<Player> hitPlayers = new List<Player>();
        private int internalChecks;
        private Player thrower;
        private Pickup thrownItem;

        private bool logDebug;

        /// <summary>
        /// Fills in the required parameters to successfully start the player collision checks.
        /// </summary>
        /// <param name="player">The thrower of the item.</param>
        /// <param name="pickup">The thrown item.</param>
        internal void AwakeFunc(Player player, Pickup pickup)
        {
            thrower = player;
            thrownItem = pickup;
            logDebug = Plugin.Instance.Config.Debug;
            Timing.RunCoroutine(CheckCoroutine());
        }

        private static float GetHealAmount(float toHeal, float current, float maximum)
        {
            if (toHeal + current > maximum)
            {
                return maximum - current;
            }

            return toHeal;
        }

        private void GrantEffects(Player target, ThrowSettings settings, bool isFriendly)
        {
            EffectSettings[] effectSettings = isFriendly ? settings.FriendlySettings.EffectSettings : settings.EnemySettings.EffectSettings;

            if (effectSettings != null && effectSettings.Length > 0)
            {
                foreach (var effectSetting in effectSettings)
                {
                    Log.Debug($"Effect granted to {target.Nickname}.\nEffect:{effectSetting.EffectType}\nIntensity:{effectSetting.Intensity}\nDuration:{effectSetting.Duration}", logDebug);
                    target.ChangeEffectIntensity(effectSetting.EffectType.ToString(), effectSetting.Intensity);
                    target.EnableEffect(effectSetting.EffectType, effectSetting.Duration);
                }
            }
        }

        private void Friendly_DealDamage(Player target, ThrowSettings throwSettings)
        {
            if (throwSettings.FriendlySettings.Damage == null)
                return;

            var amount = Random.Range(throwSettings.FriendlySettings.Damage.Minimum, throwSettings.FriendlySettings.Damage.Maximum);
            if (amount > 0)
            {
                target.ReferenceHub.falldamage.RpcDoSound();
                CheckForRagdoll(target, amount);
                thrower.GameObject.GetComponent<Scp049_2PlayerScript>().TargetHitMarker(thrower.Connection);
                Log.Debug($"Dealt {amount} damage to {target.Nickname}", logDebug);
            }
        }

        private void Friendly_DoHealing(Player target, ThrowSettings throwSettings)
        {
            if (throwSettings.FriendlySettings.Heal == null)
                return;

            var amount = Random.Range(throwSettings.FriendlySettings.Heal.Minimum, throwSettings.FriendlySettings.Heal.Maximum);
            var updatedHealAmount = GetHealAmount(amount, target.Health, target.MaxHealth);
            if (updatedHealAmount > 0)
            {
                target.Health += updatedHealAmount;
                Log.Debug($"Healed {target.Nickname} for {updatedHealAmount} health.", logDebug);
            }
        }

        private void Friendly_DoAhpHealing(Player target, ThrowSettings throwSettings)
        {
            if (throwSettings.FriendlySettings.AhpHeal == null)
                return;

            var ahpAmount = Random.Range(throwSettings.FriendlySettings.AhpHeal.Minimum, throwSettings.FriendlySettings.AhpHeal.Maximum);
            var updatedAhpAmount = GetHealAmount(ahpAmount, target.ArtificialHealth, target.MaxArtificialHealth);
            if (updatedAhpAmount > 0)
            {
                target.ArtificialHealth += updatedAhpAmount;
                Log.Debug($"Healed {target.Nickname} for {updatedAhpAmount} Ahp.", logDebug);
            }
        }

        private void Enemy_DealDamage(Player target, ThrowSettings throwSettings)
        {
            if (throwSettings.EnemySettings.Damage == null)
                return;

            var amount = Random.Range(throwSettings.EnemySettings.Damage.Minimum, throwSettings.EnemySettings.Damage.Maximum);
            if (amount > 0)
            {
                CheckForRagdoll(target, amount);
                target.ReferenceHub.falldamage.RpcDoSound();
                thrower.GameObject.GetComponent<Scp049_2PlayerScript>().TargetHitMarker(thrower.Connection);
                Log.Debug($"Dealt {amount} damage to {target.Nickname}.", logDebug);
            }
        }

        private void Enemy_DoHealing(Player target, ThrowSettings throwSettings)
        {
            if (throwSettings.EnemySettings.Heal == null)
                return;

            var healAmount = Random.Range(throwSettings.EnemySettings.Heal.Minimum, throwSettings.EnemySettings.Heal.Maximum);
            var updatedHealAmount = GetHealAmount(healAmount, target.Health, target.MaxHealth);
            if (updatedHealAmount > 0)
            {
                target.Health += updatedHealAmount;
                Log.Debug($"Healed {target.Nickname} for {updatedHealAmount} health.", logDebug);
            }
        }

        private void Enemy_DoAhpHealing(Player target, ThrowSettings throwSettings)
        {
            if (throwSettings.EnemySettings.AhpHeal == null)
                return;

            var healAmount = Random.Range(throwSettings.EnemySettings.AhpHeal.Minimum, throwSettings.EnemySettings.AhpHeal.Maximum);
            var updatedHealAmount = GetHealAmount(healAmount, target.ArtificialHealth, target.MaxArtificialHealth);
            if (updatedHealAmount > 0)
            {
                target.ArtificialHealth += updatedHealAmount;
                Log.Debug($"Healed {target.Nickname} for {updatedHealAmount} Ahp.", logDebug);
            }
        }

        private void CheckForRagdoll(Player player, float damage)
        {
            Log.Debug($"Running thrown item hit damage for {player.Nickname} at {damage} damage.", logDebug);
            if (player.Role == RoleType.Scp106)
            {
                damage *= 0.1f;
                Log.Debug($"Scp106 hit, new damage: {damage}", logDebug);
            }

            player.Hurt(damage, DamageTypes.Wall, player.Nickname, thrower.Id);
        }

        private IEnumerator<float> CheckCoroutine()
        {
            Log.Debug("Starting initial checks.", logDebug);
            if (!Plugin.Instance.Config.DamageAmounts.TryGetValue(thrownItem.itemId, out var settings))
                yield break;

            Log.Debug($"Starting effect check loop: {thrownItem.Rb.velocity.y}", logDebug);
            while (true)
            {
                try
                {
                    if (!thrownItem.Rb || thrownItem.Rb.velocity == null)
                    {
                        Log.Debug("Breaking loop, rigidbody or velocity is null.", logDebug);
                        break;
                    }

                    if (thrownItem.Rb.velocity.y == 0 && internalChecks > 5)
                    {
                        Log.Debug("Breaking loop, vertical velocity hit zero.", logDebug);
                        break;
                    }

                    Log.Debug($"Running loop of velocity check: {thrownItem.Rb.velocity.y}", logDebug);
                    internalChecks++;
                    foreach (var player in Player.List)
                    {
                        if (player == null ||
                            string.IsNullOrEmpty(player.UserId) ||
                            player.UserId == thrower.UserId ||
                            hitPlayers.Contains(player) ||
                            player.SessionVariables.ContainsKey("IsNPC") ||
                            player.SessionVariables.ContainsKey("IsGhostSpectator") ||
                            Vector3.Distance(thrownItem.transform.position, player.Position) > 2f)
                        {
                            continue;
                        }

                        var thrower035 = thrower.SessionVariables.ContainsKey("IsScp035");

                        bool isFriendly = (player.Side == thrower.Side &&
                            !player.SessionVariables.ContainsKey("IsScp035") &&
                            !thrower035 &&
                            thrower.Role != RoleType.Tutorial &&
                            player.Role != RoleType.Tutorial) || (thrower035 && player.IsScp);

                        hitPlayers.Add(player);
                        if (!isFriendly)
                        {
                            Log.Debug("Player is not friendly, adjusting accordingly.", logDebug);

                            Enemy_DealDamage(player, settings);
                            Enemy_DoHealing(player, settings);
                            Enemy_DoAhpHealing(player, settings);

                            if (settings.EnemySettings.ClearEffects)
                            {
                                player.DisableAllEffects();
                            }
                        }
                        else
                        {
                            Log.Debug("Player is friendly, adjusting accordingly.", logDebug);

                            Friendly_DealDamage(player, settings);
                            Friendly_DoHealing(player, settings);
                            Friendly_DoAhpHealing(player, settings);

                            if (settings.FriendlySettings.ClearEffects)
                            {
                                player.DisableAllEffects();
                            }
                        }

                        GrantEffects(player, settings, isFriendly);

                        if (isFriendly ? settings.FriendlySettings.ShouldDelete : settings.EnemySettings.ShouldDelete)
                        {
                            Log.Debug($"Ending loop, ShouldDelete is enabled.", logDebug);
                            hitPlayers.Clear();
                            thrownItem.Delete();
                            yield break;
                        }

                        if (isFriendly ? !settings.FriendlySettings.HitMultiple : !settings.EnemySettings.HitMultiple)
                        {
                            Log.Debug($"Ending loop, HitMultiple is disabled.", logDebug);
                            yield break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Error while checking for item collisions: {e}");
                }

                yield return Timing.WaitForOneFrame;
            }

            Log.Debug("Ended throw, vertical velocity hit zero.", logDebug);
            hitPlayers.Clear();
            Destroy(this);
        }
    }
}