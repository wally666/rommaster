<#
    .SYNOPSIS
        Provisions Azure App Service Plans.
#>

function ProvisionAppServicePlans {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\appServicePlans.json'
	
	foreach ($_ in $tokens.AzureAppServicePlans)
	{
		Write-Verbose ("Provisioning Azure App Service Plan: Name='$($_.TemplateParams.AppServicePlanName)', Resource Group Name='$($_.ResourceGroupName)'...")
		
		if ($_.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}

		$deploymentStatus = New-AzureRmResourceGroupDeployment `
								-Name ((Get-ChildItem $templateFilePath).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
								-ResourceGroupName $_.ResourceGroupName `
								-TemplateFile $templateFilePath `
								-TemplateParameterObject $_.TemplateParams `
								-Force `
								-Verbose

		$deploymentStatus
	}
}