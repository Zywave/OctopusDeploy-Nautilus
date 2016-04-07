# OctopusDeploy-Nautilus

[![Build status](https://ci.appveyor.com/api/projects/status/77fdul0exe7gjpg1?svg=true)](https://ci.appveyor.com/project/JohnCruikshank/octopusdeploy-nautilus-jl4x8)
[![Chocolatey](https://img.shields.io/chocolatey/v/nautilus.svg)](https://chocolatey.org/packages/nautilus)


Nautilus is a command line tool that helps to automate [Octopus](https://octopus.com/) deploys during horizontal scaling operations by determining all of the releases to deploy by the machine's role and environment.  It is also capable of downloading, installing, configuring, updating and registering the Octopus Tentacle.

## Commands

### Deploy
Creates deployments for the latest release of all projects related to the target machine by role and environment.

```
nautilus deploy -s https://<your-octopus-server>/ -k <your-octopus-api-key> -w
```

|Argument|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-n, --name|The target machine name. Defaults to the local machine name.|
|-w, --wait|Specifies whether to wait for each deployment to complete before exiting.|

### Install
Installs and configures an Octopus Tentacle on the local machine.

```
nautilus install -s https://<your-octopus-server>/ -k <your-octopus-api-key>
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-l, --location|The install directory of the Octopus Tentacle. Defaults to Program Files.|
|-h, --home|The home directory of the Octopus Tentacle. Defaults to "C:\Octopus"|
| -t, --thumbprint|The Octopus Server thumbprint. Defaults to global certificate thumbprint.|
|-p, --port|The port of the Octopus Tentacle. Defaults to 10933.|

### Update
Updates the Octopus Tentacle on the target machine.

```
nautilus update -s https://<your-octopus-server>/ -k <your-octopus-api-key>
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-n, --name|The target machine name. Defaults to the local machine name.|

### Register
Registers the local machine with the Octopus server.

```
nautilus register -s https://<your-octopus-server>/ -k <your-octopus-api-key> -e PROD -r app
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-e, --environments|Required. The environment names (space separated) of the machine.|
|-r, --roles|Required. The roles  (space separated) of the machine.|
|-n, --name|The machine name. Defaults to the local machine name.|
|-t, --thumbprint|The Octopus Tentacle thumbprint. Defaults to the local Tentacle thumbprint.|
|-h, --host|The Tentacle host name. Defaults to the local machine name.|
|-p, --port|The Tentacle port. Defaults to 10933.|

### Unregister
Unregisters the local machine from the Octopus server.

```
nautilus unregister -s https://<your-octopus-server>/ -k <your-octopus-api-key>
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-n, --name|The machine name. Defaults to the local machine name.|
