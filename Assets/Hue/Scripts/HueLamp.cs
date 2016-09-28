using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

using MiniJSON;

namespace UnityHue{
	/// <summary>
	/// Class for accessing and modifying a single Hue lamp
	/// </summary>
	[System.Serializable]
	public class HueLamp {
		public string name;
		public string id;
		public string type;
		public string modelID;
		public string softwareVersion;
		public HueLampState lampState;

		public HueLamp (string id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public HueLamp (string id)
		{
			this.id = id;
		}

		public void SetColor (Color color, params JsonParameter[] additionalParameters)
		{
			SetColor(color, null, null, additionalParameters);
		}

		public void SetColor (Color color, Action<string> successCallback, 
			Action<HueErrorInfo> errorCallback, params JsonParameter[] additionalParameters)
		{
			JsonParameter hue, bri, sat;
			HueParameters.ColorParameter(color, out hue, out sat, out bri);
			var list = new List<JsonParameter>(additionalParameters);
			list.Add(hue);
			list.Add(bri);
			list.Add(sat);
			SetState(successCallback, errorCallback, list.ToArray());
		}

		public void UpdateLamp (Action<HueErrorInfo> errorCallback = null)
		{
			HueBridge.instance.UpdateLamp(id, this, errorCallback);
		}

		public void SetState ()
		{
			SetState(StateToParameters(this.lampState));
		}

		public void SetState (params JsonParameter[] parameters)
		{
			SetState(null, null, parameters);
		}

		public void SetState (HueLampState state)
		{
			SetState(null, null, StateToParameters(state));
		}

		public void SetState (HueLampState state, Action<string> successCallback, 
			Action<HueErrorInfo> errorCallback)
		{
			SetState(successCallback, errorCallback, StateToParameters(state));
		}
		/// <summary>
		/// Sets the state of the lamp according to the supplied JsonParameters
		/// A JsonParameter consists of a key (the name of the parameter as specified
		/// by the hue api i.e "bri" for brightness) and a value to which that parameter
		/// should be set. 
		/// </summary>
		/// <param name="successCallback">Success callback.</param>
		/// <param name="errorCallback">Error callback.</param>
		/// <param name="parameters">Parameters.</param>
		public void SetState (Action<string> successCallback, 
			Action<HueErrorInfo> errorCallback, params JsonParameter[] parameters)
		{
			string url = HueBridge.instance.BaseURLWithUserName + "/lights/" + id + "/state";
			UnityWebRequest stateRequest = UnityWebRequest.Put(url, JsonHelper.CreateJsonParameterString(parameters));
			HueBridge.instance.SendRequest(stateRequest, successCallback, errorCallback);
		}
		public void SetName (string lampName, Action<string> successCallback = null, Action<HueErrorInfo> errorCallback = null)
		{
			string url = HueBridge.instance.BaseURLWithUserName + "/lights/" + id;
			UnityWebRequest renameRequest = UnityWebRequest.Put(url, 
				JsonHelper.CreateJsonParameterString(new JsonParameter(HueKeys.NAME, lampName)));
			HueBridge.instance.SendRequest(renameRequest, successCallback, errorCallback);
		}
		public static JsonParameter[] StateToParameters(HueLampState state)
		{
			var list = new List<JsonParameter>();
			list.Add(new JsonParameter(HueKeys.ON, state.on));
			list.Add(new JsonParameter(HueKeys.BRIGHTNESS, state.brightness));
			list.Add(new JsonParameter(HueKeys.HUE, state.hue));
			list.Add(new JsonParameter(HueKeys.SATURATION, state.saturation));
			list.Add(new JsonParameter(HueKeys.ALERT, state.alert));
			list.Add(new JsonParameter(HueKeys.EFFECT, state.effect));
			list.Add(new JsonParameter(HueKeys.TRANSITION, state.transitionTime));
			return list.ToArray();
		}

		/// <summary>
		/// Deletes the lamp.
		/// </summary>
		public void Delete()
		{
			HueBridge.instance.DeleteLamp(id, HueErrorInfo.LogError);
		}

		/// <summary>
		/// Maps an RGB color with (1, 1, 1) being complete white
		/// into hue HSV color space with hue going from 0 to 65535,
		/// saturation from 0 to 254 and brightness from 1 to 254.
		/// Keep in mind that the Hue API only accepts ints for those
		/// values.
		/// </summary>
		/// <returns>The HS vfrom RG.</returns>
		/// <param name="rgb">Rgb.</param>
		public static Vector3 HueHSVfromRGB(Color rgb) {
			float brightness, hue, saturation;
			Color.RGBToHSV(rgb, out hue, out saturation, out brightness);
			hue *= 65535f;
			saturation *= 254f;
			brightness *= 254f;
			return new Vector3(hue, saturation, Mathf.Max(brightness, 1f));
		}
	}
}
