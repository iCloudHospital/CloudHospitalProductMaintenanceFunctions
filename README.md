## Settings 

### BlobStorageOptions:ConnectionString

Azure Storage Account 연결문자열을 설정합니다.

Set Azure Storage Account connection string

## Usages

```bash
$ curl --request GET "http://localhost:7071/api/cleanAssetsOnPrdStageFunction?containername=test-func"
```

## development environment

### Install Azure Functions Tools

Install Azure Functions Core Tools, please read blow web site.

[Work with Azure Functions Core Tools](https://docs.microsoft.com/ko-kr/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#v2)

Create local.settings.json file:

```bash
$ touch local.settings.json
```

Edit local.settings.json file:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "BlobStorageOptions:ConnectionString": "<azure blob storage connection string>"
  }
}
```

### Run Local environment

```bash
$ func start
```

## Trouble shootings

## Azure Functions Settings

### azure function Did not find functions with language [dotnet].

`Settings > Configuration > Application settings`

`FUNCTIONS_WORKER_RUNTIME` settings value should be `dotnet-isolated`

Name: `FUNCTIONS_WORKER_RUNTIME`
value: `dotnet-isolated`