using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly LinkedList<NotificationEvent> _events = new();
    private readonly object _gate = new();
    private readonly int _maxRetainedEvents;

    public NotificationService(ILogger<NotificationService> logger, IOptions<NotificationOptions> notificationOptions)
    {
        _logger = logger;
        _maxRetainedEvents = notificationOptions.Value.MaxRetainedEvents;
    }

    public void Publish(NotificationEvent notificationEvent)
    {
        lock (_gate)
        {
            _events.AddFirst(notificationEvent);

            while (_events.Count > _maxRetainedEvents)
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
