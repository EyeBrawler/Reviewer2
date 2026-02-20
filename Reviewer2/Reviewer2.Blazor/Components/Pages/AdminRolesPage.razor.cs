using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Reviewer2.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reviewer2.Blazor.Components.Pages.AdminRoles
{
    public partial class AdminRolesPage : ComponentBase
    {
        [Inject]
        protected UserManager<ApplicationUser> UserManager { get; set; } = default!;

        protected List<ApplicationUser> users { get; set; } = new();
        protected Dictionary<Guid, IList<string>> roles { get; set; } = new();
        protected Dictionary<Guid, string?> selectedRoles { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var allUsers = UserManager.Users.ToList();
            users = allUsers ?? new List<ApplicationUser>();

            foreach (var user in users)
            {
                var userRoles = await UserManager.GetRolesAsync(user) ?? new List<string>();
                roles[user.Id] = userRoles;
                selectedRoles[user.Id] = userRoles.FirstOrDefault();
            }
        }

        protected async Task ChangeRoleSafe(ApplicationUser user, string? newRole)
        {
            if (!string.IsNullOrWhiteSpace(newRole))
            {
                await ChangeRole(user, newRole);
            }
        }

        private async Task ChangeRole(ApplicationUser user, string newRole)
        {
            var currentRoles = await UserManager.GetRolesAsync(user) ?? new List<string>();
            if (currentRoles.Count > 0)
            {
                await UserManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await UserManager.AddToRoleAsync(user, newRole);

            // Update local state
            var updatedRoles = await UserManager.GetRolesAsync(user) ?? new List<string>();
            roles[user.Id] = updatedRoles;
            selectedRoles[user.Id] = newRole;

            StateHasChanged();
        }
    }
}