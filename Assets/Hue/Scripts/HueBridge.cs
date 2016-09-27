using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

using MiniJSON;

namespace UnityHue{
	/// <summary>
	/// Singleton that interfaces with the Hue API. Generally the HueBridge Component
	/// doesn't have to be present in a scene but can bootstrap itself
	/// </summary>
	[AddComponentMenu("Unity Hue/Hue Bridge")]
	public class HueBridge : UnitySingleton<HueBridge> {
		[Tooltip("Used for discovery of all the bridges in your network" +
			" (that have contacted a philipps server in the past.)")]
		[SerializeField]
		private string hueDiscoveryServer = "https://www.meethue.com/api/nupnp";
		[SerializeField]
		private HueBridgeInfo currentBridge;
		[SerializeField]
		protected List<HueLamp> lamps = new List<HueLamp>();
		[SerializeField]
		protected List<HueGroup> groups = new List<HueGroup>();
		public List<HueBridgeInfo> Bridges { get; private set;}

		void Awake ()
		{
			DontDestroyOnLoad(this);
		}
		#region Public Functions

		/// <summary>
		/// Start discovery of bridges in the current network
		/// OnFinished will be called after a sucessful response 
		/// (even if the response is that zero bridges are in the network).
		/// If there are more than zero bridges in the Network the first one
		/// will be assigned to currentBridge, the full list is accesible under
		/// the Bridges property.
		/// </summary>
		/// <param name="onFinished">On finished.</param>
		public void DiscoverBridges(Action onFinished = null)
		{
			DiscoverBridges(onFinished, HueErrorInfo.LogError);
		}
		public void DiscoverBridges(Action onFinished, Action<HueErrorInfo> errorCallback)
		{
			StartCoroutine(GetBridgesEnumerator(
				x => {
					Bridges = x;
					if(Bridges.Count > 0)
						currentBridge = Bridges[0];
					if(onFinished != null)
						onFinished();
				},
				errorCallback));
		}
			
		public void UpdateLights(Action onFinished = null)
		{
			UpdateLights(onFinished, HueErrorInfo.LogError);
		}
		public void UpdateLights(Action onFinished, Action<HueErrorInfo> errorCallback)
		{
			DiscoverLights(x => {
				lamps = x; 
				if(onFinished != null)
					onFinished();
			}, errorCallback);
		}
		public void UpdateGroups(Action onFinished = null)
		{
			UpdateGroups(onFinished, HueErrorInfo.LogError);
		}
		public void UpdateGroups(Action onFinished, Action<HueErrorInfo> errorCallback)
		{
			DiscoverGroups(x => {
				groups = x;
				if(onFinished != null)
					onFinished();
			}, errorCallback);
		}
		public void DiscoverLights(Action<List<HueLamp>> lampsCallback, Action<HueErrorInfo> errorCallback)
		{
			StartCoroutine(DiscoverLightsEnumerator(lampsCallback, errorCallback));
		}
		public void DiscoverGroups(Action<List<HueGroup>> groupsCallBack, Action<HueErrorInfo> errorCallback)
		{
			StartCoroutine(DiscoverGroupsEnumerator(groupsCallBack, errorCallback));
		}
		public void CreateUser(Action onFinished = null, Action<HueErrorInfo> errorCallback = null)
		{
			CreateUser((x) => {
				currentBridge.userName = x;
				if(onFinished != null)
					onFinished();
			}, errorCallback);
		}
		public void CreateUser(string applicationName, string deviceName, Action onFinished = null, Action<HueErrorInfo> errorCallback = null)
		{
			CreateUser(applicationName, deviceName, 
				(x) => {
				currentBridge.userName = x;
				if(onFinished != null)
					onFinished();
			}, errorCallback);
		}
		public void CreateUser(Action<string> generatedUsername, Action<HueErrorInfo> errorCallback)
		{
			CreateUser(currentBridge.applicationName, currentBridge.deviceName, generatedUsername, errorCallback);
		}
		public void CreateUser(string applicationName, string deviceName, Action<string> generatedUsername,
			Action<HueErrorInfo> errorCallback)
		{
			StartCoroutine(CreateUserEnumerator(applicationName, deviceName, generatedUsername, errorCallback));
		}

		/// <summary>
		/// Deletes the lamp, careful with this one
		/// </summary>
		/// <param name="lampName">Lamp name.</param>
		/// <param name="successCallback">Success callback.</param>
		/// <param name="errorCallback">Error callback.</param>
		public void DeleteLamp(string id, Action<HueErrorInfo> errorCallback = null)
		{
			StartCoroutine(DeleteLampEnumerator(id, errorCallback));
		}
		/// <summary>
		/// Deletes a group, careful with this one
		/// </summary>
		/// <param name="lampName">Lamp name.</param>
		/// <param name="successCallback">Success callback.</param>
		/// <param name="errorCallback">Error callback.</param>
		public void DeleteGroup(string id, Action<HueErrorInfo> errorCallback = null)
		{
			StartCoroutine(DeleteGroupEnumerator(id, errorCallback));
		}
		public void UpdateLamp(string id, HueLamp lampToUpdate, Action<HueErrorInfo> errorCallback = null)
		{
			StartCoroutine(UpdateLampEnumerator(id, lampToUpdate, errorCallback));
		}
		public string GetHueStateString()
		{
			return JsonUtility.ToJson(GetStorableHueState());
		}
		public void RestoreHueFromString(string jsonContent)
		{
			if(string.IsNullOrEmpty(jsonContent))
				return;
			RestoreHueState(JsonUtility.FromJson<StoredHueInfo>(jsonContent));
		}
		public StoredHueInfo GetStorableHueState()
		{
			return new StoredHueInfo(currentBridge, Bridges);
		}
		public void RestoreHueState( StoredHueInfo savedState)
		{
			if(savedState == null)
				return;
			Bridges = savedState.allBridges;
			currentBridge = savedState.current;
		}

		#endregion

		#region Private Functions

		void ProcessBridges(string jsonResponse, Action<List<HueBridgeInfo>> ipCallback)
		{
			var list = new List<HueBridgeInfo>();
			var response = Json.Deserialize(jsonResponse);
			if(response is List<System.Object>)
			{
				var responseList = response as List<System.Object>;
				foreach(var item in responseList)
				{
					if(!(item is Dictionary<string, System.Object>))
						continue;
					var dict = item as Dictionary<string, System.Object>;
					var bridgeInfo = new HueBridgeInfo(dict["id"].ToString(), dict["internalipaddress"].ToString());
					if(dict.ContainsKey("macaddress"))
						bridgeInfo.macAdress = dict["macaddress"].ToString();
					if(dict.ContainsKey("name"))
						bridgeInfo.name = dict["name"].ToString();
					list.Add(bridgeInfo);
				}
			}
			ipCallback(list);
		}

		void ProcessLights (string jsonResponse, Action<List<HueLamp>> lampsCallback, Action<HueErrorInfo> errorCallback)
		{
			var response = Json.Deserialize(jsonResponse);
			if(HueErrorInfo.JsonContainsErrorKey(response))
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(response));
				return;
			}
			if(!(response is Dictionary<string, System.Object>))
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(null, jsonResponse));
				return;
			}
			var dict = response as Dictionary<string, System.Object>;
			var list = new List<HueLamp>();
			foreach(var kv in dict)
			{
				string id = kv.Key;
				if(!(kv.Value is Dictionary<string, System.Object>))
					continue;
				var lightDict = kv.Value as Dictionary<string, System.Object>;
				var lamp = new HueLamp(id);
				ProcessLampUpdate(lightDict, lamp);
				list.Add(lamp);
			}
			lampsCallback(list);
		}
		void ProcessGroups (string jsonResponse, Action<List<HueGroup>> groupCallBack, Action<HueErrorInfo> errorCallback)
		{
			var response = Json.Deserialize(jsonResponse);
			if(HueErrorInfo.JsonContainsErrorKey(response))
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(response));
				return;
			}
			if(!(response is Dictionary<string, System.Object>))
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(null, jsonResponse));
				return;
			}
			var dict = response as Dictionary<string, System.Object>;
			var list = new List<HueGroup>();
			foreach(var kv in dict)
			{
				string id = kv.Key;
				if(!(kv.Value is Dictionary<string, System.Object>))
					continue;
				var groupDict = kv.Value as Dictionary<string, System.Object>;
				string name = groupDict[HueKeys.NAME].ToString();
				var group = new HueGroup(name, id);
				list.Add(group);
			}
			groupCallBack(list);
		}

		void ProcessLampUpdate(string jsonResponse, HueLamp lampToUpdate, Action<HueErrorInfo> errorCallback)
		{
			var response = Json.Deserialize(jsonResponse);
			if(HueErrorInfo.JsonContainsErrorKey(response))
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(response));
				return;
			}
			if(!(response is Dictionary<string, System.Object>))
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(null, jsonResponse));
				return;
			}
			var dict = response as Dictionary<string, System.Object>;
			ProcessLampUpdate(dict, lampToUpdate);
		}
		void ProcessLampUpdate(Dictionary<string, System.Object> dict, HueLamp lampToUpdate)
		{
			lampToUpdate.name = dict[HueKeys.NAME].ToString();
			lampToUpdate.modelID = dict[HueKeys.MODEL_ID].ToString();
			lampToUpdate.type = dict[HueKeys.TYPE].ToString();
			lampToUpdate.softwareVersion = dict[HueKeys.SOFTWARE_VERSION].ToString();
			var stateDict = dict[HueKeys.STATE] as Dictionary<string, System.Object>;
			lampToUpdate.lampState = GetStateFromDictionary(stateDict);
		}

		#endregion

		#region Request Enumerators

		IEnumerator GetBridgesEnumerator(Action<List<HueBridgeInfo>> ipCallback, Action<HueErrorInfo> errorCallback = null)
		{
			UnityWebRequest bridgesWebRequest = UnityWebRequest.Get(hueDiscoveryServer);
			yield return bridgesWebRequest.Send();

			if(bridgesWebRequest.isError) 
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(bridgesWebRequest.error, null));
			}
			else {
				ProcessBridges(bridgesWebRequest.downloadHandler.text, ipCallback);
			}
		}

		IEnumerator CreateUserEnumerator(string applicationName, string deviceName,
			Action<string> generatedUserName, Action<HueErrorInfo> errorCallback = null)
		{
			string json = JsonHelper.CreateJsonParameterString(
				new JsonParameter(HueKeys.DEVICE_TYPE, applicationName   + "#" + deviceName)
			);
			//Unity Webrequest making this more difficult than it should be
			var createUserRequest = UnityWebrequestHelper.NonURLEncodedPost(BaseURL, json);
			yield return createUserRequest.Send();

			if(createUserRequest.isError) 
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(createUserRequest.error, null));
			}
			else {
				var response = Json.Deserialize(createUserRequest.downloadHandler.text);
				if(HueErrorInfo.JsonContainsErrorKey(response))
				{
					if(errorCallback != null)
						errorCallback(new HueErrorInfo(response));

				}else
				{
					var userName = JsonHelper.UnravelJson(response, HueKeys.USER_NAME);
					if(userName != null)
						generatedUserName(userName.ToString());
				}
			}
		}

		IEnumerator DeleteLampEnumerator (string id, Action<HueErrorInfo> errorCallback = null)
		{
			string url = BaseURLWithUserName + "/lights/" + id;
			UnityWebRequest deleteRequest = UnityWebRequest.Delete(url);
			yield return deleteRequest.Send();
			if(deleteRequest.isError && errorCallback != null)
				errorCallback(new HueErrorInfo(deleteRequest.error, null));
		}
		IEnumerator DeleteGroupEnumerator (string id, Action<HueErrorInfo> errorCallback = null)
		{
			string url = BaseURLWithUserName + "/groups/" + id;
			UnityWebRequest deleteRequest = UnityWebRequest.Delete(url);
			yield return deleteRequest.Send();
			if(deleteRequest.isError && errorCallback != null)
				errorCallback(new HueErrorInfo(deleteRequest.error, null));
		}

		IEnumerator UpdateLampEnumerator (string id, HueLamp lampToUpdate, 
			Action<HueErrorInfo> errorCallback = null)
		{
			string url = BaseURLWithUserName + "/lights/" + id;
			UnityWebRequest stateRequest = UnityWebRequest.Get(url);
			yield return stateRequest.Send();
			if(stateRequest.isError)
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(stateRequest.error, null));
			}else
			{
				ProcessLampUpdate(stateRequest.downloadHandler.text, lampToUpdate, errorCallback);
			}
		}

		IEnumerator DiscoverLightsEnumerator(Action<List<HueLamp>> lampsCallback, Action<HueErrorInfo> errorCallback) {
			UnityWebRequest lightsRequest = UnityWebRequest.Get(BaseURL+"/"+currentBridge.userName+"/lights");
			yield return lightsRequest.Send();

			if(lightsRequest.isError) 
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(lightsRequest.error, null));
			}
			else {
				ProcessLights(lightsRequest.downloadHandler.text, lampsCallback, errorCallback);
			}
		}
		IEnumerator DiscoverGroupsEnumerator(Action<List<HueGroup>> groups, Action<HueErrorInfo> errorCallback) {
			UnityWebRequest groupsRequest = UnityWebRequest.Get(BaseURL+"/"+currentBridge.userName+"/groups");
			yield return groupsRequest.Send();

			if(groupsRequest.isError) 
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(groupsRequest.error, null));
			}
			else {
				ProcessGroups(groupsRequest.downloadHandler.text, groups, errorCallback);
			}
		}

		IEnumerator SendRequestEnumerator(UnityWebRequest request, Action<string> successCallback, 
			Action<HueErrorInfo> errorCallback = null)
		{
			yield return request.Send();

			if(request.isError) 
			{
				if(errorCallback != null)
					errorCallback(new HueErrorInfo(request.error, null));
			}
			else {
				if(successCallback != null)
					successCallback(request.downloadHandler.text);
			}
		}

		#endregion

		#region Helper

		public string BaseURL{
			get {
				return "http://" + currentBridge.ip + "/api";
			}
		}
		public string BaseURLWithUserName{
			get {
				return "http://" + currentBridge.ip + "/api/"+currentBridge.userName;
			}
		}
		public List<HueLamp> Lights {
			get {
				return lamps;
			}
		}
		public List<HueGroup> Groups {
			get {
				return groups;
			}
		}
		public HueBridgeInfo CurrentBridge {
			get {
				return currentBridge;
			}
		}

		HueLampState GetStateFromDictionary(Dictionary<string, System.Object> dict)
		{
			var state = new HueLampState();
			state.on = (bool)dict[HueKeys.ON];
			state.reachable = (bool)dict[HueKeys.REACHABLE];
			state.hue =  int.Parse(dict[HueKeys.HUE].ToString());
			state.brightness = int.Parse(dict[HueKeys.BRIGHTNESS].ToString());
			state.saturation = int.Parse(dict[HueKeys.SATURATION].ToString());
			state.colorMode = dict[HueKeys.COLOR_MODE].ToString();
			state.effect = dict[HueKeys.EFFECT].ToString();
			state.alert = dict[HueKeys.ALERT].ToString();
			return state;
		}

		public void SendRequest(UnityWebRequest request, Action<string> successCallback, Action<HueErrorInfo> errorCallback = null)
		{
			StartCoroutine(SendRequestEnumerator(request, successCallback, errorCallback));
		}

		#endregion
	}
}