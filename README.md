# OctopusDeploy-Nautilus

[![Build status](https://ci.appveyor.com/api/projects/status/77fdul0exe7gjpg1?svg=true)](https://ci.appveyor.com/project/JohnCruikshank/octopusdeploy-nautilus-jl4x8)
[![Chocolatey](https://img.shields.io/chocolatey/v/nautilus.svg)](https://chocolatey.org/packages/nautilus)


Nautilus is a command line tool that helps to automate [Octopus](https://octopus.com/) deploys during horizontal scaling operations by determining all of the releases to deploy by the machine's role and environment.  It is also capable of downloading, installing, configuring, updating and registering the Octopus Tentacle.

## Install

Nautilus is available via [Chocolatey](https://chocolatey.org/) or direct download from [releases](https://github.com/Zywave/OctopusDeploy-Nautilus/releases/latest).  Additionally, a a simple Powershell script that downloads the latest nautilus.exe into the current directory is available to simplify automation.

##### Chocolately install
```
choco install nautilus
```

##### Powershell download

```
iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/Zywave/OctopusDeploy-Nautilus/master/download.ps1'))
```

```
@powershell -NoProfile -ExecutionPolicy Bypass -Command "iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/Zywave/OctopusDeploy-Nautilus/master/download.ps1'))"
```

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
|-f, --force|Specifies whether to force redeployment of releases to the target machine.|
|-o, --nonce|An arbritrary value to ensure that a deploy is only run once.  If the specified value matches a value previously used, this deploy will be prevented. The value is stored in an environment variable (NAUTILUS_NONCE) on the local machine.|

### Install
Installs and configures an Octopus Tentacle on the local machine.

*Local machine only*

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

### Upgrade
Upgrades the Octopus Tentacle on the target machine.

```
nautilus upgrade -s https://<your-octopus-server>/ -k <your-octopus-api-key>
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-n, --name|The target machine name. Defaults to the local machine name.|

### Register
Registers the target machine with the Octopus server.

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
|-u, --update|Specifies whether to update an existing registration.|

### Unregister
Unregisters the target machine from the Octopus server.

```
nautilus unregister -s https://<your-octopus-server>/ -k <your-octopus-api-key>
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-n, --name|The machine name. Defaults to the local machine name.|

### Purge
Unregisters offline machines in a specified role.

```
nautilus purge -s https://<your-octopus-server>/ -k <your-octopus-api-key> -r app
```

|Name|Description|
|---|---|
|-s, --server|Required. Octopus server address (e.g. http://your-octopus-server/).|
|-k, --apikey|Required. Octopus API key.|
|-r, --role|The machine role for which to purge offline nodes.|
