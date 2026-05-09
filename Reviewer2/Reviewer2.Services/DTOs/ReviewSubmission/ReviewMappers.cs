using System;
using System.Collections.Generic;
using System.Text.Json;
using Reviewer2.Data.Models;
using Serilog;

namespace Reviewer2.Services.DTOs.ReviewSubmission;

/// <summary>
/// Extension methods for mapping between Review entities and Review DTOs.
/// </summary>
public static class ReviewMappers
{
    private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    
    
    /// <summary>
    /// Maps a <see cref="Review"/> entity to a <see cref="ReviewDTO"/>.
    /// </summary>
    /// <param name="review">The review entity to map.</param>
    /// <returns>A <see cref="ReviewDTO"/> representing the review.</returns>
    public static ReviewDTO ToDTO(this Review review)
    {
        ArgumentNullException.ThrowIfNull(review);

        return new ReviewDTO
        {
            Id = review.Id,
            ReviewAssignmentId = review.ReviewAssignmentId,
            ReviewTemplateId = review.ReviewTemplateId,
            SubmittedAtUtc = review.SubmittedAtUtc,
            OverallScore = review.OverallScore,
            ConfidenceScore = review.ConfidenceScore,
            Recommendation = review.Recommendation,
            Values = DeserializeValues(review)
        };
    }

    /// <summary>
    /// Maps a <see cref="SubmitReviewDTO"/> to a <see cref="Review"/> entity.
    /// Useful when creating or updating a review from client input.
    /// </summary>
    /// <param name="dto">The submit review DTO to map.</param>
    /// <param name="existingReview">
    /// Optional existing review entity for update operations.
    /// </param>
    /// <returns>
    /// A <see cref="Review"/> entity populated with data from the DTO.
    /// </returns>
    public static Review ToEntity(
        this SubmitReviewDTO dto,
        Review? existingReview = null)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var review = existingReview ?? new Review
        {
            Id = Guid.NewGuid(),
            ReviewAssignmentId = dto.ReviewAssignmentId
        };

        review.OverallScore = dto.OverallScore;
        review.ConfidenceScore = dto.ConfidenceScore;
        review.Recommendation = dto.Recommendation;
        review.JsonContent = SerializeValues(dto.Values);

        return review;
    }
    
    /// <summary>
    /// Deserializes the review's stored JSON content into structured field
    /// values, returning an empty collection if the content is missing or invalid.
    /// </summary>
    private static Dictionary<string, ReviewValue> DeserializeValues(
        Review review)
    {
        if (string.IsNullOrWhiteSpace(review.JsonContent))
            return new Dictionary<string, ReviewValue>();

        try
        {
            return JsonSerializer.Deserialize<
                       Dictionary<string, ReviewValue>>(
                       review.JsonContent,
                       JsonOptions)
                   ?? new Dictionary<string, ReviewValue>();
        }
        catch (JsonException ex)
        {
            Log.Warning(
                ex,
                "Failed to deserialize review JSON for Review {ReviewId}, " +
                "Assignment {AssignmentId}, Template {TemplateId}.",
                review.Id,
                review.ReviewAssignmentId,
                review.ReviewTemplateId);

            return new Dictionary<string, ReviewValue>();
        }
    }

    /// <summary>
    /// Serializes structured review values into JSON for persistence.
    /// </summary>
    /// <param name="values">
    /// The review values to serialize.
    /// </param>
    /// <returns>
    /// A JSON representation of the review values.
    /// </returns>
    private static string SerializeValues(
        Dictionary<string, ReviewValue>? values)
    {
        return JsonSerializer.Serialize(
            values ?? new Dictionary<string, ReviewValue>(),
            JsonOptions);
    }
}