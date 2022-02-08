# I Am Ghost
A small storage service for storing steps in ghosts and replays.

## Usage

### Inside of a container

Requires docker desktop to be installed

Navigate to the root of the repo and run `docker compose up -d` making sure port 8080 is free.

### Outside of a container

Requires .net 6+ to be installed

Add `build.ps1 <Version> <Output Location>` to your build process replacing both with the relevant values. Or code up your own script to build it.

### Running as a service (Currently Windows)

Once built, you can install/uninstall via powershell using this: https://docs.microsoft.com/en-us/dotnet/framework/windows-services/how-to-install-and-uninstall-services

## Future Plans

- Adding support for SQL and NoSQL storage solutions.
- Adding an actual user guide.
- Adding support for cloud storage services.


