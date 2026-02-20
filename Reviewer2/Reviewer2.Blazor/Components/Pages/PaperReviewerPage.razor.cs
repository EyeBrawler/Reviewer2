using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace Reviewer2.Blazor.Components.Pages
{
    public partial class PaperReviewerPage : ComponentBase
    {
        protected List<PaperViewModel> AssignedPapers = new();

        protected override void OnInitialized()
        {
            // Demo data - replace with database/service later
            AssignedPapers = new List<PaperViewModel>
            {
                new PaperViewModel
                {
                    Id = 1,
                    Title = "AI Assisted Software Engineering",
                    Abstract = "This paper explores the role of AI in a modern developmental workflow.",
                    Authors = new List<Author>
                    {
                        new Author { Name = "Jane Doe" },
                        new Author { Name = "John Smith" }
                    },
                    Review = new ReviewModel()
                }
            };
        }

        protected void SubmitReview(PaperViewModel paper)
        {
            Console.WriteLine($"Review submitted for Paper {paper.Id}");

            // TODO:
            // await ReviewService.SaveAsync(paper.Review);
        }

        // Models
        public class PaperViewModel
        {
            public int Id { get; set; }

            public string Title { get; set; } = String.Empty;

            public string Abstract { get; set; } = String.Empty;

            public List<Author> Authors { get; set; } = new();

            public ReviewModel Review { get; set; } = new();
        }

        public class Author
        {
            public string Name { get; set; } = String.Empty;
        }

        public class ReviewModel
        {
            [Required] public int? Score { get; set; }

            [Required] public int? Confidence { get; set; }

            public string? AuthorComments { get; set; }
        }
    }
}