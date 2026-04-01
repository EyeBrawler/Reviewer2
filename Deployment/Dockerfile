# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution + project files first (better caching)
COPY Reviewer2/*.sln ./Reviewer2/
COPY Reviewer2/Reviewer2.Blazor/*.csproj ./Reviewer2/Reviewer2.Blazor/
COPY Reviewer2/Reviewer2.Data/*.csproj ./Reviewer2/Reviewer2.Data/
COPY Reviewer2/Reviewer2.Services/*.csproj ./Reviewer2/Reviewer2.Services/

# Restore dependencies (cached unless csproj changes)
RUN dotnet restore Reviewer2/Reviewer2.Blazor/Reviewer2.Blazor.csproj

# Now copy the rest of the source
COPY Reviewer2/. ./Reviewer2/

# Publish app
RUN dotnet publish Reviewer2/Reviewer2.Blazor/Reviewer2.Blazor.csproj \
    -c Release -o /app/publish

# Stage 2: Runtime (SDK for EF support)
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Copy full source (needed for EF migrations)
COPY --from=build /src/Reviewer2 /app/Reviewer2

# Install EF CLI
RUN dotnet tool install --global dotnet-ef --version 10.*
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy startup script
COPY wait-for-db.sh .

# App config
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["sh", "wait-for-db.sh"]