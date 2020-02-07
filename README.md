# DocumentManagementAPI

Following stuff is needed to run web api:
- .Net Core 2.2 (unfortunately it has reached end of official support)
- Azure storage emulator
- Microsoft SQL LocalDb

All required infrastructure(sql database, sql migrations, blob container) is setup during application startup for simplicity.
Ideally, it should be setup separately from web application using powershell, terraform scripts or ARM templates :)

Development plan: 
1. Design API data contracts
2. Choose appropriate data store. Chosen stores: azure blob storage for storing documents and enable user to download file by location, sql: to be able to set transactionally custom order of multiple documents 
3. Implement some use cases. Implemented enpoints: upload new document(max file size, invalid extension cases covered), get all uploaded documents
4. Cover implemented use cases with tests. Unfortunately I didn't have time to cover endpoints with tests.
If I had enough time to do that I would cover them with API tests. By the way, LightBDD(https://github.com/LightBDD/LightBDD) is the library which I usually use for writing tests and keep them easy to read and maintain.
