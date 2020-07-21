using System.Collections.Generic;

using Exiled.API.Features;

using UnityEngine;

namespace ThrowItems
{
	public class ThrowItems : Plugin<Config>
	{
		public override void OnEnabled()
		{
			Exiled.Events.Handlers.Player.ItemDropped += this.Player_ItemDropped;
			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.ItemDropped -= this.Player_ItemDropped;
			base.OnDisabled();
		}

		private void Player_ItemDropped(Exiled.Events.EventArgs.ItemDroppedEventArgs ev) 
		{
			if (!Config.IsEnabled) return;
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
