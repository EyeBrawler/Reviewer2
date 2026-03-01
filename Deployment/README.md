# Deployment Resources for Reviewer2

This directory contains a `publish.sh` file you can run to publish Reviewer2. It will automatically fetch the latest 
changes from the main branch of the repository each time you run it.

A folder titled `publish` will be created at the root of your repository where all the build output will live.

## How to create a systemd daemon for Reviewer2
After verifying the `publish.sh` script runs, you need to create a reviewer2.service file.
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

After `ConnectionStrings__Reviewer2Connection=`, remember to put in your own connection string. Also make sure that your
file paths for `ExecStart` and `WorkingDirectory` are correct.

## Using nginx with Reviewer2
Nginx can be used as a reverse proxy to allow the Kestrel web server (what runs with the daemon) to be on port 80/433.

First, install nginx. On a Debian based Linux distribution like Ubuntu...
```bash
sudo apt install nginx
```
Then enable and start the nginx daemon.
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
Add these contents to the file. Adjust `myhost` as necessary.
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