using System.Net.Http;

namespace desktop_app.Services.Generation;

public class EnemyGenerationAdminService
{
    private readonly HttpClient _httpClient;

    public EnemyGenerationAdminService(string token)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = ApiSettings.BaseUrl
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
