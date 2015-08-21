using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace _4Chat
{
	class ChatAPI
	{
		public ChatAPI(string key) {
			Key = key;
		}

		public async Task PostMsg(string msg, string name = "", string channel = "") {
			name = name == "" ? Name : name;
			channel = channel == "" ? Channel : channel;
			var args = new Dictionary<string, string> {
				{ "key", Key }, { "name", name }, { "channel", channel }, { "msg", msg }
			};
			var url = "http://ftplovy.olympe.in/4chat/post-msg.php";
			var json = await PostForm(url, args);
			var rCode = (int)json["response_code"].ToObject(typeof(int));
			if (rCode == 1) {
				throw new Exception(json["response"].ToString());
			}
		}

		private static async Task<JObject> PostForm(string url, Dictionary<string, string> args) {
			var client = new HttpClient();

			var response = await client.PostAsync(new Uri(url), new HttpFormUrlEncodedContent(args));
			var responseString = await response.Content.ReadAsStringAsync();

			return JObject.Parse(responseString);
		}

		public string Key { get; }
		public string Channel { get; set; }
		public string Name { get; set; }
	}
}
