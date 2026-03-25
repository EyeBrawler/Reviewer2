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