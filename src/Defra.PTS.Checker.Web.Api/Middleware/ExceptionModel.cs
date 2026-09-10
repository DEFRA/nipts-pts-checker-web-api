using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Web.Api.Middleware;

/// <summary>
/// Represents a standardised error response returned by the API.
/// </summary>
[ExcludeFromCodeCoverage]
public class ExceptionModel
{
    /// <summary>
    /// Gets or sets the HTTP status code associated with the error.
    /// </summary>
    [JsonProperty("status")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the error title.
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier for correlating the error.
    /// </summary>
    [JsonProperty("traceId")] 
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the detailed error message.
    /// </summary>
    [JsonProperty("error")]
    public string? Error { get; set; }
}
