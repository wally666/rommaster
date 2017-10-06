<#
    .SYNOPSIS
        Provision Azure Storage.
#>

function ProvisionStorageAccounts {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)
    
    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\storageAccount.json'

	foreach ($token in $tokens.AzureStorageAccounts)
	{
		Write-Verbose ("Provisioning Azure Storage Account: Name='$($token.TemplateParams.StorageAccountName)', Resource Group Name='$($token.ResourceGroupName)'...")
		if ($token.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}
		
		$deploymentStatus = New-AzureRmResourceGroupDeployment `
								-Name ((Get-ChildItem $templateFilePath).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
								-ResourceGroupName $token.ResourceGroupName `
								-TemplateFile $templateFilePath `
								-TemplateParameterObject $token.TemplateParams
						
		$deploymentStatus			

		$deploymentStatus.Outputs.keys | %{
			Populate-KeyVaultSecret $token.KeyVault.Name "$($token.KeyVault.KeyPrefix)-$_" $_ $deploymentStatus.Outputs.$_.Value
			$token["$_"] = $deploymentStatus.Outputs.$_.Value
		}
	}
}