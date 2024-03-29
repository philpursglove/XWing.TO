﻿param location string
param environment string
param abbreviation string

var skuSettings = {
    Development: {
        skuAbbreviation: 'B1'
    }
    Test: {
        skuAbbreviation: 'B1'
    }
    Production: {
        skuAbbreviation: 'S1'
    }
}

var skuSetting = skuSettings[environment].skuAbbreviation

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'plan-xwingto${abbreviation}'
  location: location
  sku: {
    name: skuSetting
    capacity: 1
  }
}

resource appService 'Microsoft.Web/sites@2021-01-15' = {
  name: 'app-xwingto${abbreviation}'
  location: location
  tags: {
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan': 'Resource'
  }
  properties: {
    serverFarmId: appServicePlan.id
  }
}

resource appServiceStack 'Microsoft.Web/sites/config@2021-01-15' = {
  parent: appService
  name: 'metadata'
  kind: 'web'
  properties: {
    CURRENT_STACK : 'dotnetcore'
  }
}

var environmentNames = {
  Development: {environmentName: 'Development'}
  Test: {environmentName: 'Staging'}
  Production: {environmentName: 'Production'}
}

var deploySettings = (environment == 'Development' || environment == 'Testing')

resource appSettings 'Microsoft.Web/sites/config@2022-03-01' = if(deploySettings) {
  parent: appService
  name: 'appsettings'
  kind: 'string'
  properties: {
    'ASPNETCORE_ENVIRONMENT': environmentNames[environment].environmentName
  }
}

output appServiceName string = appService.name
