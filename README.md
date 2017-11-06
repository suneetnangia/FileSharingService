# Azure Secure File Sharing Service
A generic file sharing service built on Azure PaaS services. Service provides restricted SAS tokens as per the policy configured by you. Authorisation is configured by the data in Cosmos DB.

Azure Services Utilised-
1. App Services
2. Key Vault
3. Storage Account
4. Cosmos DB
5. Application Insights

This is the implementation of second pattern "A lightweight service authenticates the client as needed and then generates a SAS" described here-
https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1


## Overall Design

<p align="center">
  <img src="https://github.com/suneetnangia/FileSharingService/blob/master/FileSharingServiceDesign.PNG?raw=true" width="800"/>  
</p>

## Notes-
1. Each storage account has some limits, please aware of those e.g. IOPS limit per account.
2. You can use Hot/Cold/Archive storage tiers to store files in storage account.
3. Consider strategy to move old files to archive/cool storage tiers periodically to optimise cost.
4. File Management API may split in to multiple smaller services in the same project but exposed as a single cohesive microservice.
5. Optionally, consider Azure Search on top of blob store/cosmos db to index and search files.
6. You can generate unlimited number of SAS tokens.
7. UI Layer will need to refresh sas token if the previous one is expired due to token TTL.
8. Monitor storage accounts for SAS token validation failures to proactively find the issues.
9. Please consider use of API Management service if you have multiple services exposed to applications. 
10. Upload Flow- Lead user logs in via AAD, AAD returns claim lead=true, API validates the token, returns the write only SAS token at the container level to UI. Allowed users must present the claim which allowed by API via reference data stored in Cosmos DB, to start with, it will be Lead=True. Permission matrix will include group name and files mapping.
When uploading files, AAD claims which include the Trust specific AAD group name which will be used in permission matrix to allow anyone in that group to manage files.

11. SAS (Service SAS, adhoc) token for downloaders can have the following limitations-

*IP

*Token TTL

*Https only access

*On demand when download link is clicked.

12. Invalidation of SAS token is important, please consider this thoroughly. 

## Cosmos DB Doc Structure

{
    "id": "replace_with_new_document_id",
    "Identity": "suneetnangia@yxz.com",
    "File": {
        "Container": "allfiles",
        "Path": "article1.json",
        "Permissions": 1
    },
    "_rid": "XRwCAIvO8AABAAAAAAAAAA==",
    "_self": "dbs/XRwCAA==/colls/XRwCAIvO8AA=/docs/XRwCAIvO8AABAAAAAAAAAA==/",
    "_etag": "\"3600e867-0000-0000-0000-5a003ec70000\"",
    "_attachments": "attachments/",
    "_ts": 1509965511
}

Reference Documents-

https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1
https://docs.microsoft.com/en-us/azure/azure-subscription-service-limits#storage-limits
https://docs.microsoft.com/en-us/azure/virtual-network/ddos-protection-overview




