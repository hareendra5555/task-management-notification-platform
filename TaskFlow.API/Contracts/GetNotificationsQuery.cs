using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Contracts;

public class GetNotificationsQuery
{
    [Range(1, 100)]
    public int Count { get; set; } = 20;
}
