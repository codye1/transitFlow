using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace TransitFlow.mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Auth.cshtml");
        }

        [HttpPost("/auth/login")]
        public async Task<IActionResult> ProxyLogin([FromBody] object model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7094/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

                // Зберігаємо accessToken для MVC додатку
                Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                // ПРОСТО ПРОКИДАЄМО REFRESH COOKIE ВІД API КОРИСТУВАЧУ
                if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
                {
                    foreach (var cookie in cookieHeaders)
                    {
                        Response.Headers.Append("Set-Cookie", cookie);
                    }
                }

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("/auth/register")]
        public async Task<IActionResult> ProxyRegister([FromBody] object model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7094/auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}