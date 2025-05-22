namespace SFC.Identity.Messages.Events.User;
public class UserCreated
{
    public required UserEntity User { get; set; }
}