<#
    .SYNOPSIS
        Provisions Azure Resource Group.
#>
function ProvisionResourceGroups {
    param (
		[Parameter(Mandatory=$true)]
		$tokens
	)

	foreach ($_ in $tokens.AzureResourceGroups)
	{
		Write-Verbose ("Provisioning Azure Resource Group: Name='$($_.Name)', Location='$($_.Location)'...")

		if ($_.Skip -eq 'true') {
			Write-Warning "Skipped."
			continue;
		}

        $tag = @{
            Name="owner";
            Value=$_.Owner
        }

        $deploymentStatus = New-AzureRmResourceGroup `
			-Name $_.Name `
			-Tag $tag `
			-Location $_.Location `
			-ErrorAction Stop `
			-Force `
			-Verbose
			
		$deploymentStatus
	}
}