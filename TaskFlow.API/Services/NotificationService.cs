using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly LinkedList<NotificationEvent> _events = new();
    private readonly object _gate = new();

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public void Publish(NotificationEvent notificationEvent)
    {
        lock (_gate)
        {
            _events.AddFirst(notificationEvent);

            while (_events.Count > 200)
            {
                _events.RemoveLast();
            }
        }

        _logger.LogInformation(
            "Notification event published: {EventType} for task {TaskId}",
            notificationEvent.EventType,
            notificationEvent.TaskId);
    }

    public IReadOnlyCollection<NotificationEvent> GetRecent(int count)
    {
        lock (_gate)
        {
            return _events.Take(count).ToList();
        }
    }
}
