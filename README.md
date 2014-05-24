dotnet-sqlmetal-plus
====================

The intermediate database markup language (.dbml) file is not properly updated when changing the underlying SQL model.

There is a command-line tool SqlMetal.exe that is a code generation tool to generates this code and mapping for the LINQ to SQL component of the .NET Framework. This tool is automatically installed with Visual Studio.

The project adds a plugin for visual studio so SQLMetal so with just one click the code generation tool will be run to force an updated .dbml file.

The intermediate database markup language (.dbml) file for customization is not always automatically updated in Visual Studio, therefore SQLMetal is needed to force this code regeneration. 

Origin Source:

http://sqlmetalplus.codeplex.com

http://www.codeproject.com/Articles/37198/SqlMetalPlus-A-VS-Add-in-to-Manage-Custom-Changes

The origin code should work on Visual Studio 2008 and 2010.

Added additional code to get it working on Visual Studio 2013.


Build the code or just download the binaries found in SetupProject/bin/Release
