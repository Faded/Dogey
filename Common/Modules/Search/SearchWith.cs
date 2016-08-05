using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
        
        public static string StackExchange(string site, string tag, string query)
        {
            const string queryUrl = "https://api.stackexchange.com/2.2/search/advanced?page=1&pagesize=1&order=desc&sort=relevance&q={0}&answers=1&tagged={1}&site={2}";

            Uri queryUri = new Uri(string.Format(queryUrl, Uri.EscapeDataString(query), Uri.EscapeDataString(tag), Uri.EscapeDataString(site)));
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

        public static string StackExchangeTags(string site, string tag)
        {
            const string queryUrl = "https://api.stackexchange.com/2.2/tags?page=1&pagesize=25&order=desc&sort=popular&inname={0}&site=stackoverflow";

            Uri queryUri = new Uri(string.Format(queryUrl, Uri.EscapeDataString(tag), Uri.EscapeDataString(site)));
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
                    return $"I was unable to find any tags like `{tag}`.";
                }
                else
                {
                    var tags = new List<string>();
                    foreach(var tagobj in obj["items"])
                    {
                        tags.Add(tagobj["name"].ToString());
                    }

                    return string.Join(", ", tags);
                }
            }
        }

    }
}
