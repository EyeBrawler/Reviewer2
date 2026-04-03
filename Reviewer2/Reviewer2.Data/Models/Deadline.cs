using System;

namespace Reviewer2.Data.Models
{
    /// <summary>
    /// Represents an important date or milestone associated with a conference,
    /// such as submission deadlines, review periods, or notification dates.
    /// </summary>
    public class Deadline
    {
        /// <summary>
        /// Gets or sets the unique identifier for the deadline.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the conference to which this deadline belongs.
        /// Serves as the foreign key in the database relationship.
        /// </summary>
        public int ConferenceId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated conference.
        /// </summary>
        public Conference? Conference { get; set; }

        /// <summary>
        /// Gets or sets the name of the deadline (e.g., "Submission Deadline",
        /// "Review Deadline", "Notification Date").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which this deadline occurs.
        /// Uses <see cref="DateTimeOffset"/> to preserve time zone information.
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Gets or sets the priority of the deadline.
        /// Higher values indicate more important or prominent deadlines.
        /// </summary>
        public int Priority { get; set; }
    }
}