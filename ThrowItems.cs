using System;
using System.Collections.Generic;
using System.Text;

using CommandSystem;

using Exiled.API.Features;

using Mirror;

using UnityEngine;

namespace ThrowItems
{
	public class ThrowItems : Plugin<Config>
	{
		public override string Name => "ThrowItems";
		public override void OnEnabled()
		{
			Exiled.Events.Handlers.Player.ItemDropped += this.Player_ItemDropped;
			Exiled.Events.Handlers.Server.SendingConsoleCommand += this.Server_SendingConsoleCommand;
			base.OnEnabled();
		}
		int whoThrew = -1;
		private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev) 
		{
			//if (!ev.Allow) return;

			if(ev.Name.Equals("drop") || ev.Name.Equals("throw"))
			{
				ev.Allow = false;
				Inventory inv = ev.Player.GameObject.GetComponent<Inventory>();
				if(inv != null) 
				{
					int index = inv.GetItemIndex();
					if (index == -1) 
					{
						ev.ReturnMessage = "Buddy, you should be holding an item...";
						return;
					} 

					whoThrew = ev.Player.Id;
					inv.CallCmdDropItem(index);
					ev.ReturnMessage = "Rude boi throwing items at people smh";
					// This would "fix" a game bug, only some times
					//MEC.Timing.CallDelayed(0.15f,
					/*MEC.Timing.CallPeriodically(0.14f, 0.02f, 
						() => {
							inv.RefreshModels();
							inv.RefreshWeapon();
							inv.SetCurItem(ItemType.None);
							inv.NetworkcurItem = ItemType.None;
							inv.NetworkitemUniq = -1;
					});*/
				}
			}
			else if (ev.Name.Equals("petme")) 
			{
				GameObject prefab = null;
				foreach (var thing in NetworkManager.singleton.spawnPrefabs) {
					if(thing.name == "Player") {
						prefab = thing;
						break;
					}
				}
				Role role = CharacterClassManager._staticClasses.Get(RoleType.Scp173);
				if (prefab == null) return;
				GameObject gameObject = UnityEngine.Object.Instantiate(prefab, ev.Player.ReferenceHub.PlayerCameraReference);
				var ccm = gameObject.GetComponent<CharacterClassManager>();
				if (!ccm)
				{
					Log.Info("bruh");
				}
				else
				{
					ccm.SetPlayersClass(RoleType.Scp173, gameObject);
					ccm.MyModel = role.model_player; // in case
				}
				if(!gameObject.GetComponent<NetworkIdentity>()) { Log.Info("im done"); }
				NetworkServer.Spawn(gameObject);
				gameObject.transform.position = ev.Player.Position;
				gameObject.transform.parent = ev.Player.GameObject.transform;
				gameObject.transform.position = ev.Player.Position;
				gameObject.transform.localPosition.Set(0, 1f, 2f);
				gameObject.transform.localScale.Set(10f, 1f, 2f);

			}
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.ItemDropped -= this.Player_ItemDropped;
			Exiled.Events.Handlers.Server.SendingConsoleCommand -= this.Server_SendingConsoleCommand;
			base.OnDisabled();
		}

		private void Player_ItemDropped(Exiled.Events.EventArgs.ItemDroppedEventArgs ev) 
		{
			if (!Config.IsEnabled) return;
			if (Config.MustUseCommand && ev.Player.Id != whoThrew) return;
			whoThrew = -1;
			MEC.Timing.RunCoroutine(ThrowWhenRigidbody(ev.Pickup, (ev.Player.ReferenceHub.PlayerCameraReference.forward + Config.addLaunchForce).normalized));
		}

		private IEnumerator<float> ThrowWhenRigidbody(Pickup pickup, Vector3 dir) 
		{
			yield return MEC.Timing.WaitUntilFalse(() => pickup.Rb == null); // mom im scared of loops
			pickup.Rb.transform.Translate(Config.initialPosVec3, Space.Self);
			pickup.Rb.AddForce(dir * Config.ThrowForce, ForceMode.Impulse);
		}
	}
}
