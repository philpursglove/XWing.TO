param location string
param abbreviation string

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'law-xwingto${abbreviation}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}


resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-xwingto${abbreviation}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    RetentionInDays: 60
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
  }
}

output appInsightsId string = appInsightsComponents.id