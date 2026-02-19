using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Reviewer2.Blazor.Components.Pages
{
    public partial class SubmitPaper : ComponentBase
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        protected SubmissionModel Submission { get; set; } = new()
        {
            Authors = new List<AuthorModel> { new AuthorModel() }
        };

        protected IBrowserFile? UploadedFile { get; set; }
        protected string? FileError { get; set; }

        protected bool CanSubmit =>
            UploadedFile != null &&
            editContext?.Validate() == true;

        protected void AddAuthor()
        {
            Submission.Authors.Add(new AuthorModel());
            editContext?.NotifyFieldChanged(
                new FieldIdentifier(Submission, nameof(Submission.Authors)));
            StateHasChanged();
        }

        protected void RemoveAuthor(AuthorModel author)
        {
            if (Submission.Authors.Count > 1)
            {
                Submission.Authors.Remove(author);
                editContext?.NotifyFieldChanged(
                    new FieldIdentifier(Submission, nameof(Submission.Authors)));

                StateHasChanged();
            }
        }

        protected async Task HandleFileUpload(InputFileChangeEventArgs e)
        {
            FileError = null;
            UploadedFile = null;

            var file = e.File;

            if (file == null)
                return;

            if (file.ContentType != "application/pdf")
            {
                FileError = "Only PDF files are allowed.";
                return;
            }

            if (!file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                FileError = "File must have a .pdf extension.";
                return;
            }

            if (file.Size > MaxFileSize)
            {
                FileError = "PDF must be smaller than 10MB.";
                return;
            }

            UploadedFile = file;

            using var stream = file.OpenReadStream(MaxFileSize);
            await Task.CompletedTask;
            
            editContext?.NotifyValidationStateChanged();
            StateHasChanged();
        }

        protected async Task HandleSubmit()
        {
            if (UploadedFile == null)
            {
                FileError = "A PDF must be uploaded before submission.";
                StateHasChanged();
                return;
            }

            // TODO:
            // Save submission to database
            // Save file to disk or cloud storage
            // Send confirmation email

            await Task.CompletedTask;
        }

        protected void HandleInvalid()
        {
            Console.WriteLine("FORM INVALID");
        }

        // =============================
        // Internal Models (self-contained)
        // =============================

        public class SubmissionModel
        {
            [Required]
            public string Title { get; set; } = string.Empty;

            [Required]
            public string Abstract { get; set; } = string.Empty;

            [Required]
            public string Track { get; set; } = string.Empty;

            public List<AuthorModel> Authors { get; set; } = new();
        }

        public class AuthorModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            
            public string Email { get; set; } = string.Empty;
        }
        
        private EditContext? editContext;

        protected override void OnInitialized()
        {
            editContext = new EditContext(Submission);

            editContext.OnFieldChanged += (_, __) =>
            {
                StateHasChanged();
            };
        }
    }
}
