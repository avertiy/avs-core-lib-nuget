## avs-core-lib
useful utils and extensions gathered together in one place 

## PowerConsole

inspired by https://github.com/dejanstojanovic/Power-Console

# What is PowerConsole?
PowerConsole is a .NET standard library where I gathred together and brought some own ideas 
of useful console stuff: 
 - writing & printing colored text to console, message statuses Debug, Info, Error etc.
 - user input extensions: ReadLine, ReadKey and ReadKeyAsync, PromptYesNo
 - getting/setting console font (font name, size & weight)
 - print utilities like printTable, printArray etc. 
 - printF - writing colored text to console empowered by custom formatter i.e. rich text formatting
requires string.Format analog like X.Format to be added through service extensions: services.AddPowerConsoleFormatter(x=> X.Format(x)), 
by default a standard string.Format is used i.e basic .NET formatting only


for usage see the demo app - ConsoleFeaturesDemoService.cs

# How to plusbish package to nuget

https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli

1. Open a command line and change to the folder containing the .nupkg file.

2. Run the following command, replacing the key value with your API key

dotnet nuget push AVS.CoreLib.PowerConsole.1.0.0.nupkg --api-key APIKEY --source https://api.nuget.org/v3/index.json


