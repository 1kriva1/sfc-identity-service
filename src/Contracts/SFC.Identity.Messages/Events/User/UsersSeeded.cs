namespace SFC.Identity.Messages.Events.User;
public class UsersSeeded
{
    public IEnumerable<UserEntity> Users { get; init; } = [];
}