using System.ComponentModel;
using System.Globalization;

using Exiled.API.Features;
using Exiled.API.Interfaces;

using UnityEngine;

using YamlDotNet.Serialization;

namespace ThrowItems
{
	public sealed class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		[Description("i dunno how this one scales so don't ask. also not doing a 35 item dictionary for each item. can be negative so you throw it back (like poopoo)")]
		public float ThrowForce { get; set; } = 17f;

		[Description("setting to 0 \"disables\" the random spin, otherwise the items will randomly spin")]
		public float RandomSpinForce { get; set; } = 0f;

		[Description("if u set this to (0, 0.25, 0) it will throw up from player perspective. just adds force. must be: (X, Y, Z)")]
		public string LaunchAddForce
		{
			get
			{
				return $"({addLaunchForce.x.ToString(CultureInfo.InvariantCulture)}, {addLaunchForce.y.ToString(CultureInfo.InvariantCulture)}, {addLaunchForce.z.ToString(CultureInfo.InvariantCulture)})";
			}
			set
			{
				ParseVector(value, ref addLaunchForce);
			}
		}

		[Description("if u set this to (0, -0.5, 0) it looks like it comes out of the player's dong. relative position to the player. must be: (X, Y, Z)")]
		public string InitialOffsetPosition
		{
			get {
				return $"({initialPosVec3.x.ToString(CultureInfo.InvariantCulture)}, {initialPosVec3.y.ToString(CultureInfo.InvariantCulture)}, {initialPosVec3.z.ToString(CultureInfo.InvariantCulture)})";
			}
			set
			{
				ParseVector(value, ref initialPosVec3);
			}
		}

		public bool MustUseCommand { get; set; } = false;

		private void ParseVector(string value, ref Vector3 output)
		{
			string helper = value.Trim();

			if (helper[0] != '(' || helper[helper.Length - 1] != ')')
				goto Retardation;

			string[] values = helper.Split(',');

			if (values.Length < 3)
				goto Retardation;
			var whatToTrim = new char[] { ' ', '(', ')' };

			try
			{
				for (int i = 0; i < 3; i++)
				{
					output[i] = float.Parse(values[i].Trim(whatToTrim), CultureInfo.InvariantCulture);
				}
			} catch { goto Retardation; }

			return;

		Retardation:
			Log.Error("Vectors configs MUST be: (X.XX, Y.YY, Z.ZZ) (i.e.: (0, 0.5, 0), and they MUST use a '.').");
			return;
		}

		[YamlIgnore()]
		public Vector3 initialPosVec3 = new Vector3(0f, 0.5f, 0f);
		[YamlIgnore()]
		public Vector3 addLaunchForce = new Vector3(0f, 0.25f, 0f);
	}
}
