using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityHue;

public class HueUIRepresentation : MonoBehaviour {
	private HueLamp lamp;
	public Text nameText;
	public Toggle onToggle;
	public Slider hueSlider;
	public Slider brightnessSlider;
	public Slider saturationSlider;
	public Slider transitionTime;

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
			HueLamp.LightOnParameter(onToggle.isOn),
			HueLamp.BrightnessParameter((int) brightnessSlider.value),
			HueLamp.HueParameter((int) hueSlider.value),
			HueLamp.SatParameter((int) saturationSlider.value),
			HueLamp.TransitionParameter((int) transitionTime.value)
		);
	}
	public void SetColorLoop()
	{
		if(lamp == null)
			return;
		lamp.SetState(
			HueLamp.EffectParameter()
		);
	}
	public void SetBlink()
	{
		if(lamp == null)
			return;
		lamp.SetState(
			HueLamp.AlertParameter()
		);
	}
}
