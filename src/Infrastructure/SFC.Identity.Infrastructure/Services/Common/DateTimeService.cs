using SFC.Identity.Application.Interfaces.Common;

namespace SFC.Identity.Infrastructure.Services.Common;
public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime DateNow => DateTime.UtcNow.Date;
}