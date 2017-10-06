param(
    [Parameter(Mandatory=$false)]
    [string[]]
    $Environment = 'dev',
	[Parameter(Mandatory=$true)]
	$Password
)

# Get-Module PowerShellGet -list | Select-Object Name,Version,Path
# Write-Verbose (Get-Module azure* -ListAvailable).Version

# Install the Azure Resource Manager modules from the PowerShell Gallery
# Install-Module AzureRM
# Install-Module AzureAD

#******************************************************************************
# Script body
# Execution begins here
#******************************************************************************
$ErrorActionPreference = "Stop"

. "$PSScriptRoot\cmdlet\common\Convert-PSObjectToHashtable.ps1"
. "$PSScriptRoot\cmdlet\common\Convert-HashtableToPSCustomObject.ps1"
. "$PSScriptRoot\cmdlet\Populate-KeyVaultSecret.ps1"

$tokensFilePath = '.\tokens.{0}.json' -f $Environment

Write-Verbose ("Loading tokens... '{0}'" -f $tokensFilePath)
$json = ConvertFrom-Json -InputObject (Gc $tokensFilePath -Raw)

$tokens = Convert-PSObjectToHashtable $json

foreach ($sqlServer in $tokens.AzureSqlServers) {
	$sqlServer.TemplateParams.administratorLoginPassword = $Password
}
foreach ($sqlDatabase in $tokens.AzureSqlDatabases) {
	$sqlDatabase.TemplateParams.administratorLoginPassword = $Password
}

$steps = @(
'LoginToAzure',
'CleanDeployments',
'ProvisionResourceGroups',
'ProvisionSendGridAccounts',
'ProvisionKeyVaults',
'ProvisionAppRegistrations',
'ProvisionStorageAccounts', 
'ProvisionEventHubs', 
'ProvisionAppInsights', 
'ProvisionDashboards', 
'ProvisionAppServicePlans', 
'ProvisionAppServices',
'ProvisionSqlServers',
'ProvisionSqlDatabases'
)

foreach ($step in $steps) {
		Write-Host "`n"
		Write-Verbose "Importing cmdlet: '$step'."
		. "$PSScriptRoot\cmdlet\$step.ps1"
        Write-Verbose "Executing: '$step'"
        . $step $tokens

		if ($LASTEXITCODE) {
			throw "Failed: $LASTEXITCODE"
		}
}

# Get-AzureRmADUser
# Get-AzureRmADServicePrincipal

return Convert-HashtableToPsCustomObject $tokens