using System;
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Models;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
