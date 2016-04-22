<a name="1.2.0"></a>
# [1.2.0](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.1.0...v1.2.0) (2016-04-22)


### Features

* lib methods throw exceptions for errors rather than return error codes ([a1213b6](https://github.com/zywave/OctopusDeploy-Nautilus/commit/a1213b6))


### BREAKING CHANGES

* INautilusService methods no longer return codes, but
instead throw exceptions with an error code attached



<a name="1.1.0"></a>
# [1.1.0](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.4...v1.1.0) (2016-04-21)


### Bug Fixes

* log error message of validation exception instead of full stack ([d9b3141](https://github.com/zywave/OctopusDeploy-Nautilus/commit/d9b3141))
* not logging full exception string for validation errors ([32f9b52](https://github.com/zywave/OctopusDeploy-Nautilus/commit/32f9b52))
* retry msi exec if another installer is running ([465533a](https://github.com/zywave/OctopusDeploy-Nautilus/commit/465533a)), closes [#12](https://github.com/zywave/OctopusDeploy-Nautilus/issues/12)

### Features

* library build and packaging ([dce3de2](https://github.com/zywave/OctopusDeploy-Nautilus/commit/dce3de2))



<a name="1.0.4"></a>
## [1.0.4](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.3...v1.0.4) (2016-04-18)


### Bug Fixes

* trim trailing slashes from directories as Tentacle.exe commands do not like them ([df1be10](https://github.com/zywave/OctopusDeploy-Nautilus/commit/df1be10))



<a name="1.0.3"></a>
## [1.0.3](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.2...v1.0.3) (2016-04-18)


### Features

* verbose install output ([8cbbe03](https://github.com/zywave/OctopusDeploy-Nautilus/commit/8cbbe03))



<a name="1.0.2"></a>
## [1.0.2](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.1...v1.0.2) (2016-04-18)


### Bug Fixes

* check for file and directory existence before calling delete file on install command ([2b4a575](https://github.com/zywave/OctopusDeploy-Nautilus/commit/2b4a575))

### Features

* applications directory option on install ([2b4a575](https://github.com/zywave/OctopusDeploy-Nautilus/commit/2b4a575))


<a name="1.0.1"></a>
## [1.0.1](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0...v1.0.1) (2016-04-15)


### Bug Fixes

* default home directory to system drive ([93a793c](https://github.com/zywave/OctopusDeploy-Nautilus/commit/93a793c)), closes [#8](https://github.com/zywave/OctopusDeploy-Nautilus/issues/8)
* delete previous tentacle.config ([696156d](https://github.com/zywave/OctopusDeploy-Nautilus/commit/696156d))



<a name="1.0.0"></a>
# [1.0.0](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.8...v1.0.0) (2016-04-13)




<a name="1.0.0-prerelease.8"></a>
# [1.0.0-prerelease.8](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.7...v1.0.0-prerelease.8) (2016-04-13)


### Features

* registration update option ([625b011](https://github.com/zywave/OctopusDeploy-Nautilus/commit/625b011))



<a name="1.0.0-prerelease.7"></a>
# [1.0.0-prerelease.7](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.6...v1.0.0-prerelease.7) (2016-04-12)


### Bug Fixes

* nonce check returns 0 ([f7ddb8f](https://github.com/zywave/OctopusDeploy-Nautilus/commit/f7ddb8f))



<a name="1.0.0-prerelease.6"></a>
# [1.0.0-prerelease.6](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.5...v1.0.0-prerelease.6) (2016-04-12)


### Features

* deploy nonce ([19049de](https://github.com/zywave/OctopusDeploy-Nautilus/commit/19049de))
* detect whether tentacle is installed ([8cecefe](https://github.com/zywave/OctopusDeploy-Nautilus/commit/8cecefe))



<a name="1.0.0-prerelease.5"></a>
# [1.0.0-prerelease.5](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.4...v1.0.0-prerelease.5) (2016-04-11)


### Bug Fixes

* run tentacle.exe in console mode to get thumbprint ([a7e937c](https://github.com/zywave/OctopusDeploy-Nautilus/commit/a7e937c))



<a name="1.0.0-prerelease.4"></a>
# [1.0.0-prerelease.4](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.3...v1.0.0-prerelease.4) (2016-04-11)


### Features

* inject dependencies into exe using LibZ ([a3302e0](https://github.com/zywave/OctopusDeploy-Nautilus/commit/a3302e0)), closes [#1](https://github.com/zywave/OctopusDeploy-Nautilus/issues/1)



<a name="1.0.0-prerelease.3"></a>
# [1.0.0-prerelease.3](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.2...v1.0.0-prerelease.3) (2016-04-08)


### Bug Fixes

* get updated task after waiting for completion ([08336b8](https://github.com/zywave/OctopusDeploy-Nautilus/commit/08336b8)), closes [#6](https://github.com/zywave/OctopusDeploy-Nautilus/issues/6)

### Features

* previous deploy detection ([d688f10](https://github.com/zywave/OctopusDeploy-Nautilus/commit/d688f10)), closes [#7](https://github.com/zywave/OctopusDeploy-Nautilus/issues/7)
* purge command ([658366a](https://github.com/zywave/OctopusDeploy-Nautilus/commit/658366a)), closes [#2](https://github.com/zywave/OctopusDeploy-Nautilus/issues/2)



<a name="1.0.0-prerelease.2"></a>
# [1.0.0-prerelease.2](https://github.com/zywave/OctopusDeploy-Nautilus/compare/1.0.0-prerelease.0...v1.0.0-prerelease.2) (2016-04-06)


### Bug Fixes

* Fixing incorrect exe version

<a name="1.0.0-prerelease.1"></a>
# 1.0.0-prerelease.1 (2016-04-06)


### Features

* console messages, verbs, stubbed commands ([97ed0ff](https://github.com/zywave/OctopusDeploy-Nautilus/commit/97ed0ff))
* core role based deploy logic ([866c3d3](https://github.com/zywave/OctopusDeploy-Nautilus/commit/866c3d3))
* deploy, install, register, unregister commands ([afb10ae](https://github.com/zywave/OctopusDeploy-Nautilus/commit/afb10ae))
* environment display names ([ee332b2](https://github.com/zywave/OctopusDeploy-Nautilus/commit/ee332b2))
* tentacle configuration part of install, command cleanup ([cae09a2](https://github.com/zywave/OctopusDeploy-Nautilus/commit/cae09a2))
* update command ([915921b](https://github.com/zywave/OctopusDeploy-Nautilus/commit/915921b))
* wait for deploy option, console cleanup ([ab1238a](https://github.com/zywave/OctopusDeploy-Nautilus/commit/ab1238a))



