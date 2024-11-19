using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace CustomSerilogLib;

public class CustomJsonSink : ILogEventSink
{
    private readonly JsonFormatter _formatter = new(renderMessage: true);

    public void Emit(LogEvent logEvent)
    {
        using var writer = new StringWriter();
        _formatter.Format(logEvent, writer);
        var formattedLog = writer.ToString();

        var modifiedJson = ModifyJson(formattedLog, logEvent.MessageTemplate.Render(logEvent.Properties));
        Console.Write(modifiedJson);
    }

    private string ModifyJson(string json, string newMessageValue)
    {
        var jsonObject = JObject.Parse(json);
        ReplaceRenderedMessage(jsonObject, newMessageValue);
        ConvertToCamelCase(jsonObject);
        return jsonObject + Environment.NewLine;
    }

    private void ReplaceRenderedMessage(JObject jsonObject, string newMessageValue)
    {
        if (jsonObject.ContainsKey("RenderedMessage"))
        {
            var messageProperty = jsonObject["RenderedMessage"];
            jsonObject.Remove("RenderedMessage");
            jsonObject.Add("message", newMessageValue);
        }
    }

    private void ConvertToCamelCase(JObject jsonObject)
    {
        foreach (var property in jsonObject.Properties().ToList())
        {
            var camelCasePropertyName = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];

            switch (property.Value)
            {
                case JObject childObject:
                    ConvertToCamelCase(childObject);
                    break;
                case JArray childArray:
                {
                    foreach (var item in childArray.OfType<JObject>())
                    {
                        ConvertToCamelCase(item);
                    }
                    break;
                }
            }

            if (camelCasePropertyName != property.Name)
            {
                property.Replace(new JProperty(camelCasePropertyName, property.Value));
            }
        }
    }
}