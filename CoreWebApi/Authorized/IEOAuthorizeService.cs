namespace CoreWebApi.Authorized
{
    public interface IEOAuthorizeService
    {
        bool IsApiAuthorized(string apiKey);
    }
}