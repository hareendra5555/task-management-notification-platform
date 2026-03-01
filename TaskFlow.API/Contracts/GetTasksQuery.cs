using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TaskFlow.API.Contracts;

public class GetTasksQuery : IValidatableObject
{
    public bool? IsCompleted { get; set; }
    public bool? IsHighUrgency { get; set; }
    public string? Search { get; set; }
    public DateTime? DueFrom { get; set; }
    public DateTime? DueTo { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int? MinPriorityScore { get; set; }
    public int? MaxPriorityScore { get; set; }
    public string SortBy { get; set; } = "priority";
    public string SortDirection { get; set; } = "desc";

    [Range(1, 10000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueFrom.HasValue && DueTo.HasValue && DueFrom.Value > DueTo.Value)
        {
            yield return new ValidationResult(
                "DueFrom must be earlier than or equal to DueTo.",
                [nameof(DueFrom), nameof(DueTo)]);
        }

        if (CreatedFrom.HasValue && CreatedTo.HasValue && CreatedFrom.Value > CreatedTo.Value)
        {
            yield return new ValidationResult(
                "CreatedFrom must be earlier than or equal to CreatedTo.",
                [nameof(CreatedFrom), nameof(CreatedTo)]);
        }

        if (MinPriorityScore.HasValue && MaxPriorityScore.HasValue && MinPriorityScore.Value > MaxPriorityScore.Value)
        {
            yield return new ValidationResult(
                "MinPriorityScore must be less than or equal to MaxPriorityScore.",
                [nameof(MinPriorityScore), nameof(MaxPriorityScore)]);
        }

        if (!string.IsNullOrWhiteSpace(SortBy))
        {
            var normalizedSortBy = SortBy.Trim().ToLowerInvariant();
            var allowedSortFields = new HashSet<string> { "priority", "duedate", "created", "title" };

            if (!allowedSortFields.Contains(normalizedSortBy))
            {
                yield return new ValidationResult(
                    "SortBy must be one of: priority, dueDate, created, title.",
                    [nameof(SortBy)]);
            }
        }

        if (!string.IsNullOrWhiteSpace(SortDirection))
        {
            var normalizedSortDirection = SortDirection.Trim().ToLowerInvariant();
            if (normalizedSortDirection is not ("asc" or "desc"))
            {
                yield return new ValidationResult(
                    "SortDirection must be either asc or desc.",
                    [nameof(SortDirection)]);
            }
        }
    }
}
