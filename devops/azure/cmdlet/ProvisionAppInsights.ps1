<#
    .SYNOPSIS
        Provision Azure App Insights.
#>
function ProvisionAppInsights {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)
    
    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\appInsights.json'
    
	foreach ($token in $tokens.AzureAppInsights)
	{
		Write-Verbose ("Provisioning Azure App Insights: Name='$($token.TemplateParams.AppInsightName)', Resource Group Name='$($token.ResourceGroupName)'...")
		if ($token.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}

		$deploymentStatus = New-AzureRmResourceGroupDeployment `
								-Name ((Get-ChildItem $templateFilePath).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
								-ResourceGroupName $token.ResourceGroupName `
								-TemplateFile $templateFilePath `
								-TemplateParameterObject $token.TemplateParams `
								-Verbose
		
		$deploymentStatus
		
		$deploymentStatus.Outputs.keys | %{
			Populate-KeyVaultSecret $token.KeyVault.Name "$($token.KeyVault.KeyPrefix)-$_" $_ $deploymentStatus.Outputs.$_.Value
		}
	}
}