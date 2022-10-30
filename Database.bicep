param location string
param environment string
param abbreviation string
@secure()
param sqlAdminPassword string

resource sqlServer 'Microsoft.Sql/servers@2014-04-01' ={
  name: 'sql-xwingto${abbreviation}'
  location: location
  properties: {
      administratorLogin: 'darthvader'
      administratorLoginPassword: sqlAdminPassword
  }
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: 'sqldb-XWingTO${abbreviation}'
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    edition: 'Free'
    requestedServiceObjectiveName: 'Free'
  }
}
