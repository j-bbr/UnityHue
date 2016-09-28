using UnityEngine;
using System.Collections;

namespace UnityHue{
	/// <summary>
	/// Commonly used parameters for Hue API calls
	/// </summary>
	public static class HueParameters {

		/// <summary>
		/// Transforms a RGB into color into the corresponding hue, brightness and saturation
		/// parameters for the Hue lamp.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="hue">Hue.</param>
		/// <param name="saturation">Saturation.</param>
		/// <param name="brightness">Brightness.</param>
		public static void ColorParameter(Color color, out JsonParameter hue, out JsonParameter saturation, out JsonParameter brightness)
		{
			Vector3 hsv = HueLamp.HueHSVfromRGB(color);
			hue = HueParameter(Mathf.RoundToInt(hsv.x));
			saturation = SaturationParameter(Mathf.RoundToInt(hsv.y));
			brightness = BrightnessParameter(Mathf.RoundToInt(hsv.z));
		}

		public static JsonParameter LightOnParameter(bool on)
		{
			return new JsonParameter(HueKeys.ON, on);
		}	
		public static JsonParameter BrightnessParameter(int brightness)
		{
			return new JsonParameter(HueKeys.BRIGHTNESS, brightness);
		}
		public static JsonParameter HueParameter(int hue)
		{
			return new JsonParameter(HueKeys.HUE, hue);
		}
		public static JsonParameter SaturationParameter(int sat)
		{
			return new JsonParameter(HueKeys.SATURATION, sat);
		}
		/// <summary>
		/// Creates a transitiontime parameter. This sets the duration of the transition
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
	}
}
