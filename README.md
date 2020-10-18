# avs-core-lib
useful utils and extensions gathered together in one place 

# PowerConsole

inspired by https://github.com/dejanstojanovic/Power-Console
gathred together and brought some own ideas of Console extension methods
many useful stuff in one place 
 - writing & printing colored text to console, message statuses Debug, Info, Error etc.
 - user input extensions: ReadLine, ReadKey and ReadKeyAsync, PromptYesNo
 - getting/setting console font (font name, size & weight)
 - printF - writing colored text to console empowered by custom formatter, 
requires one like X.Format to be added through service extensions: services.AddPowerConsoleFormatter(x=> X.Format(x)), by default a standard string.Format is used
