using System.Text.Encodings.Web;
using System.Text.Json;

using Grpc.Core;

using SFC.Identity.Api.Infrastructure.Models.Pagination;
using SFC.Identity.Contracts.Headers;
using SFC.Identity.Infrastructure.Constants;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class HeadersExtensions
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static void AddPaginationHeader(this HttpResponse response, PageMetadataModel metadata)
    {
        response?.Headers.Append(CommonConstants.PaginationHeaderKey, JsonSerializer.Serialize(metadata, WriteOptions));
    }

    public static void AddPaginationHeader(this ServerCallContext context, PaginationHeader metadata)
    {
        Metadata responseHeaders = new()
        {
            { CommonConstants.PaginationHeaderKey, JsonSerializer.Serialize(metadata, WriteOptions) }
        };
        context.WriteResponseHeadersAsync(responseHeaders);
    }

    public static void AddAuditableHeaderIfRequested(this ServerCallContext context, AuditableHeader header)
    {
        Metadata.Entry? auditableHeader = context.RequestHeaders.FirstOrDefault(h =>
            string.Equals(h.Key, CommonConstants.AuditableHeaderKey, StringComparison.OrdinalIgnoreCase));

        if (bool.TryParse(auditableHeader?.Value, out bool result) && result)
        {
            Metadata responseHeaders = new()
            {
                { CommonConstants.AuditableHeaderKey, JsonSerializer.Serialize(header, WriteOptions) }
            };

            context.WriteResponseHeadersAsync(responseHeaders);
        }
    }
}