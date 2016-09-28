using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace UnityHue {
	public static class JsonHelper {
		//finds the first occurence of an object in a dictionary with a supplied key
		//a bit ugly but gets the job done quick most of the time
		public static System.Object UnravelJson(System.Object obj, string keyToLookFor)
		{
			if(obj is List<System.Object>)
			{
				var list = obj as List<System.Object>;
				foreach(var item in list)
				{
					var result = UnravelJson(item, keyToLookFor);
					if(result != null)
						return result;
				}
				return null;
			}else if(obj is Dictionary<string, System.Object>)
			{
				return RecurseThroughDictionary(obj as Dictionary<string, System.Object>, keyToLookFor);
			}else
				return null;
		}
		public static System.Object RecurseThroughDictionary (Dictionary<string, System.Object> dict, string keyToLookFor)
		{
			if(dict.ContainsKey(keyToLookFor))
				return dict[keyToLookFor];

			foreach(var entry in dict)
			{
				var result = UnravelJson(entry.Value, keyToLookFor);
				if(result != null)
					return result;
			}
			return null;
		}
		public static void RecurseThroughDictionary (Dictionary<string, System.Object> dict, string keyToLookFor,
			ref List<System.Object> results)
		{
			if(dict.ContainsKey(keyToLookFor))
				results.Add(dict[keyToLookFor]);

			foreach(var entry in dict)
			{
				UnravelJson(entry.Value, keyToLookFor, ref results);
			}
		}
		/// <summary>
		/// Gets a list of all the objects with the supplied keys somewhere in the json
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="keyToLookFor">Key to look for.</param>
		/// <param name="results">Results.</param>
		public static void UnravelJson(System.Object obj, string keyToLookFor, ref List<System.Object> results)
		{
			if(obj is List<System.Object>)
			{
				var list = obj as List<System.Object>;
				foreach(var item in list)
				{
					UnravelJson(item, keyToLookFor, ref results);
				}
			}else if(obj is Dictionary<string, System.Object>)
			{
				RecurseThroughDictionary(obj as Dictionary<string, System.Object>, keyToLookFor);
			}
		}

		public static Dictionary<string, System.Object> CreateJsonParameterDictionary(params JsonParameter[] parameters)
		{
			var dict = new Dictionary<string, System.Object>();
			foreach(var parameter in parameters)
			{
				dict.Add(parameter.parameterKey, parameter.parameterValue);
			}
			return dict;
		}
		public static string CreateJsonParameterString(params JsonParameter[] parameters)
		{
			return Json.Serialize(CreateJsonParameterDictionary(parameters));
		}
			
	}
}
