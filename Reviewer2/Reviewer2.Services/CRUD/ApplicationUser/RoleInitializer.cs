using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Reviewer2.Services.CRUD.ApplicationUser;

/// <summary>
/// Creates the associated roles within the Database via Microsoft Identity
/// </summary>
public class RoleInitializer
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleInitializer"/> class.
    /// </summary>
    /// <param name="roleManager">
    /// The <see cref="RoleManager{T}"/> responsible for managing 
    /// <see cref="IdentityRole{Guid}"/> entities in the identity store.
    /// This service is used to create and verify the existence of 
    /// application roles during system initialization.
    /// </param>
    public RoleInitializer(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Defines the system-level roles used by the conference management application.
    ///
    /// Roles represent elevated permissions within the system. Regular authenticated
    /// users do NOT require a role to submit papers or register for the conference.
    /// Roles should only be assigned to users who perform administrative or review duties.
    ///
    /// Role descriptions:
    ///
    /// Admin
    ///     System-level administrator.
    ///     Has full access to all features, including user management and role assignment.
    ///     Intended for platform maintenance and oversight.
    ///
    /// ConferenceChair
    ///     Highest authority for the conference.
    ///     Can make final acceptance/rejection decisions, manage deadlines,
    ///     configure review templates, and oversee scheduling.
    ///
    /// PaperChair
    ///     Assists in managing the review process.
    ///     Can assign reviewers, monitor review progress, and view submitted reviews.
    ///     Can accept and reject papers?
    ///
    /// Reviewer
    ///     Member of the program committee.
    ///     Can access assigned papers and submit reviews.
    ///     Must also have an active ReviewAssignment to review a specific paper.
    ///
    /// Notes:
    /// - Authenticated users without a role can still submit papers and act as authors.
    /// - Authorship and attendance are modeled through domain entities (Author, Registration),
    ///   not through Identity roles.
    /// - Roles represent system permissions, not workflow states.
    /// </summary>
    /// <remarks>
    /// This method iterates through a predefined list of roles and checks if each role exists in the database.
    /// If a role does not exist, it creates the role using the RoleManager.
    /// </remarks>
    public async Task InitializeAsync()
    {
        string[] roles =
        [
            "Admin",
            "ConferenceChair",
            "PaperChair",
            "Reviewer"
        ];

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = role
                    });
            }
        }
    }
}