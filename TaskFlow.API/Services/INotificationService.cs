using System.Collections.Generic;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public interface INotificationService
{
    void Publish(NotificationEvent notificationEvent);
    IReadOnlyCollection<NotificationEvent> GetRecent(int count);
}
