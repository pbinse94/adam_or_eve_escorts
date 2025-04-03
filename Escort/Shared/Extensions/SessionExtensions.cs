using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace Shared.Extensions
{
    public static class SessionExtensions
    {
        public static T? GetComplexData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static void SetComplexData(this ISession session, string key, object? value)
        {
            session.SetString(key, value == null ? null : JsonConvert.SerializeObject(value));
        }
    }


}
