param location string
param abbreviation string

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'func-xwingto${abbreviation}'
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: 'plan-xwingto${abbreviation}'
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${7:storageAccountName2};AccountKey=${listKeys(${8:'storageAccountID2'}, '2019-06-01').key1}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${9:storageAccountName3};AccountKey=${listKeys(${10:'storageAccountID3'}, '2019-06-01').key1}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(${11:'name'})
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(${12:'insightsComponents.id'}, '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
      ]
    }
  }
}
