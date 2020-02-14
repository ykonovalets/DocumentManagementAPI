# DocumentManagementAPI

Following stuff is needed to run web api:
- .Net Core 2.2 (unfortunately it has reached end of official support)
- Started Azure storage emulator
- Started Microsoft SQL LocalDb

All required infrastructure(sql database, sql migrations, blob container) is setup during application startup for simplicity.
Ideally, it should be setup separately from web application using powershell, terraform scripts or ARM templates :)

Development plan: 
1. Design API data contracts. Document contracts via swagger 
2. Choose appropriate data store. Chosen stores: azure blob storage for storing documents and enable user to download file by location, sql: to be able to set transactionally custom order of multiple documents 
3. Implement some use cases and cover them with unit tests. Implemented enpoints: upload new document(max file size, invalid extension cases covered), get all uploaded documents
4. Cover implemented use cases with API tests. Unfortunately I didn't have time to cover endpoints with API tests since it's time-consuming task(spinning up Kestrel web server or TestServer with startup configuration, reset data stores before each test start)
If I had enough time to do that I would cover them using LightBDD(https://github.com/LightBDD/LightBDD) library which I usually use for writing tests and keep them easy to read and maintain.
