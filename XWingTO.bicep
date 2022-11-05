targetScope = 'subscription'

param location string = deployment().location
@allowed([
    'Development'
    'Test'
    'Production'
])
param environment string
@secure()
param sqlAdminPassword string

var environmentSettings = {
    Development: {
        environmentAbbreviation: '-dev'
    }
    Test: {
        environmentAbbreviation: '-test'
    }
    Production: {
        environmentAbbreviation: ''
    }
}

var abbreviation = environmentSettings[environment].environmentAbbreviation

resource rgXWing 'Microsoft.Resources/resourceGroups@2021-01-01' = {
    name: 'rg-xwingto${abbreviation}'
    location: location
}

module database 'Database.bicep' = {
    name: 'database'
    scope: rgXWing
    params: {
        location: rgXWing.location
        environment: environment
        abbreviation: abbreviation
        sqlAdminPassword: sqlAdminPassword
    }
}

module appservice 'AppService.bicep' = {
    name: 'appservice'
    scope: rgXWing
    params: {
        location: rgXWing.location
        environment: environment
        abbreviation: abbreviation
    }
}

module appInsights 'AppInsights.bicep' = {
    name: 'appInsights'
    scope: rgXWing
    params: {
        location: rgXWing.location
        abbreviation: abbreviation
    }
}

module storage 'StorageAccount.bicep' = {
    name: 'storage'
    scope: rgXWing
    params: {
        location: location
        environment: environment
    }
}

module functionApp 'FunctionApp.bicep' = {
    name: 'functionApp'
    scope: rgXWing
    params: {
        location: location
        abbreviation: abbreviation
        storageAccountName: storage.outputs.storageAccountName
        appInsightsId: appInsights.outputs.appInsightsId
    }
}