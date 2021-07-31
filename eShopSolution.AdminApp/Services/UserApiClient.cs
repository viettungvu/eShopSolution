using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class UserApiClient : IUserApiClient
    {
        private IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor _httpContextAccessor;

        public UserApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            HttpClient client = _httpClientFactory.CreateClient("meta");
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("users/authenticate", httpContent);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<bool>> Create(RegisterRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            HttpClient client = _httpClientFactory.CreateClient("meta");
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("users/create", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(UserPagingRequest request)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.GetAsync($"users/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserVm>>>(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<ApiErrorResult<PagedResult<UserVm>>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.DeleteAsync($"users/{id}");
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.PutAsync($"users/{id}", httpContent);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.GetAsync($"users/{id}");
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<UserVm>>(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<ApiErrorResult<UserVm>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = _httpClientFactory.CreateClient("meta");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            HttpResponseMessage response = await client.PutAsync($"users/{id}", httpContent);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(await response.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(await response.Content.ReadAsStringAsync());
        }
    }
}