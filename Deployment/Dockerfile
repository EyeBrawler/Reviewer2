# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

RUN apt-get update && apt-get install -y git unzip && rm -rf /var/lib/apt/lists/*

# Clone repo
RUN git clone https://github.com/EyeBrawler/Reviewer2.git .

# Restore all projects
RUN dotnet restore Reviewer2/Reviewer2.Blazor/Reviewer2.Blazor.csproj
RUN dotnet restore Reviewer2/Reviewer2.Data/Reviewer2.Data.csproj

# Publish Blazor app
RUN dotnet publish Reviewer2/Reviewer2.Blazor/Reviewer2.Blazor.csproj -c Release -o /app/publish

# Stage 2: Runtime (using SDK for simplicity)
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /app/Reviewer2 /app/Reviewer2

# Install EF tool
RUN dotnet tool install --global dotnet-ef --version 10.*
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy wait-for-db script
COPY wait-for-db.sh .

# Expose port
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["sh", "wait-for-db.sh"]