
namespace UnityHue {
	[System.Serializable]
	public class HueBridgeInfo {
		public string id;
		public string name;
		public string ip;
		public string macAdress;
		public string userName;
		public string applicationName;
		public string deviceName;

		public HueBridgeInfo (string id, string name, string ip, string macAdress)
		{
			this.id = id;
			this.name = name;
			this.ip = ip;
			this.macAdress = macAdress;
		}
		public HueBridgeInfo (string id, string ip)
		{
			this.id = id;
			this.ip = ip;
		}
		public HueBridgeInfo ()
		{

		}

		public bool HasIP {
			get {
				return !string.IsNullOrEmpty(ip);
			}
		}
		public bool HasUsername {
			get {
				return !string.IsNullOrEmpty(userName);
			}
		}
	}
}
