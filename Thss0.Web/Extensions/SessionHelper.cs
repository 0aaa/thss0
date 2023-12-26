using Newtonsoft.Json;

namespace Thss0.Web.Extensions
{
    public static class SessionHelper
    {
        public static void Serialize(this ISession session, string key, object value)
            => session.SetString(key, JsonConvert.SerializeObject(value));

        public static GenType Deserialize<GenType>(this ISession session, string key)
            => JsonConvert.DeserializeObject<GenType>(session.GetString(key) ?? string.Empty)!;
    }
}