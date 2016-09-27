using UnityEngine;
using System.Collections;

namespace UnityHue{
	[System.Serializable]
	public class HueLampState{
		public bool on = false;
		public int brightness;
		public int hue;
		public int saturation;
		[Tooltip("The options are none, select for a short flash, " +
			"and lselect for a 15 second flash")]
		public string alert;
		[Tooltip("The options are none, and colorloop" +
			"which cycles through the hue range at " +
			"current brightness and saturation")]
		public string effect;
		public string colorMode;
		[Tooltip("The time it takes, in multiples" +
			"of 100 ms to reach the new state" +
			"from the current state")]
		public int transitionTime = 4;
		public bool reachable = false;

	}
}
