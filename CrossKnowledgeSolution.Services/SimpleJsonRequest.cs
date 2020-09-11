using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System;

namespace CrossKnowledgeSolution.Services
{
    public class SimpleJsonRequest
    {
        IDistributedCache cacheService;
        public SimpleJsonRequest(IDistributedCache cacheService)
        {
            this.cacheService = cacheService;
        }
        public async Task<string> GetAsync(string url)
        {
            var cacheData = await cacheService.GetStringAsync(url);
            if (!string.IsNullOrEmpty(cacheData))
                return cacheData;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = await reader.ReadToEndAsync();
                SaveCache(url, result);
                return result;
            }
        }

        private void SaveCache(string key, string data)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            cacheService.SetString($"{key}", data, options);
        }

        public async Task<string> PostAsync(string url, Dictionary<string, string> data)
        {
            return await RequestAsync("POST", url, data);
        }

        public async Task<string> PutAsync(string url, Dictionary<string, string> data)
        {
            return await RequestAsync("PUT", url, data);
        }

        public async Task<string> PatchAsync(string url, Dictionary<string, string> data)
        {
            return await RequestAsync("PATCH", url, data);
        }

        public async Task<string> DeleteAsync(string url, Dictionary<string, string> data)
        {
            return await RequestAsync("DELETE", url, data);
        }

        private async Task<string> RequestAsync(string method, string url, Dictionary<string, string> data)
        {
            var cacheKey = GetCacheKey(method, url, data);
            var cacheData = await cacheService.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheData))
                return cacheData;

            string dataString = JsonConvert.SerializeObject(data);
            byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = "application/json";
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = await reader.ReadToEndAsync();
                SaveCache(cacheKey, result);
                return result;
            }
        }

        private string GetCacheKey(string method, string url, Dictionary<string, string> data)
        {
            return $"{method}-{url}-{String.Join(',', data.Select(item => item.Key + "=" + item.Value))}";
        }
    }
}