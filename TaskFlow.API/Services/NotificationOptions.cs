using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Services;

public class NotificationOptions
{
    public const string SectionName = "Notifications";

    [Range(50, 5000)]
    public int MaxRetainedEvents { get; set; } = 200;
}
