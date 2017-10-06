<#
    .SYNOPSIS
        Provisions Event Hub.
#>

function ProvisionEventHubs {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)
    
    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\eventHubs.json'

	foreach ($token in $tokens.AzureEventHubs)
	{
		Write-Verbose ("Provisioning Azure Event Hubs: Name='$($token.TemplateParams.EventHubName)', Resource Group Name='$($token.ResourceGroupName)'...")
		
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
			$token["$_"] = $deploymentStatus.Outputs.$_.Value
		}
	}
}