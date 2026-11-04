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
    // hash of RefreshTests.GameServer.Resources.1x1.png
    private const string IGNORE_HASH = "0ec63b140374ba704a58fa0c743cb357683313dd";

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

        // Explicitly ignore the 1x1 PNG so we don't just return the same for all images.
        // Probably hacky, but should be enough for unit tests.
        string receivedHash = BitConverter.ToString(SHA1.HashData(body)).Replace("-", "").ToLower();
        if (receivedHash == IGNORE_HASH)
        {
            return new(new AipiResponse<Dictionary<string, float>>
            {
                Success = true,
                Reason = null,
                Data = [], // Nothing, the 1x1 is good to go
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