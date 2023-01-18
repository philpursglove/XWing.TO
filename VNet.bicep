param location string
param abbreviation string

resource vnet 'Microsoft.Network/virtualNetworks@2022-07-01' = {
    name: 'vnet-XWingTO${abbreviation}'
    location: location

    properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'snet-xwingto-sql${abbreviation}'
        properties: {
          addressPrefix: '10.0.0.1/16'
        }
      }
      {
        name: 'snet-xwingto-web${abbreviation}'
        properties: {
          addressPrefix: '10.0.0.2/16'
        }
      }
      {
        name: 'snet-xwingto-function${abbreviation}'
        properties: {
          addressPrefix: '10.0.0.3/16'
        }
      }
    ]
  }
}