targetScope = 'subscription'

@allowed([
  'Development'
  'Test'
  'Production'
])
param environment string

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
  name: 'rg-XWingTO${abbreviation}'
  location: 'uksouth'
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