using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityHue{
	[System.Serializable]
	public class StoredHueInfo{
		public HueBridgeInfo current;
		public List<HueBridgeInfo> allBridges;

		public StoredHueInfo (HueBridgeInfo current, List<HueBridgeInfo> allBridges)
		{
			this.current = current;
			this.allBridges = allBridges;
		}
		
	}
}