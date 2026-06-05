using System.Text.Json;
using System.Text.Json.Nodes;

namespace BytLabs.MicroserviceTemplate.Domain.Utils
{
    // RECIPE: Dynamic data merge. Shallow-merges element2 INTO element1 (element2 keys win),
    // so an Update can carry only the changed keys of the dynamic `Data` payload.
    public static class JsonElementExtensions
    {
        public static JsonElement Merge(this JsonElement element1, JsonElement element2)
        {
            var node1 = JsonObject.Parse(element1.GetRawText()) as JsonObject;
            var node2 = JsonObject.Parse(element2.GetRawText()) as JsonObject;

            if (node1 is null) throw new NotImplementedException();
            if (node2 is null) return element1;

            foreach (var kvp in node2)
            {
                node1[kvp.Key] = kvp.Value?.DeepClone(); // overwrites existing keys
            }

            return JsonDocument.Parse(node1.ToJsonString()).RootElement;
        }
    }
}
