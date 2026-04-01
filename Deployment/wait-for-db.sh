#!/bin/sh
set -e

echo "Waiting for database at $ConnectionStrings__Reviewer2Connection..."

cd /app/Reviewer2

dotnet restore

# Move into the Data project directory
cd /app/Reviewer2/Reviewer2.Data

# Retry until DB is ready and migrations succeed
until dotnet ef database update --startup-project ../Reviewer2.Blazor; do
  echo "Database not ready yet, retrying in 5 seconds..."
  sleep 5
done

echo "Database ready!"

# Go back to app root to run Blazor
cd /app

echo "Starting Reviewer2 Blazor app..."
exec dotnet Reviewer2.Blazor.dll