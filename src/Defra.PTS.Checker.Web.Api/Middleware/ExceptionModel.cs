using Newtonsoft.Json;

namespace Defra.PTS.Checker.Web.Api.Middleware;

public class ExceptionModel
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("traceId")] 
    public string TraceId { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }
}
