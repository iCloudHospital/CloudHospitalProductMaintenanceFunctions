## Settings 

### BlobStorageOptions:ConnectionString

Azure Storage Account 연결문자열을 설정합니다.

Set Azure Storage Account connection string

### BlobStorageOptions:ContainerName

대상 컨테이너 이름을 설정합니다. 여러개를 지정할 때에는 쉼표로 구분된 문자열을 입력합니다.

Set cleaning target container names; string list as separated by comma.

e.g.) test1,test2

### AuthorizationOptions:ApiKey

api key 를 설정합니다. 설정하지 않으면 익명 액세스가 가능합니다.

Set api key, Otherwise anonymous access. 


## Usages

```bash
$ curl --header "ch-api-key:samplekey" --request GET "http://localhost:7071/api/cleanAssetsOnPrdStageFunction?containername=test-func"
```

컨테이너 이름 쿼리 문자열이 요청 url을 통해 전달되는 경우 서버 설정에서 대상 컨테이너 이름을 무시합니다.

If container name query string is pass through request url, Ignore target container name on server setting. 


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
    "BlobStorageOptions:ConnectionString": "<azure blob storage connection string>",
    "BlobStorageOptions:ContainerName": "<Target container names: e.g.) test1,test2,test3>",
    "AuthorizationOptions:ApiKey": "api-key"
  }
}
```
