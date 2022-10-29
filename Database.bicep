param location string
param environment string
param abbreviation string

resource sqlServer 'Microsoft.Sql/servers@2014-04-01' ={
  name: 'sql-XWingTO${abbreviation}'
  location: location
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: 'sqldb-XWingTO${abbreviation}'
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    edition: 'Free'
    maxSizeBytes: '1048576'
    requestedServiceObjectiveName: 'Free'
  }
}
