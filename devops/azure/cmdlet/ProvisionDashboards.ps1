<#
    .SYNOPSIS
        Provisions Azure Dashboards.
#>

function ProvisionDashboards {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)
    
    $templateFilePath = Join-Path -Path $PSScriptRoot -ChildPath '..\template\dashboard.json'

	foreach ($_ in $tokens.AzureDashboards)
	{
		Write-Verbose ("Provisioning Azure Dashboards: Name='$($_.TemplateParams.DashboardName)', Resource Group Name='$($_.ResourceGroupName)'...")
		
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
		
		# $outputs = @{
			# IngestEventHubConnectionString = $deploymentStatus.Outputs['namespaceConnectionString'].Value
			# IngestEventHubSharedAccessPolicyPrimaryKey = $deploymentStatus.Outputs['sharedAccessPolicyPrimaryKey'].Value
		# }

		# $outputs.keys | %{ $outputs.$_ }
		# Update-Outputs -Environment $Environment -CategoryName 'AzureEventHub' -Outputs $outputs
	}
}