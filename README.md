# rommaster
ROM Master helps to manage collections of games supported by an emulator

### Requirements

1. [.Net Core 2.1](https://www.microsoft.com/net/download/dotnet-core)

### Test

Execute `/devops/test.cmd`

### Installation

File `tokens.dev.json` allows you to control deployment configuration.
You should consider changing names of the Azure resource like KeyVault and StorageAccount in this file.

Then run batch scripts in the following order:

1. bootstrapper.cmd
2. publish.cmd

