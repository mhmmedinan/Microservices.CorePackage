using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Core.Abstractions.Messaging.Serialization.Newtonsoft;

public class NewtonsoftJsonMessageSerializer : IMessageSerializer
{
    public NewtonsoftJsonMessageSerializer()
    {
        
    }
    public T? Deserialize<T>(string payload, bool camelCase = true)
    {
        return JsonConvert.DeserializeObject<T>(payload, CreateSerializerSettings(camelCase));
    }

    public object? Deserialize(string payload, Type type, bool camelCase = true)
    {
        return JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings(camelCase));
    }

    public string Serialize(object obj, bool camelCase = true, bool indented = true)
    {
        return JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));
    }

    protected virtual JsonSerializerSettings? CreateSerializerSettings(bool camelCase = true, bool indented = false)
    {
        var settings = new JsonSerializerSettings();

        settings.ContractResolver = new ContractResolverWithPrivate();

        if (indented)
        {
            settings.Formatting = Formatting.Indented;
        }

        // for handling private constructor
        settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        return settings;
    }


    private class ContractResolverWithPrivate : CamelCasePropertyNamesContractResolver
    {
        // http://danielwertheim.se/json-net-private-setters/
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
