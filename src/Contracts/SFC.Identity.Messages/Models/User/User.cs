using SFC.Identity.Messages.Models.Common;

namespace SFC.Identity.Messages.Models.User;
public class User : Auditable
{
    public Guid Id { get; set; }
}