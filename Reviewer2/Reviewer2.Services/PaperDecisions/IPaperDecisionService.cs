using System;
using System.Threading.Tasks;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.PaperDecisions;


/// <summary>
/// Defines operations for making final decisions on <see cref="Paper"/> submissions.
/// 
/// This service acts as an application-layer coordinator for decision workflows,
/// including acceptance, rejection, and revision requests. It is responsible for
/// loading the appropriate paper, invoking domain logic, and persisting changes.
/// </summary>
public interface IPaperDecisionService
{
    /// <summary>
    /// Records an acceptance decision for the specified paper.
    /// 
    /// This operation transitions the paper to an accepted state if it is eligible
    /// for a final decision (e.g., after review completion or direct submission,
    /// depending on workflow rules).
    /// </summary>
    /// <param name="paperId">
    /// The unique identifier of the paper to accept.
    /// </param>
    /// <param name="chairUserId">
    /// The unique identifier of the conference chair (or authorized decision-maker)
    /// performing the acceptance.
    /// </param>
    /// <param name="comment">
    /// Optional remarks or instructions associated with the acceptance decision.
    /// These may be visible to the authors.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper does not exist or is not in a valid state for acceptance.
    /// </exception>
    Task AcceptAsync(Guid paperId, Guid chairUserId, string? comment);
    
    /// <summary>
    /// Records a rejection decision for the specified paper.
    /// 
    /// This operation transitions the paper to a rejected state if it is eligible
    /// for a final decision (e.g., after review completion or direct submission,
    /// depending on workflow rules).
    /// </summary>
    /// <param name="paperId">
    /// The unique identifier of the paper to reject.
    /// </param>
    /// <param name="chairUserId">
    /// The unique identifier of the conference chair (or authorized decision-maker)
    /// performing the rejection.
    /// </param>
    /// <param name="comment">
    /// Optional remarks explaining the rejection decision.
    /// These may be visible to the authors.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper does not exist or is not in a valid state for rejection.
    /// </exception>
    Task RejectAsync(Guid paperId, Guid chairUserId, string? comment);
    
    /// <summary>
    /// Requests revisions for the specified paper.
    /// 
    /// This operation transitions the paper into a revision-required state,
    /// indicating that the authors must address reviewer or chair feedback
    /// before a final decision can be made.
    /// </summary>
    /// <param name="paperId">
    /// The unique identifier of the paper requiring revisions.
    /// </param>
    /// <param name="chairUserId">
    /// The unique identifier of the conference chair (or authorized decision-maker)
    /// requesting revisions.
    /// </param>
    /// <param name="comment">
    /// Optional feedback describing the required revisions.
    /// This is typically visible to the authors.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper does not exist or is not in a valid state for requesting revisions.
    /// </exception>
    Task RequestRevisionsAsync(Guid paperId, Guid chairUserId, string? comment);
}