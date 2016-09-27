using UnityEngine;
using System.Collections;

namespace UnityHue {
	public class HueInfoStorer : MonoBehaviour {
		public string prefKey = "StoredHueInformation";
		
		public void Save()
		{
			PlayerPrefs.SetString(prefKey, HueBridge.instance.GetHueStateString());
		}
		public bool Restore()
		{
			var storedState = PlayerPrefs.GetString(prefKey, "");
			if(string.IsNullOrEmpty(storedState))
				return false;
			else
			{
				HueBridge.instance.RestoreHueFromString(storedState);
				return true;
			}
		}
	}
}
