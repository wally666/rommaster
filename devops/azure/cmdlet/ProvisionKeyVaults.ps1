<#
    .SYNOPSIS
        Provisions Azure Key Vaults.
#>
function ProvisionKeyVaults {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\keyVault.json'
	
	foreach ($token in $tokens.AzureKeyVaults)
	{
		Write-Verbose ("Provisioning Azure Key Vault: Name='$($token.TemplateParams.KeyVaultName)', Resource Group Name='$($token.ResourceGroupName)'...")
		
		if ($token.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}

		#TODO: set objectId in proper way
		#$objectId = (Get-AzureRMADUser | Where {$_.Type -eq 'User'})[1].Id
		$account = (Get-AzureRmContext).Account -replace "@", "_"
		$users = Get-AzureRMADUser | Where {$_.UserPrincipalName -match "^$account"}
		$objectId = $users[0].Id
		
		$token.TemplateParams.clientObjectId = $objectId
		$token.TemplateParams.supervisorObjectId = $objectId
		
		$deploymentStatus = New-AzureRmResourceGroupDeployment `
								-Name ((Get-ChildItem $templateFilePath).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
								-ResourceGroupName $token.ResourceGroupName `
								-TemplateFile $templateFilePath `
								-TemplateParameterObject $token.TemplateParams `
								-Force `
								-Verbose

		$deploymentStatus
		
		$deploymentStatus.Outputs.keys | %{
			$token["$_"] = $deploymentStatus.Outputs.$_.Value
		}		
		
		# TODO: fix it - doesn't work on my subscription!
		# Grant access to current user
        # $context = Get-AzureRmContext
        # $deploymentStatus = Set-AzureRmKeyVaultAccessPolicy -VaultName $token.TemplateParams.KeyVaultName -UserPrincipalName $context.Account -PermissionsToSecrets All
		# $deploymentStatus
				
		Write-Verbose ("Populating secrets...")
		
		foreach ($secret in $token.Secrets)
		{
			Populate-KeyVaultSecret $token.TemplateParams.KeyVaultName $secret.Key 'Secret' $secret.Value
		}
	}
}