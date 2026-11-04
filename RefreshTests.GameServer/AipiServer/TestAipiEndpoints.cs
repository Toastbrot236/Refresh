using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Responses;
using Bunkum.Listener.Protocol;
using Bunkum.Protocols.Http;
using Refresh.Common;
using Refresh.Core.Services;

namespace RefreshTests.GameServer.AipiServer;

public class TestAipiEndpoints : EndpointGroup
{
    private const string IGNORED_IMAGE_HASH = "165c9093e7ca3b4b27ea6c6b41e5b7e432af4e4e";
    public const string IGNORED_IMAGE = "TEX test hacks lol";

    [HttpEndpoint("/"), Authentication(false)]
    public string TestAipi(RequestContext context)
    {
        return "AIPI scanning service";
    }
    
    [HttpEndpoint("/eva/predict", HttpMethods.Post, ContentType.BinaryData), Authentication(false)]
    public Response ScanImage(RequestContext context, Stream body)
    {
        // check if we're secretly requesting a failure
        string? forcedFailureReason = context.RequestHeaders.Get("X-ForcedFailureReason");
        if (forcedFailureReason != null)
        {
            return new(new AipiResponse<Dictionary<string, float>>
            {
                Success = false,
                Reason = forcedFailureReason,
                Data = null,
            }, ContentType.Json, NotAcceptable);
        }

        // Explicitly ignore if it's this image specifically, so we can test different results for different
        // images per config if wanted. Might be hacky this way, but should be enough for simple testing.
        string receivedHash = BitConverter.ToString(SHA1.HashData(body)).Replace("-", "").ToLower();
        if (receivedHash == IGNORED_IMAGE_HASH)
        {
            return new(new AipiResponse<Dictionary<string, float>>
            {
                Success = true,
                Reason = null,
                Data = [], // Nothing, no tags found
            }, ContentType.Json);
        }
        
        string? thresholdStr = context.QueryString.Get("threshold");
        if (thresholdStr == null || !float.TryParse(thresholdStr, out float threshold))
        {
            context.Logger.LogTrace(RefreshContext.Aipi, $"Threshold not provided or unparseable, falling back to 0.0");
            threshold = 0.0f;
        }
        
        Dictionary<string, float> tags = [];
        if (threshold <= 67) tags.Add("sixSeven", 67.0f);
        if (threshold <= 123.456) tags.Add("hi", 123.456f);
        
        return new(new AipiResponse<Dictionary<string, float>>
        {
            Success = true,
            Reason = null,
            Data = tags,
        }, ContentType.Json);
    }
}