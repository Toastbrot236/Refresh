using JetBrains.Annotations;
using Newtonsoft.Json;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes;

namespace RefreshTests.GameServer.Extensions;

public static class HttpClientExtensions
{
    private static ApiResponse<TData>? ReadData<TData>(HttpResponseMessage response) where TData : class, IApiResponse
    {
        ApiResponse<TData>? body = JsonConvert.DeserializeObject<ApiResponse<TData>>(response.Content.ReadAsStringAsync().Result);
        if (body != null) body.StatusCode = response.StatusCode;
        return body;
    }
    
    private static ApiListResponse<TData>? ReadList<TData>(HttpResponseMessage response) where TData : class, IApiResponse
    {
        ApiListResponse<TData>? body = JsonConvert.DeserializeObject<ApiListResponse<TData>>(response.Content.ReadAsStringAsync().Result);
        if (body != null) body.StatusCode = response.StatusCode;
        return body;
    }
    
    [Pure]
    public static ApiResponse<TData>? GetData<TData>(this HttpClient client, string endpoint, bool ensureSuccessful = true, bool ensureFailure = false) where TData : class, IApiResponse
    {
        HttpResponseMessage response = client.GetAsync(endpoint).Result;
        if (ensureSuccessful)
            Assert.That(response.StatusCode, Is.EqualTo(OK));
        else if (ensureFailure)
            Assert.That(response.StatusCode, Is.Not.EqualTo(OK));
        return ReadData<TData>(response);
    }
    
    [Pure]
    public static ApiListResponse<TData>? GetList<TData>(this HttpClient client, string endpoint, bool ensureSuccessful = true, bool ensureFailure = false) where TData : class, IApiResponse
    {
        HttpResponseMessage response = client.GetAsync(endpoint).Result;
        if (ensureSuccessful)
            Assert.That(response.StatusCode, Is.EqualTo(OK));
        else if (ensureFailure)
            Assert.That(response.StatusCode, Is.Not.EqualTo(OK));
        return ReadList<TData>(response);
    }
    
    public static ApiResponse<TData>? PostData<TData>(this HttpClient client, string endpoint, object data, bool ensureSuccessful = true, bool ensureFailure = false) where TData : class, IApiResponse
    {
        HttpResponseMessage response = client.PostAsync(endpoint, new StringContent(data.AsJson())).Result;
        if (ensureSuccessful)
            Assert.That(response.StatusCode, Is.EqualTo(OK));
        else if (ensureFailure)
            Assert.That(response.StatusCode, Is.Not.EqualTo(OK));
        return ReadData<TData>(response);
    }
    
    public static ApiResponse<TData>? PatchData<TData>(this HttpClient client, string endpoint, object data, bool ensureSuccessful = true, bool ensureFailure = false) where TData : class, IApiResponse
    {
        HttpResponseMessage response = client.PatchAsync(endpoint, new StringContent(data.AsJson())).Result;
        if (ensureSuccessful)
            Assert.That(response.StatusCode, Is.EqualTo(OK));
        else if (ensureFailure)
            Assert.That(response.StatusCode, Is.Not.EqualTo(OK));
        return ReadData<TData>(response);
    }

    public static ApiResponse<TData>? DeleteData<TData>(this HttpClient client, string endpoint, object data, bool ensureSuccessful = true, bool ensureFailure = false) where TData : class, IApiResponse
    {
        // HttpClient's DeleteAsync method does not allow us to include a body
        HttpRequestMessage request = new(HttpMethod.Delete, endpoint)
        {
            Content = new StringContent(data.AsJson())
        };
        HttpResponseMessage response = client.SendAsync(request).Result;

        if (ensureSuccessful)
            Assert.That(response.StatusCode, Is.EqualTo(OK));
        else if (ensureFailure)
            Assert.That(response.StatusCode, Is.Not.EqualTo(OK));
        return ReadData<TData>(response);
    }
}