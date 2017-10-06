<#
    .SYNOPSIS
        Provisions Azure SQL Servers.
#>
function ProvisionSqlServers {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\sqlServer.json'
	
	foreach ($token in $tokens.AzureSqlServers)
	{
		Write-Verbose ("Provisioning Azure SQL Server: Name='$($token.TemplateParams.SqlServerName)', Resource Group Name='$($token.ResourceGroupName)'...")
		
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
	}
}