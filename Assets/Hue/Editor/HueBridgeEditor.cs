using UnityEngine;
using UnityEditor;

namespace UnityHue{
	[CustomEditor(typeof(HueBridge), true)]
	[CanEditMultipleObjects]
	public class HueBridgeEditor : Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			HueBridge script = target as HueBridge;

			if(GUILayout.Button("Discover Bridge", GUILayout.Height(30)))
			{
				script.DiscoverBridges();
			}

			GUILayout.Space(7f);

			if(script.CurrentBridge == null || string.IsNullOrEmpty(script.CurrentBridge.ip))
			{
				EditorGUILayout.HelpBox("The current bridge doesn't have an ip", MessageType.Warning);
			}
			else
			{
				if(GUILayout.Button("Get Lights", GUILayout.Height(30)))
				{	
					script.UpdateLights();
				}

				GUILayout.Space(7f);

				if(GUILayout.Button("Get Groups", GUILayout.Height(30)))
				{
					script.UpdateGroups();
				}

				GUILayout.Space(7f);

				if(string.IsNullOrEmpty(script.CurrentBridge.applicationName) || string.IsNullOrEmpty(script.CurrentBridge.deviceName))
				{
					EditorGUILayout.HelpBox("The current bridge doesn't have application name and device name", MessageType.Warning);
				}else
				{
					if(string.IsNullOrEmpty(script.CurrentBridge.applicationName))
					{
						EditorGUILayout.HelpBox("Their is already a username that will be overwritten if you create a new user", MessageType.Info);
						GUILayout.Space(7f);
					}
					if(GUILayout.Button("Create User", GUILayout.Height(30)))
						script.CreateUser();
				}
			}
		}
	}
}
