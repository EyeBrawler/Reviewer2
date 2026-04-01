# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Install git to clone repo
RUN apt-get update && apt-get install -y git unzip && rm -rf /var/lib/apt/lists/*

# Clone GitHub repo
RUN git clone https://github.com/EyeBrawler/Reviewer2.git .

# Publish the Blazor project
RUN dotnet publish Reviewer2/Reviewer2.Blazor/Reviewer2.Blazor.csproj -c Release -o /app/publish

# Stage 2: Runtime (using SDK for simplicity)
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Copy Data Project for EF migrations
COPY --from=build /app/Reviewer2/Reviewer2.Data /app/Reviewer2.Data

# Ensure EF tools are available
RUN dotnet tool install --global dotnet-ef --version 10.*
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy migration script
COPY wait-for-db.sh .

# Expose port
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Run migration script
ENTRYPOINT ["sh", "wait-for-db.sh"]