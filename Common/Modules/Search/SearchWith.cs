using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public static class SearchWith
    {
        public static string Youtube(string query)
        {
            const string queryUrl = "https://www.youtube.com/watch?v={0}";
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Program.config.Token.Google,
                ApplicationName = "Dogey"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 1;

            var searchListResponse = searchListRequest.Execute();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    return string.Format(queryUrl, searchResult.Id.VideoId);
                }
            }
            return $"I was unable to find a video on youtube like `{query}`.";
        }
        
        public static string StackExchange(string tag, string query)
        {
            const string queryUrl = "http://api.stackexchange.com/2.2/search?pagesize=1&order=desc&sort=votes&tagged={0}&intitle={1}&site=stackoverflow";

            Uri queryUri = new Uri(string.Format(queryUrl, Uri.EscapeDataString(tag), Uri.EscapeDataString(query)));
            var request = (HttpWebRequest)HttpWebRequest.Create(queryUri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            var response = request.GetResponse();

            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string json = reader.ReadToEnd();

                var obj = JObject.Parse(json);

                if (!obj["items"].HasValues)
                {
                    return $"I was unable to find a question with the tag `{tag}` like `{query}`.";
                }
                else
                {
                    return obj["items"][0]["link"].ToString();
                }
            }
        }
    }
}
