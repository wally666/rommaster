function LoginToAzure {
    param (
		[Parameter(Mandatory=$true)]
		$Tokens
	)
	
    $profilePath = $Tokens.Credentials.ProfilePath 
    $subscriptionName = $Tokens.AzureCommon.SubscriptionName
	
	if(Test-Path -Path $profilePath)
	{
		$ctx = Import-AzureRmContext -Path $profilePath
		$ctx.Context.TokenCache.Deserialize($ctx.Context.TokenCache.CacheData)
	}
	else
	{
		Login-AzureRmAccount
		Save-AzureRmContext -Path $profilePath
	}

	Write-Verbose ("Selecting subscription... '{0}'" -f $subscriptionName)
    Get-AzureRmSubscription -SubscriptionName $subscriptionName | Set-AzureRmContext
}