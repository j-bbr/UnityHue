using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;

namespace UnityHue{
	/// <summary>
	/// Stores information about the error that occured when performing an operation
	/// with Unity Hue. Can either be an error with the webrequest (webrequest error field)
	/// with the hue api (errorcode, address, description) or with unexpected json (failingJson)
	/// </summary>
	[System.Serializable]
	public class HueErrorInfo{
		public string webrequestError;
		/// <summary>
		/// The error code, in case this was a hue api error. A list with error codes can be
		/// found here: http://www.developers.meethue.com/documentation/error-messages
		/// </summary>
		public int errorCode;
		public string address;
		public string description;
		public string failingJson;

		public HueErrorInfo (string webrequestError, string failingJson = null)
		{
			this.webrequestError = webrequestError;
			this.failingJson = failingJson;
		}

		public HueErrorInfo (System.Object jsonObject)
		{
			var errorObject = JsonHelper.UnravelJson(jsonObject, "error");
			var	dict = errorObject as Dictionary<string, System.Object>;
			
			if(dict != null)
			{
				if(dict.ContainsKey(HueKeys.TYPE))
					this.errorCode = int.Parse(dict[HueKeys.TYPE].ToString());
				if(dict.ContainsKey(HueKeys.ADDRESS))
					this.address = dict[HueKeys.ADDRESS].ToString();
				if(dict.ContainsKey(HueKeys.DESCRIPTION))
					this.description = dict[HueKeys.DESCRIPTION].ToString();
			}
		}
		/// <summary>
		/// Standard way of handling the error. Simply logs all 
		/// error information to the console
		/// </summary>
		/// <param name="error">Error.</param>
		public static void LogError(HueErrorInfo error)
		{
			Debug.LogWarning(error.ToString());
		}
		public override string ToString ()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Unity Hue encountered an error with the following details: ").
			AppendLine("Webrequest Error : " + webrequestError).
			AppendLine("Error Code : " + errorCode.ToString()).
			AppendLine("Adress : " + address).
			AppendLine("Description : " + description).
			AppendLine("Non-Decoding JSON : " + failingJson);
			return builder.ToString();
		}
		public static bool JsonContainsErrorKey (System.Object json)
		{
			return JsonHelper.UnravelJson(json, HueKeys.ERROR) != null;
		}

		public bool IsRequestError {
			get {
				return !string.IsNullOrEmpty(webrequestError);
			}
		}
		public bool IsJsonDecodeError {
			get {
				return !string.IsNullOrEmpty(failingJson);
			}
		}
		public bool IsHueAPIError {
			get {
				return !string.IsNullOrEmpty(description);
			}
		}
	}
}
