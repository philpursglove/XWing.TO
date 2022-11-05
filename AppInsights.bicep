﻿param location string
param abbreviation string

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'law-xwingto${abbreviation}'
  location: location
  properties: {
    sku: {
      name: 'Free'
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
  }
}

output appInsightsId string = appInsightsComponents.id