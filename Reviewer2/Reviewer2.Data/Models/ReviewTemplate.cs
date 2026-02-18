using System;

namespace Reviewer2.Data.Models;

public class ReviewTemplate
{
    public Guid Id { get; set; }

    public int Version { get; set; }

    public string Name { get; set; } = string.Empty;

    public string JsonSchema { get; set; } = "{}";

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
