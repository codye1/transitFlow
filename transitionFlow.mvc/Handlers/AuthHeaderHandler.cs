using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        string? accessToken = context?.Request.Cookies["accessToken"];

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        var requestUrl = request.RequestUri?.AbsolutePath ?? "";
        if (response.StatusCode == HttpStatusCode.Unauthorized
            && !requestUrl.Contains("/auth/login")
            && !requestUrl.Contains("/auth/refresh"))
        {
            var (isRefreshed, newAccessToken) = await TryRefreshTokenAsync(context);

            if (isRefreshed && newAccessToken != null)
            {
                var newRequest = CloneRequest(request);
                newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                return await base.SendAsync(newRequest, cancellationToken);
            }
            else
            {
                context?.Response.Cookies.Delete("accessToken");
                context?.Response.Redirect("/login");
            }
        }

        return response;
    }

    private async Task<(bool Success, string? AccessToken)> TryRefreshTokenAsync(HttpContext? context)
    {
        if (context == null) return (false, null);

        try
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return (false, null);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7094");

            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/auth/refresh");
            // Передаємо куку (браузер надіслав її вже розкодованою, тому тут все буде ок)
            refreshRequest.Headers.Add("Cookie", $"refreshToken={refreshToken}");

            var response = await client.SendAsync(refreshRequest);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
                if (data != null && !string.IsNullOrEmpty(data.AccessToken))
                {
                    context.Response.Cookies.Append("accessToken", data.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
                    {
                        foreach (var cookie in cookieHeaders)
                        {
                            context.Response.Headers.Append("Set-Cookie", cookie);
                        }
                    }

                    return (true, data.AccessToken);
                }
            }
        }
        catch
        {
            return (false, null);
        }

        return (false, null);
    }

    private HttpRequestMessage CloneRequest(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri)
        {
            Content = req.Content
        };
        foreach (var header in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        foreach (var prop in req.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(prop.Key.ToString()), prop.Value);
        }
        return clone;
    }
}

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
}