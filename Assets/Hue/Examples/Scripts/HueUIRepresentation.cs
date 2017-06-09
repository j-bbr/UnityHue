using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityHue;

namespace UnityHue.Examples{
	public class HueUIRepresentation : MonoBehaviour {
		private HueLamp lamp;
		public Text nameText;
		public Toggle onToggle;
		public Slider hueSlider;
		public Slider brightnessSlider;
		public Slider saturationSlider;
		public Slider transitionTime;
		private bool effectActive;

		public void Initialize(HueLamp lamp)
		{
			this.lamp = lamp;
			nameText.text = lamp.name;
		}

		public void SetState()
		{
			if(lamp == null)
				return;
			lamp.SetState(
				HueParameters.LightOnParameter(onToggle.isOn),
				HueParameters.BrightnessParameter((int) brightnessSlider.value),
				HueParameters.HueParameter((int) hueSlider.value),
				HueParameters.SaturationParameter((int) saturationSlider.value),
				HueParameters.TransitionParameter((int) transitionTime.value)
			);
		}
		public void SetColorLoop()
		{
			if(lamp == null)
				return;
			effectActive = !effectActive;
			lamp.SetState(
				HueParameters.EffectParameter(effectActive ? "colorloop" : "none")
			);
		}
		public void SetBlink()
		{
			if(lamp == null)
				return;
			lamp.SetState(
				HueParameters.AlertParameter()
			);
		}
		public void SetRacingColors(float timeOut = 1f)
		{
			if(lamp == null)
				return;
			StartCoroutine(RacingCountdown(timeOut));
		}
		IEnumerator RacingCountdown(float timeOut = 1f)
		{
			//Change the color instantly (no transition time)
			lamp.SetColor(Color.red, HueParameters.TransitionParameter(0));
			yield return new WaitForSeconds(timeOut);
			lamp.SetColor(Color.yellow, HueParameters.TransitionParameter(0));
			yield return new WaitForSeconds(timeOut);
			lamp.SetColor(Color.green,HueParameters.TransitionParameter(0));
		}
	}
}