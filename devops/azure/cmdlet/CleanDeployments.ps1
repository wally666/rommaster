<#
    .SYNOPSIS
        Clean all deployments for current env.
#>
function CleanDeployments {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

	foreach ($token in $tokens.AzureResourceGroups)
	{
		Write-Verbose ("Cleaning Azure Resource Group Deployments: Name='$($token.Name)', Location='$($token.Location)'...")

		if ($token.SkipCleaning -eq 'true') {
			Write-Warning "Cleaning Skipped."
			continue;
		}		
				
		$deployments = Get-AzureRmResourceGroupDeployment -ResourceGroupName $token.Name -ErrorAction SilentlyContinue
		$deploymentsToDelete = $deployments | where { $_.Timestamp -lt ((get-date).AddDays(-7)) }

		$deploymentsToDelete
		$profilePath = $Tokens.Credentials.ProfilePath
		
		foreach ($deployment in $deploymentsToDelete) {
			# Remove-AzureRmResourceGroupDeployment -ResourceGroupName $deployment.ResourceGroupName -DeploymentName $deployment.DeploymentName
			
			Start-Job -ScriptBlock {
				param($resourceGroupName, $deploymentName, $profilePath)
				Select-AzureRmProfile -Path $profilePath
				Remove-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -DeploymentName $deploymentName
			} -ArgumentList @($deployment.ResourceGroupName, $deployment.DeploymentName, $profilePath) | Out-Null
		}
				
		# (Get-Job -State Running).Length
		
		# Waits on all jobs to complete
		# Get-Job | Wait-Job

		# Removes the completed one.
		# Get-Job -State Completed | Remove-Job

		# Output results of all jobs
		# Get-Job | Receive-Job

		# Cleanup
		# Get-Job | Remove-Job
	}
}
