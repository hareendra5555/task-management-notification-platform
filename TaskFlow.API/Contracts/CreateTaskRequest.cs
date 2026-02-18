using System;
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Contracts;

public class CreateTaskRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }
    public bool IsHighUrgency { get; set; }
}
