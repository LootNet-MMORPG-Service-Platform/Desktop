using System;

namespace desktop_app.Services;

public static class ApiSettings
{
#if DEBUG
    public static readonly Uri BaseUrl = new("https://localhost:7124");
#else
    public static readonly Uri BaseUrl = new("https://lootnet-api.onrender.com");
#endif
}