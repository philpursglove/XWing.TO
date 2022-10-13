param location string
param environment string
param abbreviation string

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'plan-XWingTO${abbreviation}'
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
}


resource appService 'Microsoft.Web/sites@2021-01-15' = {
  name: 'app-XWingTO${abbreviation}'
  location: location
  tags: {
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan': 'Resource'
  }
  properties: {
    serverFarmId: appServicePlan.id
  }
}
