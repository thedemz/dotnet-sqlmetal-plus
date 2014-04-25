SqlMetaPlus.sln

Is the Visual Studio (v. 12.0) 2013 Project

### ===================================
### Contains installation for versions:
### ===================================

	Visual Studio (v. 9.0) 2008

	Visual Studio (v. 10.0) 2010

	Visual Studio (v. 12.0) 2013

Compiled and Tested with MVC 4.5

### ===================================
### To Compile the code:
### ===================================

1.	WiX 3.8


Download at: http://wixtoolset.org/


2.	Microsoft Data Connection Dialog 1.2.0



The data connection dialog is a database tool component released with Visual Studio.
It allows users to build connection strings and to connect to specific data sources.

In order to use the data connection dialog independently of
Visual Studio you have to install the standalone source code.

To install Microsoft Data Connection Dialog,
run the following command in the Package Manager Console:


	PM> Install-Package DataConnectionDialog

And also the command

	PM> Install-Package IDataConnectionProperties

Info:	http://msdn.microsoft.com/en-us/library/microsoft.data.connectionui(v=vs.90).aspx


3.	Build the solution.

Change the properties for the MVC version you are using.