### Features

* lib methods throw exceptions for errors rather than return error codes ([a1213b6](https://github.com/zywave/OctopusDeploy-Nautilus/commit/a1213b6))


### BREAKING CHANGES

* INautilusService methods no longer return codes, but
instead throw exceptions with an error code attached