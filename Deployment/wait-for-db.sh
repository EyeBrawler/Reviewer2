#!/bin/sh
set -e

# Wait until the database is reachable
echo "Waiting for database at $ConnectionStrings__Reviewer2Connection..."
until dotnet ef database update --project /app/Reviewer2.Data/Reviewer2.Data.csproj; do
  echo "Database not ready yet, retrying in 5 seconds..."
  sleep 5
done

# Start the Blazor app
echo "Starting Reviewer2 Blazor app..."
exec dotnet Reviewer2.Blazor.dll