using SFC.Identity.Messages.Commands.Common;

namespace SFC.Identity.Messages.Commands.User;
public class SeedUsers : InitiatorCommand
{
    public IEnumerable<UserEntity> Users { get; init; } = [];
}