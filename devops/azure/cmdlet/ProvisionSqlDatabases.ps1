<#
    .SYNOPSIS
        Provisions Azure SQL Databases.
#>
function ProvisionSqlDatabases {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\sqlDatabase.json'

	foreach ($token in $tokens.AzureSqlDatabases)
	{
		Write-Verbose ("Provisioning Azure SQL Database: Name='$($token.TemplateParams.SqlDatabaseName)', Resource Group Name='$($token.ResourceGroupName)'...")

		if ($token.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}

		$deploymentStatus = New-AzureRmResourceGroupDeployment `
								-Name ((Get-ChildItem $templateFilePath).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
								-ResourceGroupName $token.ResourceGroupName `
								-TemplateFile $templateFilePath `
								-TemplateParameterObject $token.TemplateParams `
								-Force `
								-Verbose

		$deploymentStatus
		
		$deploymentStatus.Outputs.keys | %{
			Populate-KeyVaultSecret $token.KeyVault.Name "$($token.KeyVault.KeyPrefix)-$_" $_ $deploymentStatus.Outputs.$_.Value
			$token["$_"] = $deploymentStatus.Outputs.$_.Value
		}
						
		if ($token.SkipCreatingFirewallRule -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}
		
		$ip = Invoke-RestMethod http://ipinfo.io/json | Select -exp ip
		$startip = $ip
		$endip = $ip
		
		try {
		$deploymentStatus = Remove-AzureRmSqlServerFirewallRule `
			-FirewallRuleName $token.FirewallRuleName `
			-ResourceGroupName $token.ResourceGroupName `
			-ServerName $token.TemplateParams.SqlServerName `
			-WarningAction Continue `
			-Force `
			-ErrorAction Continue
		}
		catch {
			Write-Verbose ("Problem with removing Firewall Rule")
		}
		
		$deploymentStatus
		
		$deploymentStatus = New-AzureRmSqlServerFirewallRule `
			-ResourceGroupName $token.ResourceGroupName `
			-ServerName $token.TemplateParams.SqlServerName `
			-FirewallRuleName $token.FirewallRuleName `
			-StartIpAddress $startip -EndIpAddress $endip `
			-Verbose
		
		$deploymentStatus
	}
}