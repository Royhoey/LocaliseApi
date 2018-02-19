using LocaliseApi.Interfaces;
using LocaliseApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocaliseRepository
{
	public class LocaliseRepository : ILocaliseRepository
	{
		private static readonly string _baseApiUri = "https://localise.biz/api";

		private readonly HttpClient _httpClient;
		private readonly string _apiKey;

		public LocaliseRepository(string apiKey)
		{
			_apiKey = apiKey;
			_httpClient = new HttpClient();
		}

		public async Task<IEnumerable<Locale>> GetLocales()
		{
			var uri = $"{_baseApiUri}/locales?key={_apiKey}";
			var response = await _httpClient.GetAsync(uri);

			return JsonConvert.DeserializeObject<List<Locale>>(await response.Content.ReadAsStringAsync());
		}

		public async Task<TranslationData> GetTranslation(string id, string locale)
		{
			var uri = $"{_baseApiUri}/translations/{id}/{locale}?key={_apiKey}";
			var response = await _httpClient.GetAsync(uri);

			return JsonConvert.DeserializeObject<TranslationData>(await response.Content.ReadAsStringAsync());
		}

		public async Task<IEnumerable<TranslationData>> GetTranslations(string id)
		{
			var uri = $"{_baseApiUri}/translations/{id}?key={_apiKey}";
			var response = await _httpClient.GetAsync(uri);

			return JsonConvert.DeserializeObject<List<TranslationData>>(await response.Content.ReadAsStringAsync());
		}

		public async Task AddTranslation(string id, Locale locale, string translation)
		{
			var uri = $"{_baseApiUri}/translations/{id}/{locale.Code}/?key={_apiKey}";

			var response = await _httpClient.PostAsync(uri, new StringContent(translation));

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error while adding translation '{translation}' for asset '{id}'. Response status code: {response.StatusCode}. Reason phrase: {response.ReasonPhrase}");
			}
		}

		public async Task<bool> AssetExists(string id)
		{
			var uri = $"{_baseApiUri}/assets/{id}?key={_apiKey}";
			var response = await _httpClient.GetAsync(uri);

			return response.IsSuccessStatusCode;
		}

		public async Task CreateAsset(string name, string id, List<string> tags, string type = "text")
		{
			var uri = $"{_baseApiUri}/assets?key={_apiKey}";

			var requestHeaders = new Dictionary<string, string> {
				{ "name", name },
				{ "id", id },
				{ "type", type }
			};

			var response = await _httpClient.PostAsync(uri, new FormUrlEncodedContent(requestHeaders));

			if (tags.Count > 0)
			{
				var tagTasks = new List<Task>();
				tags.ForEach(t => tagTasks.Add(TagAsset(id, t)));

				Task.WaitAll(tagTasks.ToArray());
			}

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error while creating asset '{id}'. Response status code: {response.StatusCode}. Reason phrase: {response.ReasonPhrase}");
			}
		}
		
		public async Task TagAsset(string id, string tag)
		{
			var uri = $"{_baseApiUri}/assets/{id}/tags?key={_apiKey}";

			var requestHeaders = new Dictionary<string, string> {
				{ "name", tag },
				{ "id", id }
			};

			var response = await _httpClient.PostAsync(uri, new FormUrlEncodedContent(requestHeaders));
			
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error while tagging asset '{id}' with tag '{tag}'. Response status code: {response.StatusCode}. Reason phrase: {response.ReasonPhrase}");
			}
		}		
	}
}
