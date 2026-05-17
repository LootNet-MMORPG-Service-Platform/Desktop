using System;
using System.Threading.Tasks;
using Meziantou.Framework.Win32;

namespace desktop_app.Services;

public class AuthTokenService
{
    private const string ApplicationName = "LootNet.desktop-app.refresh-token";
    private const string UserName = "refresh-token";

    public Task SaveRefreshTokenAsync(string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            EnsureWindowsCredentialManager();

            #pragma warning disable CA1416
            CredentialManager.WriteCredential(
                applicationName: ApplicationName,
                userName: UserName,
                secret: token,
                comment: "LootNet desktop app refresh token",
                persistence: CredentialPersistence.LocalMachine);
            #pragma warning restore CA1416
        }

        return Task.CompletedTask;
    }

    public Task<string?> GetRefreshTokenAsync()
    {
        EnsureWindowsCredentialManager();

        #pragma warning disable CA1416
        var credential = CredentialManager.ReadCredential(ApplicationName);
        #pragma warning restore CA1416

        return Task.FromResult(credential?.Password);
    }

    public Task ClearRefreshTokenAsync()
    {
        EnsureWindowsCredentialManager();

        #pragma warning disable CA1416
        CredentialManager.DeleteCredential(ApplicationName);
        #pragma warning restore CA1416

        return Task.CompletedTask;
    }

    private static void EnsureWindowsCredentialManager()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            throw new PlatformNotSupportedException("Windows Credential Manager requires Windows XP or newer.");
    }
}
