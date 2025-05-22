namespace SFC.Identity.Infrastructure.Settings;
public class GrpcSettings
{
    public const string SectionKey = "Grpc";

    public int MaxReceiveMessageSizeInMb { get; set; }

    public int MaxSendMessageSizeInMb { get; set; }
}