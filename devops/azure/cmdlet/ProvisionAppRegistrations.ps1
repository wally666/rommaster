<#
    .SYNOPSIS
        Provisions App Registrations.
#>
function ProvisionAppRegistrations {
        param (
		[Parameter(Mandatory=$true)]
		$tokens
	)
	
    $aadApp = Get-AzureRmADApplication -IdentifierUri $tokens.AzureCommon.ApplicationHomePageUlr
	$aadApp
	
	if(-not $aadApp) {
            $aadApp = New-AzureRmADApplication `
                -DisplayName $tokens.AzureCommon.ApplicationDisplayName `
                -HomePage $tokens.AzureCommon.ApplicationHomePageUlr `
                -IdentifierUris @($tokens.AzureCommon.ApplicationHomePageUlr) `
                -ReplyUrls @($tokens.AzureCommon.ApplicationHomePageUlr) `

            $servicePrincipal = New-AzureRmADServicePrincipal -ApplicationId $aadApp.ApplicationId
			$servicePrincipal
			
            $cred = New-AzureRmADAppCredential -ObjectId $aadApp.ObjectId -Password $tokens.AzureCommon.ApplicationClientSecret
			$cred
	}

    $spn = Get-AzureRmADServicePrincipal -ServicePrincipalName $aadApp.ApplicationId
	$spn
	
	Write-Verbose "ObjectId: $($aadApp.ObjectId)"
    Write-Verbose "ApplicationId: $($aadApp.ApplicationId)"
    Write-Verbose "SPObjectId: $($spn.Id)"
	
	$tokens.AzureCommon.applicationId = $aadApp.ApplicationId

	foreach ($token in $tokens.AzureKeyVaults)
	{
		Write-Verbose ("Provisioning Azure Key Vault Access Policy: Name='$($token.TemplateParams.KeyVaultName)', Resource Group Name='$($token.ResourceGroupName)'...")
		
		if ($token.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}
		
		$ap = Set-AzureRmKeyVaultAccessPolicy -VaultName $token.TemplateParams.KeyVaultName -ObjectId $spn.Id -PermissionsToSecrets get
		$ap
	}
}