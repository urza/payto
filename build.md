### how to build on ubuntu

opt out of the telemetry<br/>
`export DOTNET_CLI_TELEMETRY_OPTOUT=1`

install dotnet 8 sdk<br/>
`sudo apt-get install -y dotnet-sdk-8.0`

clone the repo<br/>
`git clone https://github.com/urza/payto && cd payto`

compile as single binary<br/>
`dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true`

and you will find `payto` binary in `/payto/bin/Release/net8.0/linux-x64/publish/` - copy it to where you like, but it must be able to call/execute lightning-cli<br/>

or alternatively run without compiling - navigate into the payto directory and:<br/>
`dotnet run`
