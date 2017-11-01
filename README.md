# Azure Secure File Sharing Service
A generic file sharing service built on Azure PaaS services.

Azure Services Utilised-
1. App Services
2. Key Vault
3. Storage Account
4. Cosmos DB
5. Application Insights

This is the implementation of second pattern "A lightweight service authenticates the client as needed and then generates a SAS" described here-
https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1


##Design

##Security Aspects-
1. Token is generated on demand by the API against the request from the client/UI.
2. Token is valid for only shortest required time e.g. 1min.
3. Token is 
