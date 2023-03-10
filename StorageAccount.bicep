param location string
param environment string

var environmentSettings = {
    Development: {
        environmentAbbreviation: 'dev'
    }
    Test: {
        environmentAbbreviation: 'test'
    }
    Production: {
        environmentAbbreviation: ''
    }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'stxwingto${environmentSettings[environment].environmentAbbreviation}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource queueServices 'Microsoft.Storage/storageAccounts/queueServices@2022-09-01' = {
  name: 'default'
  parent: resourceSymbolicName
  properties: {
  }
}

resource scoreQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2022-09-01' = {
  name: 'scoreQueue'
  parent: queueServices
  properties: {
    metadata: {}
  }
}

output storageAccountName string = storageAccount.name