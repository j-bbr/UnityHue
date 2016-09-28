namespace UnityHue{
	/// <summary>
	/// A struct with a key and an object value. One or more JsonParameter
	/// can then be used to constructe a Json dictionary with the helper
	/// functions in JsonHelper. Has constructors that automatically box
	/// common types (bool, int, float, string) into an object
	/// </summary>
	public struct JsonParameter {
		public string parameterKey;
		public System.Object parameterValue;


		public JsonParameter (string parameterKey, System.Object parameterValue)
		{
			this.parameterKey = parameterKey;
			this.parameterValue = parameterValue;
		}
		public JsonParameter (string parameterKey, bool parameterValue)
		{
			this.parameterKey = parameterKey;
			this.parameterValue = parameterValue as System.Object;
		}
		public JsonParameter (string parameterKey, float parameterValue)
		{
			this.parameterKey = parameterKey;
			this.parameterValue = parameterValue as System.Object;
		}
		public JsonParameter (string parameterKey, int parameterValue)
		{
			this.parameterKey = parameterKey;
			this.parameterValue = parameterValue as System.Object;
		}
		public JsonParameter (string parameterKey, string parameterValue)
		{
			this.parameterKey = parameterKey;
			this.parameterValue = parameterValue as System.Object;
		}
		public JsonParameter (string parameterKey, params JsonParameter[] parameterValue)
		{
			this.parameterKey = parameterKey;
			this.parameterValue = JsonHelper.CreateJsonParameterDictionary(parameterValue) as System.Object;
		}
	}
}