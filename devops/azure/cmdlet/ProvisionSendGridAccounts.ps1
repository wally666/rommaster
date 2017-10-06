<#
    .SYNOPSIS
        Provision Azure Send Grid Accounts.
#>

function ProvisionSendGridAccounts {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)
    
    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\sendGridAccount.json'

	foreach ($token in $tokens.AzureSendGridAccounts)
	{
		Write-Verbose ("Provisioning Azure Send Grid Account: Name='$($token.TemplateParams.Name)', Resource Group Name='$($token.ResourceGroupName)'...")
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
	}
}