# App Center Build App
Console application for building application, which is configured in [Visual Studio App Center](https://appcenter.ms/). This app uses [App Center API](https://openapi.appcenter.ms/) to retrieving branches of target app and then requests new build for each of them. 

When build is finished, app prints log message with information about build result in the next format: <br>*\<branch name\> build \<completed/failed\> in \<build duration\> seconds. Link to build logs: \<link\>*.

## How to use
Build project and start AppCenterBuildApp.exe file. By default application requires *inputData.json* file to get App Center API Token and name of target application for build. You can pass your own filename in commandline arguments or customize parameters in default *inputData.json* file.
Input data file should be formatted as JSON and contains the following keys: 
- **ApiToken** (string) - Api token for authentication in App Center API ([read more](https://docs.microsoft.com/en-us/appcenter/api-docs/))
- **TargetApp** (string) - Name of target application for build

Example of input data:
>{
>  "ApiToken": "*Actual value of API token from App Center*",
>  "TargetApp": "TestApp"
>}

For correct work a repository with one or several branches should be connected to the target application in the [App Center Build](https://docs.microsoft.com/en-us/appcenter/build/) section
