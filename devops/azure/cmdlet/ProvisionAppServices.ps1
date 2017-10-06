<#
    .SYNOPSIS
        Provisions Azure App Services.
#>

function ProvisionAppServices {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\appService.json'
	
	foreach ($token in $tokens.AzureAppServices)
	{
		Write-Verbose ("Provisioning Azure App Service: Name='$($token.TemplateParams.AppServiceName)', Resource Group Name='$($token.ResourceGroupName)'...")
		
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

		$outputs = @{
			PublishingUserName = $deploymentStatus.Outputs.publishingUsername.Value
			PublishingPassword = $deploymentStatus.Outputs.publishingPassword.Value
		}

		$outputs
		
		$deploymentStatus.Outputs.keys | %{
			$token["$_"] = $deploymentStatus.Outputs.$_.Value
		}
		
		if ($token.AlwaysOn -eq 'true') {
			Write-Warning "Always On..."
		
			# $tokens.AzureStorageAccounts.Where(a => a.Skip != 'true' && a.TemplateParams.StorageAccountName == $token.AzureWebJobsDashboardStorageAccountName)
			$azureStorageAccount = $tokens.AzureStorageAccounts[0]
			$azureWebJobsDashboard = $azureStorageAccount.connectionString
		
			$configObject = @{ 
				siteConfig = @{ 
					AlwaysOn = $true
					AppSettings = @(
						@{ 
							Name = "AzureWebJobsDashboard"
							Value = $azureWebJobsDashboard 
						}
					)
				}
			}
			
			$deploymentStatus = Set-AzureRmResource `
				-ResourceGroupName $token.ResourceGroupName `
				-ResourceType Microsoft.Web/sites -Name $token.TemplateParams.AppServiceName `
				-PropertyObject $configObject `
				-Force `
				-Verbose
				
			$deploymentStatus
		}
	}
}