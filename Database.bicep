param location string
param environment string
param abbreviation string
@secure()
param sqlAdminPassword string

var skuSettings = {
  Development: {
    capacity: 1
    family: 'Gen5'
    name: 'GP_S_Gen5'
    size: null
    tier: 'GeneralPurpose'
  }
  Test: {
    capacity: 1
    family: 'Gen5'
    name: 'GP_S_Gen5'
    size: null
    tier: 'GeneralPurpose'
  }
  Production: {
    capacity: 10
    family: null
    name: 'Standard'
    size: null
    tier: 'Standard'
  }
}

var skuSetting = skuSettings[environment]

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-xwingto${abbreviation}'
  location: location
  properties: {
      administratorLogin: 'darthvader'
      administratorLoginPassword: sqlAdminPassword
      publicNetworkAccess: 'Disabled'
      minimalTlsVersion: '1.2'
  }
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  parent: sqlServer
  name: 'sqldb-xwingto${abbreviation}'
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
  sku: {
      capacity: skuSetting.capacity
      family: skuSetting.family
      name: skuSetting.name
      size: skuSetting.size
      tier: skuSetting.tier
  }
}

// Needs to be calculated
output connectionString string = ''