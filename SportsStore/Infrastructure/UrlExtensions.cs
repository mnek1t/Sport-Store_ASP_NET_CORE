namespace SportsStore.Infrastructure
{
    public static class UrlExtensions
    {
        public static string PathAndQuery(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.QueryString.HasValue ? $"{request.Path}{request.QueryString}" : request.Path.ToString();
        }
    }
}
