using System.Collections.Generic;

using Exiled.API.Features;

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
					inv.NetworkitemUniq = -1;
					inv.Network_curItemSynced = ItemType.None;
				}
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
			if (Config.CommandEnabled && Config.MustUseCommand && ev.Player.Id != whoThrew) return;
			whoThrew = -1;
			MEC.Timing.RunCoroutine(ThrowWhenRigidbody(ev.Pickup, (ev.Player.ReferenceHub.PlayerCameraReference.forward + Config.addLaunchForce).normalized));
		}

		private IEnumerator<float> ThrowWhenRigidbody(Pickup pickup, Vector3 dir) 
		{
			yield return MEC.Timing.WaitUntilFalse(() => pickup.Rb == null); // mom im scared of loops
			pickup.Rb.transform.Translate(Config.initialPosVec3, Space.Self);
			pickup.Rb.AddForce(dir * Config.ThrowForce, ForceMode.Impulse);
			Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f)).normalized;
			pickup.Rb.angularVelocity = rand.normalized * Config.RandomSpinForce;
		}
	}
}
