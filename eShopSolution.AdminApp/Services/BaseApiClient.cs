using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class BaseApiClient
    {
        private IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor _httpContextAccessor;

        public BaseApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<bool>> OnPostAsync<TRequest>(string url, TRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient("meta");
            HttpResponseMessage response = await client.PostAsync(url, httpContent);
            var data = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return new ApiSuccessResult<bool>();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(data);
        }

        public async Task<TModel> OnGetAsync<TModel>(string url)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TModel>(data);
        }

        public async Task<ApiResult<List<T>>> OnGetListAsync<T>(string url)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();
            var jsonObject = (List<T>)JsonConvert.DeserializeObject(data, typeof(List<T>));
            if (response.IsSuccessStatusCode)
            {
                return new ApiSuccessResult<List<T>>(jsonObject);
            }
            return new ApiResult<List<T>>();
        }

        public async Task<ApiResult<bool>> OnDeleteAsync(string url)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
                return new ApiSuccessResult<bool>();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<bool>> OnPutAsync<TRequest>(string url, TRequest request)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.PutAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
                return new ApiSuccessResult<bool>();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(await response.Content.ReadAsStringAsync());
        }
    }
}