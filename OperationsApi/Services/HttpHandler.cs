namespace OperationsApi.Services;

internal sealed class HttpHandler( IConfiguration configuration, ILogger<HttpHandler> logger )
{
    readonly HttpClient _http = GetHttpClient( configuration );
    readonly ILogger<HttpHandler> _logger = logger;

    internal async Task<T?> TryGet<T>( string url )
    {
        try
        {
            using var httpResponse = await _http.GetAsync( url );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T>( e, url );
        }
    }
    internal async Task<T?> TryPost<T>( string url, object? body )
    {
        try
        {
            using var httpResponse = await _http.PostAsJsonAsync( url, body );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T>( e, url );
        }
    }
    internal async Task<T?> TryPut<T>( string url, object? body )
    {
        try
        {
            using var httpResponse = await _http.PostAsJsonAsync( url, body );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T>( e, url );
        }
    }
    internal async Task<T?> TryDelete<T>( string url )
    {
        try
        {
            using var httpResponse = await _http.DeleteAsync( url );
            return await HandleHttpResponse<T>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T>( e, url );
        }
    }

    async Task<T?> HandleHttpResponse<T>( HttpResponseMessage httpResponse )
    {
        if (httpResponse.IsSuccessStatusCode)
        {
            var content = await httpResponse.Content.ReadFromJsonAsync<T>();
            if (content is null)
                _logger.LogError( $"Http response content is null." );
            return content;
        }

        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        _logger.LogError( $"An exception was thrown during an http request : {errorContent}" );
        return default;
    }
    T? HandleHttpException<T>( Exception e, string requestUrl )
    {
        _logger.LogError( e, $"{requestUrl} ----- {e.Message}" );
        return default;
    }
    
    static HttpClient GetHttpClient( IConfiguration config ) =>
        new( new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes( 3 ) } ) {
            BaseAddress = new Uri( config["BaseUrl"] ?? throw new Exception( "Failed to get BaseUrl from IConfiguration in HttpService!" ) )
        };
}