# Azure Secure File Sharing Service
A generic file sharing service built on Azure.

This is the implementation of second pattern "A lightweight service authenticates the client as needed and then generates a SAS" described here-
https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1

Security Design Points-
1. SAS token is generated on demand eveytime a user clicks on the download link. Generated token will have the following constraints (this can be extented to more constraints), which is managed by the injected policy-
* TTL
* IP Restriction
* Specific Access Level (e.g. ReadOnly)
2. Storage Account master keys are stored securely in Azure Key Vault. Key is requested on demand by the Token generation API to generate the SAS token requested by the client. Key is then disposed, never stored by the API anywhere. 

Azure Storage Account (ASA) key management pattern used is described here-

https://docs.microsoft.com/en-us/azure/key-vault/key-vault-ovw-storage-keys

File Map Design-
What is file map? It is a set of related documents which provide mapping between files, users and their access levels. 

How is it implemented?
File Map is implemented as a RESTful service backed by CosmosDB. 
Reasons for using CosmosDB in backend are-
1. Native support for JSON documents.
2. Low latency (select parition key wisely).
3. Searcheable data (if designed correctly).
4. Scalable store (select parition key wisely).
5. Resilient store (if designed correctly).

Partition key selection process-
Selecting partition key in Cosmos DB is an important decision for various reasons like you cannot change the partition key once selected but more importantly data is paritioned on this key which allows Cosmos DB to scale horizontaly which has impact on the query latency.
In our case, there will be more reads than writes i.e. people will downloading/listing files more than uploading hence partition key design needs to consider this read vs write ratio. 
Point here is, fanning out a query to muttiple partitions will take longer than sending it to a single partition and on the contrary parallel writes to a single partition will longer than writing to multiple partitions. You cannot have both, our balance is more towards read than write for this solution.
To learn more about partitioning design, see here- https://docs.microsoft.com/en-us/azure/cosmos-db/partition-data#partitioning-in-azure-cosmos-db

We've the following collections in my database-
1. Users (with partition key user id)
2. Files (with partition key filepath)
3. Maps (with partition key user id)

### Resources-
1. Storage Explorer to manage items in Storage Account and Cosmos DB (new feature, not used here yet). https://azure.microsoft.com/en-us/features/storage-explorer/
2. Managed Service Indentity- https://docs.microsoft.com/en-us/azure/app-service/app-service-managed-service-identity