using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MBModManager.Handlers;

public static class WebHandler
{
    public static async Task<T?> GetJson<T>(string url)
    {
        using var httpClient = new HttpClient();
        var json = await httpClient.GetStringAsync(url).ConfigureAwait(false);
        httpClient.Dispose();

        var obj = JsonConvert.DeserializeObject<T>(json);
        

        return obj ?? default;
    }
}
