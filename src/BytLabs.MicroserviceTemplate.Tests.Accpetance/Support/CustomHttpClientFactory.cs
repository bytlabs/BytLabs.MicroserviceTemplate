namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;

public class CustomHttpClientFactory : IHttpClientFactory
{
    private Func<HttpClient> _baseHttpClientFn;

    public CustomHttpClientFactory(Func<HttpClient> httpClientFn) => _baseHttpClientFn = httpClientFn;

    public HttpClient CreateClient(string name) => _baseHttpClientFn.Invoke();
}

