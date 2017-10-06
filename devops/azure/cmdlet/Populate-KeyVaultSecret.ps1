<#
    .SYNOPSIS
        Populates Azure KeyVault secret.
#>

function Populate-KeyVaultSecret {
        param (
		[Parameter(Mandatory=$true)]
		$keyVaultName,
		[Parameter(Mandatory=$true)]
		$key,
		[Parameter(Mandatory=$false)]
		$type = "Unknown",
		[Parameter(Mandatory=$true)]
		$vault
	)

	Write-Verbose ("Populating Azure Key Vault: Name: '$keyVaultName', Type='$type', Key='$key'...")
	
    $secretvalue = ConvertTo-SecureString -String $vault -AsPlainText -Force
    Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name $key-SecretValue -ContentType $type -SecretValue $secretvalue
}