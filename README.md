###################
What is in the Repository?
###################

This repository contains a project I worked on while learning ASP.NET Web API with Ultimate ASP.Net Core Web API.
this repository contains all the topics discussed in the book some I changed myself i.e. the Authentication controller and the mail service are not addressed in the book.
one can find this repository valuable if he/she is looking for a project-starter kit or directory structure this project follows Onion Architecture.

*******************
Server Requirements
*******************

.NET version 7 or newer is recommended.

It should work on .NET 6 as well, but I strongly advise you NOT to run
such old versions of .NET, because of potential security and performance
issues, as well as missing features.

************
How to Run this Project
************
- clone this repository https://github.com/MikiyasWT/CompanyEmployees
- cd into CompanyEmployees
- open the terminal and run the command "dotnet restore CompanyEmployees.sln"
- Now if you are on a Unix system(ubuntu or Mac) download the MSSQL server image from docker, remember to configure it wisely and keep the password and username while installing the image. if you are 	running on Windows start MSSQL service
- for visualization purposes, you can use Azure Data Studio on Mac/Ubuntu or Microsoft SQL Server Management Studio on Windows.
- configure your variables inside appsetings.json file
- 
  "ConnectionStrings": {
    "sqlConnection":"Server=IPaddress,port;Database=CompanyEmployee;User Id=your user name;TrustServerCertificate=True;Password=password"
  }
- if you are on Mac/ubuntu use the above connection string format otherwise use the following

  "ConnectionStrings": {
	    "DefaultConnection": "Server=server\\SQLEXPRESS; User ID=user id; Password=password; Database=CompanyEmployee;TrustServerCertificate=True"
	  }
	
- start the SQL server image you downloaded above
- once sql server is started
- run cd CompanyEmployees
- dotnet restore
- dotnet run
- Now for test purpose goto Postman and run this URL http://localhost:5229/api/companies using the get method
- if everything is ok you will get a 200 ok response, otherwise, it will return Internal

- if you have any questions or suggestion please reach out to me on Linkedin I will try to respond as fast as possible



