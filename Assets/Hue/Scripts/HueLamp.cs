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
			ColorParameter(color, out hue, out sat, out bri);
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

		public static Vector3 HSVFromRGB(Color rgb) {
			float max = Mathf.Max(rgb.r, Mathf.Max(rgb.g, rgb.b));
			float min = Mathf.Min(rgb.r, Mathf.Min(rgb.g, rgb.b));
			
			float brightness = rgb.a;
			
			float hue, saturation;
			if (max == min) {
				hue = 0f;
				saturation = 0f;
			} else {
				float c = max - min;
				if (max == rgb.r) {
					hue = (rgb.g - rgb.b) / c;
				} else if (max == rgb.g) {
					hue = (rgb.b - rgb.r) / c + 2f;
				} else {
					hue = (rgb.r - rgb.g) / c + 4f;
				}

				hue *= 60f;
				if (hue < 0f) {
					hue += 360f;
				}
				
				saturation = c / max;
			}

			return new Vector3(hue, saturation, brightness);
		}
		#region Parameters
		/// <summary>
		/// Transforms a RGB into color into the corresponding hue, brightness and saturation
		/// parameters for the Hue lamp
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="hue">Hue.</param>
		/// <param name="saturation">Saturation.</param>
		/// <param name="brightness">Brightness.</param>
		public static void ColorParameter(Color color, out JsonParameter hue, out JsonParameter saturation, out JsonParameter brightness)
		{
			Vector3 hsv = HSVFromRGB(color);
			hue = new JsonParameter(HueKeys.HUE, hsv.x);
			saturation = new JsonParameter(HueKeys.SATURATION, hsv.y);
			brightness = new JsonParameter(HueKeys.BRIGHTNESS, hsv.z);
		}

		public static JsonParameter LightOnParameter(bool on)
		{
			return new JsonParameter(HueKeys.ON, on);
		}	public static JsonParameter BrightnessParameter(int brightness)
		{
			return new JsonParameter(HueKeys.BRIGHTNESS, brightness);
		}
		public static JsonParameter HueParameter(int hue)
		{
			return new JsonParameter(HueKeys.HUE, hue);
		}
		public static JsonParameter SatParameter(int sat)
		{
			return new JsonParameter(HueKeys.SATURATION, sat);
		}
		/// <summary>
		/// Creates an transitiontime parameter. This sets the duration of the transition
		/// between the current and the new state as a multiple of 100 ms so the default
		/// transitionTime of 4 results in a 400ms transition
		/// </summary>
		/// <returns>The parameter.</returns>
		/// <param name="transitionTime">Transition time.</param>
		public static JsonParameter TransitionParameter(int transitionTime = 4)
		{
			return new JsonParameter(HueKeys.TRANSITION, transitionTime);
		}
		/// <summary>
		/// Creates an effect parameter. Options currently are "none" and "colorloop" cycling 
		/// through the hue range with current brightness and saturation
		/// </summary>
		/// <returns>The parameter.</returns>
		/// <param name="alertType">Alert type.</param>
		public static JsonParameter EffectParameter(string effectType = HueKeys.COLOR_LOOP)
		{
			return new JsonParameter(HueKeys.EFFECT, effectType);
		}
		/// <summary>
		/// Creates an alert parameter. Options currently are "none", "select" performing one 
		/// breath cycle and "lselect" performing breath cycles for 15 seconds
		/// </summary>
		/// <returns>The parameter.</returns>
		/// <param name="alertType">Alert type.</param>
		public static JsonParameter AlertParameter(string alertType = HueKeys.SELECT)
		{
			return new JsonParameter(HueKeys.ALERT, alertType);
		}

		#endregion
	}
}
