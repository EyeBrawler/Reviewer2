# Deployment Resources for Reviewer2

This folder (``Deployment``) contains a variety of useful resources for deploying your own instance of Reviewer2. At the present moment, the fastest and easiest way to deploy Reviewer2 is via Docker Compose. To learn more on how to do this, see the next section, *Deploying with Docker Compose*. Additional resources for setting up a standalone Reviewer2 database with docker and configuring ``nginx`` are also below.

---

## Full Docker Compose Deployment (easiest)

### 1. Install and Prepare Docker and Docker Compose

First, update your system.

```bash
sudo apt update
```

```bash
sudo apt full-upgrade
```

Next, install docker and docker compose. On Ubuntu 24.04 and similar Ubuntu based Linux distributions.

```bash
sudo apt install docker.io docker-compose-v2
```

Then, check if the docker group exists.

```bash
grep docker /etc/group
```

If not, create it.

```bash
sudo groupadd docker
```

Then, add your user to the group.

```bash
sudo usermod -aG docker $USER
```

After upgrading and installing all of this software, a full reboot of your system is advisable. (docker can be weird)

### 2. Clone Reviewer2

If you do not have git installed (unlikely but possible)...

```bash
sudo apt install git
```

Clone the Reviewer2 code base. (feel free to specify a tag or branch if you like).

```bash
git clone https://github.com/EyeBrawler/Reviewer2.git
```

### Setup Environment Variables for Docker Compose

Navigate to the deployment directory within your clone of Reviewer2.

```bash
cd Reviewer2/Deployment
```

Open the hidden ``env`` file in a text editor of your choosing.

```bash
nano .env.fullstack
```

Below is an example of ``.env.fullstack``. The main fields worth changing here are the password (and perhaps the port number if you already have an instance of Postgres on port 5432 on your machine).

```env
# App environment
ASPNETCORE_ENVIRONMENT=Production

# Database
POSTGRES_USER=reviewer2
POSTGRES_PASSWORD=mysecurepassword123 <--- Your user's password goes here
POSTGRES_DB=reviewer2_prod
DB_HOST=db
DB_PORT=5432

# External ports
DB_EXTERNAL_PORT=5432 <--- You may want to change the port number if postgres is already installed
APP_EXTERNAL_PORT=5000

# Connection string (unchanged)
REVIEWER2_CONNECTION="Host=${DB_HOST};Port=${DB_PORT};Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"
```

---

## Using nginx with Reviewer2

Nginx can be used as a reverse proxy to allow the Kestrel web server (running in a daemon or docker container) to be on port 80/433.

First, make sure your system is up to date.

```bash
sudo apt update
```

```bash
sudo apt full-upgrade
```

Second, install nginx. On a Debian based Linux distribution like Ubuntu...

```bash
sudo apt install nginx
```

Then enable and start the ``nginx`` daemon.

```bash
sudo systemctl enable nginx
```

```bash
sudo systemctl start nginx
```

Next create a reviewer2 file for nginx.

```bash
sudo nano /etc/nginx/sites-available/reviewer2
```

Add these contents to the file. Adjust `myhost` as necessary. (``myhost`` will be your domain name registered with DNS)

```
server {
    listen 80;
    server_name myhost;

    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;

        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;

        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

After creating the file, enable the site with these commands.

```bash
sudo ln -s /etc/nginx/sites-available/reviewer2 /etc/nginx/sites-enabled/
```

```bash
sudo nginx -t
```

```bash
sudo systemctl reload nginx
```

Additional instructions for HTTPS and SSL may come in the future.

---

## The Publish Bash Script

 `publish.sh` is a bash script you can run to publish Reviewer2 within your local clone of the Reviewer2. It will automatically fetch the latest changes from the ``main`` branch of the repository each time you run it. When running the script, a folder titled `publish` will be created at the root of your repository and contain all build outputs from the script. This is useful if you plan on deploying Reviewer2 natively and want to have the web app running as a ``systemd`` daemon. To learn more about setting up the a ``systemd`` daemon for Reviewer2, see the section on *How to Create a systemd Daemon for Reviewer2.*

**Note**: The publish script requires installation of the .NET SDK.

## Standalone Database Deployment with Docker

If you want to install PostgreSQL separately from your web server (for whatever reason) or if you are developing Reviewer2, setting up a PostgreSQL database with docker can be significantly faster than installing PostgreSQL manually.

First, install docker if you have not already. On Debian based Linux distributions...

```bash
sudo apt install docker.io
```

Then, check if the docker group exists.

```bash
grep docker /etc/group
```

If not, create it.

```bash
sudo groupadd docker
```

Then, add your user to the group.

```bash
sudo usermod -aG docker $USER
```

For these changes to apply, you will have to log out.

After this, the simplest way to get a database going is to run this command. Port numbers, names, and passwords in the commands below can be changed to your liking.

```bash
docker run --name Reviewer2_Data -e POSTGRES_PASSWORD=mystrongpassword123 -p 5432:5432 -d postgres
```

Creating a more configured DBMS may look like...

```bash
docker run -d --name postgres --hostname postgres-db -e POSTGRES_USER=reviewer2 -e POSTGRES_PASSWORD="my_password" -e POSTGRES_DB=reviewer2_prod -p 5433:5432 postgres:latest
```

In the above line, port 5433 is being exposed rather than 5432 as to not conflict with an existing Postgres install.

To add the necessary tables to your database you will first need to set a user secret.

Navigate to the Reviewer2.Data directory of your clone of the repository. The command below is run from the root of the repo.
```bash
cd Reviewer2/Reviewer2.Data
```

Set your user secrets. Change fields like passwords and names to match the values you specified when creating the container.
```bash
dotnet user-secrets set "ConnectionStrings:Reviewer2Connection" "Host=localhost;Port=5432;Database=reviewer2;Username=myPostgresUser;Password=My Secure Password;Include Error Detail=true"
```
Update the database
```bash
dotnet ef database update
```

If you are missing the EF Core tools, you can install them with this command.
```bash
dotnet tool install --global dotnet-ef
```

## How to Create a systemd Daemon for Reviewer2

Creating a ``systemd`` daemon can be a convenient way to deploy Reviewer2 if you do not want to have your web server running in a Docker container. Before creating your service, make sure that you have a Postgres database available (if not, see *Standalone Database Deployment with Docker* or install natively for your OS) and verify that you can successfully run the ``publish.sh`` script.

After verifying the `publish.sh` script runs without errors, you need to create a reviewer2.service file.

```bash
sudo nano /etc/systemd/system/reviewer2.service
```

Within the reviewer2.service file, paste this...

```
[Unit]
Description=Reviewer2 Blazor Application
After=network.target

[Service]
WorkingDirectory=/home/reviewer2/Reviewer2/publish
ExecStart=/usr/bin/dotnet /home/reviewer2/Reviewer2/publish/Reviewer2.Blazor.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=reviewer2
User=reviewer2
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000
Environment=ConnectionStrings__Reviewer2Connection=Host=myhost;Port=5432;Database=reviewer2;Username=reviewer2;Password=mysecurepassword123

[Install]
WantedBy=multi-user.target
```

After `ConnectionStrings__Reviewer2Connection=`, remember to put in your own connection string. Also make sure that your file paths for `ExecStart` and `WorkingDirectory` are correct. The parent directories in each of the paths before ``Reviewer2/publish`` will vary depending on where you originally cloned the repository.

**Note:**  Using the ``~`` symbol in files paths will not work here. 

### Reload systemd and Enable the Service

After creating the service file, systemd doesn’t automatically pick it up.

```bash
sudo systemctl daemon-reexec
```

```bash
sudo systemctl daemon-reload
```

Then enable the service so it starts on boot:

sudo systemctl enable reviewer2

### Start the Service

sudo systemctl start reviewer2

### Checking Service Status (useful if something goes wrong)

sudo systemctl status reviewer2

Explain briefly what to look for:

- `active (running)` → good
- `failed` → something is wrong (check logs next)

### Viewing Service Logs (Critical for Debugging)

If something has gone very wrong, take a look at the logs for the daemon.

```bash
journalctl -u reviewer2 -f
```

```bash
journalctl -u reviewer2 --since "10 minutes ago"
```

This shows ASP.NET errors, database connection issues, etc. It is essentially the console output you would see when running Reviewer2 manually.

### Updating, Restarting, and Stopping the Service

To update Reviewer2 run...

```bash
git pull
```

```bash
sudo systemctl restart reviewer2
```
