@allowed([
  'Development'
  'Test'
  'Production'
])
param environment string

param location string

var environmentAbbreviation = environment == 'Production' ? '' : environment == 'Test' ? '-test' : '-dev'

targetScope = 'subscription'

resource rgXWing 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: 'rg-XWingTO${environmentAbbreviation}'
  location: location
}

module appservice 'AppService.bicep' = {
    name: 'appservice'
    scope: rgXWing
    params: {
        location: rgXWing.location
        environment: environment
        abbreviation: environmentAbbreviation
    }
}