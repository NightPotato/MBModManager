using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MBModManager.Handlers;

public class WebHandler
{
    public static async Task<T?> GetJson<T>(string url)
    {
        using HttpClient httpClient = new HttpClient();
        string json = await httpClient.GetStringAsync(url);
        T? obj = JsonConvert.DeserializeObject<T>(json);

        return obj ?? default;
    }
}
