# OctopusDeploy-Nautilus

Nautilus helps to automate [Octopus](https://octopus.com/) deploys during horizontal scaling operations by determining all of the releases to deploy by the machine's role and environment.  It is also capable of downloading, installing, configuring, updating and registering the Octopus Tentacle.

## Commands

### Deploy
Creates deployments for the latest release of all projects related to the target machine by role and environment.

### Install
Installs and configures an Octopus Tenticle on the local machine.

### Update
Updates the Octopus Tenticle on the target machine.

### Register
Registers the local machine with the Octopus server.

### Unregister
Unregisters the local machine from the Octopus server.
