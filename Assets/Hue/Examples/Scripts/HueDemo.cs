using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityHue;

namespace UnityHue.Examples{
	public class HueDemo : MonoBehaviour {
		public HueInfoStorer storer;
		public GameObject hueUIRepresentationPrefab;
		public GameObject createUserScreen;
		public Text createUserText;
		public Button createUserButton;
		public RectTransform lampMenu;
		public string applicationName = "UHue", deviceName ="MyHue";


		void Awake () {
			//Either the ip and username are stored in the monobehaviour or it was succesfully restored from player prefs
			if((HueBridge.instance.CurrentBridge != null && HueBridge.instance.CurrentBridge.HasIP && HueBridge.instance.CurrentBridge.HasUsername)|| 
				storer.Restore())
			{
				HueBridge.instance.UpdateLights(OnLightsRetrieved, HandleLightsError);
			}else
			{
				//We have to discover the bridges
				HueBridge.instance.DiscoverBridges(OnBridgesDiscovered);
			}
		}
		public void OnLightsRetrieved()
		{
			createUserScreen.SetActive(false);
			foreach(var lamp in HueBridge.instance.Lights)
			{
				GameObject representation = Instantiate(hueUIRepresentationPrefab, lampMenu) as GameObject;
				representation.GetComponent<HueUIRepresentation>().Initialize(lamp);
			}
			storer.Save();
		}
		public void OnBridgesDiscovered()
		{
			createUserScreen.SetActive(true);
			if(HueBridge.instance.Bridges.Count < 1)
			{
				createUserText.text = "Couldn't find any bridges in your Network";
				createUserButton.gameObject.SetActive(false);
				Debug.LogWarning("Failed to find Bridges in your Network");
			}else
				createUserButton.gameObject.SetActive(true);
				
		}
		public void RegisterApp()
		{
			HueBridge.instance.CreateUser(applicationName, deviceName, ()=> HueBridge.instance.UpdateLights(OnLightsRetrieved), OnRegistrationError);
			createUserButton.gameObject.SetActive(false);
		}
		public void HandleLightsError(HueErrorInfo error)
		{
			Debug.LogWarning(error);
			if(!error.IsRequestError)
				return;
			Debug.Log( "Connecting to a previously stored hue failed, trying to discover new bridges");
			HueBridge.instance.DiscoverBridges(OnBridgesDiscovered);
		}

		public void OnRegistrationError(HueErrorInfo error)
		{
			if(error.errorCode == 101)
			{
				createUserText.text = "The Link Button on the Bridge wasn't pressed. Press it and try again";
				createUserButton.gameObject.SetActive(true);
			}else
				HueErrorInfo.LogError(error);
				
		}
	}
}