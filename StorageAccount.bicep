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
 output storageAccountName string = storageAccount.name