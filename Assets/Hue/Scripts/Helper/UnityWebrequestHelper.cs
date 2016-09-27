using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class UnityWebrequestHelper {

	/// <summary>
	/// Non URL encoded post request for a json submit.
	/// </summary>
	/// <returns>The URL encoded post request.</returns>
	/// <param name="url">URL.</param>
	/// <param name="postData">Post data.</param>
	public static UnityWebRequest NonURLEncodedPost(string url, string postData)
	{
		var request = new UnityWebRequest(url, "POST");
		byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
		request.uploadHandler = new UploadHandlerRaw(bodyRaw) as UploadHandler;
		request.downloadHandler = new DownloadHandlerBuffer() as DownloadHandler;
		request.SetRequestHeader("Content-Type", "application/json");
		return request;
	}
}
