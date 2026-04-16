## [demo-5] - 2026-04-17
### Added
* Paper chairs have a page from which they can approve papers.
* When submitting a review or looking at submitted reviews for a paper, an embedded PDF view of the paper itself is visible.
* Conference/paper chairs and admins can customize conference review forms using the template editor form.

### Fixed
* Drag and drop logic for reviewers on the reviewer assignment page has been improved with no a full page refresh after
each drop no longer required.
* Reviewers can no longer manipulate raw JSON data for a review via the review form.
* Minor formatting tweaks have been made to the deployment documentation to make it more readable and easy to follow, especially
when viewing the Markdown raw.

### Notes
* This release has the main objective of completing the core reviewing logic in Reviewer2, primarily by adding the ability 
to approve papers. With this feature now present, the application could be used to manage almost all the core 
conference administration workflows.

---

## [demo-4] - 2026-04-03

### Added
* New Database tables to store conference metadata (like the call for papers text and deadlines)
* A new "Reviewer Assignment" page with drag and drop functionality for paper chairs and admins to use for pairing Reviewers to 
submitted papers. Basic auto assignment/matching is available.
* A new button exists on the home page which allows logged-in users to volunteer to be a reviewer.
* A new homepage exists that can display user configurable conference information such as a call for papers, the name of the conference,
and deadlines.
* The mockup review submission page has been made functional.

### Changed
* The primary deployment method for Reviewer2 is now Docker with the Docker compose plugin rather than systemd services. All well known
options for deployment are generally documented in the ``README.md`` located in the ``Deployment`` folder.
* The paper submission form now allows other Reviewer2 users to be linked as Authors on a paper.

### Removed
* The old generic Reviewer2 home page

### Notes
This release marks a large step toward a more complete conference management workflow in Reviewer2. One of the most
signigicant features still missing is conference chair paper approval. The fundamentals of the reviewing workflow are
arguably now present though.

---

## [demo-3] - 2026-03-06

### Added

* Deployment documentation including Nginx configuration and application hosting setup.
* `publish.sh` script to automate deployment workflows.
* Initial user seeding mechanism for administrative access.
* Pagination and sorting functionality for the submissions table.
* Enhanced submission details page with additional metadata, including author information and identifiers.
* Global styling improvements for consistent theming across buttons, tables, badges, and navigation elements.

### Changed

* Improved overall UI consistency to better align with the Reviewer2 theme.
* Refined submissions table with improved layout, sorting, pagination, and time display.
* Increased file upload limits to support large files (up to 500MB).
* Updated time handling by converting DateTime to DateTimeOffset.
* Simplified and stabilized admin role management functionality.

### Fixed

* File upload limitations that previously caused failures with large files.
* Authorization issues allowing unintended access to submission details.
* UI issues affecting usability and interaction within submission and admin pages.

### Notes

* This release focuses on stabilization, usability improvements, and deployment readiness.
* The core submission workflow is now more polished and performant.
* The system is better prepared for real-world usage, including large file handling and deployment.

---

## [demo-2] - 2026-02-20

### Added

* Core paper submission domain models, including Paper, PaperFile, Author, Review, ReviewAssignment, and ReviewTemplate.
* Initial database migrations for the submission and review system.
* `PaperSubmissionService` for handling draft creation, updates, and file uploads.
* `FileStorageService` for managing file uploads and retrieval.
* Paper submission interface with validation, author management, and file upload support.
* Paper listing and submission details pages.
* Paper reviewer page and admin role management page.
* `PaperQueryService` and supporting DTOs for retrieving submission data.
* File controller for serving uploaded papers and enabling PDF previews.
* Integration of MudBlazor and Bootstrap icons for improved UI.

### Changed

* Unified database contexts into a single `ApplicationContext` and standardized identity fields to use GUIDs.
* Enhanced navigation to support submission, reviewer, and admin workflows.
* Improved overall UI consistency using Bootstrap.
* Updated role initialization behavior.

### Removed

* Obsolete authorization placeholder page.
* Default role assignment during registration

### Fixed

* User profile update issues related to GUID handling.
* UI and validation issues in submission and authentication flows.

### Notes

* This release introduces the core paper submission and review infrastructure.
* Marks the transition from authentication-focused functionality to domain-specific features.
* Establishes a scalable architecture using services, DTOs, and improved separation of concerns.

---

## [demo-1] - 2026-02-06

### Added

* Initial project structure with Blazor frontend and supporting data and services layers.
* ASP.NET Identity integration with full authentication workflows (login, registration, password reset, email confirmation).
* Database context setup with initial migrations.
* `Reviewer2.Services` project for business logic and service-layer abstraction.
* Initial implementation of `ApplicationUserService`.
* DTO-based architecture for user-related operations.
* Role initialization service for seeding roles on startup.
* Extended user registration to include first name, last name, username, and email.
* User profile management functionality.

### Notes

* Passkey functionality was explored but disabled due to lack of database support.
* Introduced a service-oriented architecture to improve maintainability and separation of concerns.
* Establishes the foundation for future feature development.