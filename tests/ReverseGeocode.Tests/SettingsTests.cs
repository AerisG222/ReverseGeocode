using ReverseGeocode.Commands;

namespace ReverseGeocode.Tests;

// where the two secrets come from.  a deployment passes them through the
// environment, because a command line argument is readable by every user on the
// host; an explicit option still wins, for running the tool by hand.
//
// environment variables are process wide, so every test here restores the two it
// touches - and nothing else in the suite reads them.
public class SettingsTests
{
    [Fact]
    public void SecretsAreReadFromTheEnvironment()
    {
        using var env = new ScopedEnvironment("env-secret", "env-key");

        var settings = Settings();

        Assert.Equal("env-secret", settings.ResolvedClientSecret);
        Assert.Equal("env-key", settings.ResolvedGoogleApiKey);
        Assert.True(settings.Validate().Successful);
    }

    [Fact]
    public void AnExplicitOptionWinsOverTheEnvironment()
    {
        using var env = new ScopedEnvironment("env-secret", "env-key");

        var settings = Settings(clientSecret: "cli-secret", googleApiKey: "cli-key");

        Assert.Equal("cli-secret", settings.ResolvedClientSecret);
        Assert.Equal("cli-key", settings.ResolvedGoogleApiKey);
    }

    [Fact]
    public void MissingSecretsFailValidation()
    {
        using var env = new ScopedEnvironment(null, null);

        Assert.False(Settings().Validate().Successful);
        Assert.False(Settings(clientSecret: "cli-secret").Validate().Successful);
        Assert.True(Settings(clientSecret: "cli-secret", googleApiKey: "cli-key").Validate().Successful);
    }

    static LoadGeocodeDataCommand.Settings Settings(string clientSecret = "", string googleApiKey = "") =>
        new() { ClientSecret = clientSecret, GoogleApiKey = googleApiKey };

    // sets both variables for the life of a test, then puts back what was there
    sealed class ScopedEnvironment : IDisposable
    {
        readonly string? _secret = Environment.GetEnvironmentVariable(LoadGeocodeDataCommand.Settings.CLIENT_SECRET_ENV);
        readonly string? _key = Environment.GetEnvironmentVariable(LoadGeocodeDataCommand.Settings.GOOGLE_API_KEY_ENV);

        public ScopedEnvironment(string? secret, string? key)
        {
            Environment.SetEnvironmentVariable(LoadGeocodeDataCommand.Settings.CLIENT_SECRET_ENV, secret);
            Environment.SetEnvironmentVariable(LoadGeocodeDataCommand.Settings.GOOGLE_API_KEY_ENV, key);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(LoadGeocodeDataCommand.Settings.CLIENT_SECRET_ENV, _secret);
            Environment.SetEnvironmentVariable(LoadGeocodeDataCommand.Settings.GOOGLE_API_KEY_ENV, _key);
        }
    }
}
