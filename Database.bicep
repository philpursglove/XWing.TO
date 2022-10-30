param location string
param environment string
param abbreviation string
@secure()
param sqlAdminPassword string

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-xwingto${abbreviation}'
  location: location
  properties: {
      administratorLogin: 'darthvader'
      administratorLoginPassword: sqlAdminPassword
      publicNetworkAccess: 'Disabled'
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
      name: 'Free'

  }
}

// Needs to be calculated
output connectionString string = ''