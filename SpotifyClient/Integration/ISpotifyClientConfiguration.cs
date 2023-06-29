namespace Tolltech.SpotifyClient.Integration
{
    public interface ISpotifyClientConfiguration
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
}