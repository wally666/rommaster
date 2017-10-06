#addin "Cake.Powershell"
#addin "MagicChunks"
#addin "nuget:?package=Cake.SqlServer"
#tool "nuget:?package=xunit.runner.console"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var environment = Argument<string>("environment", "dev");
var adminSqlDatabasePassword = Argument<string>("adminSqlDatabasePassword", null);

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles("./../src/**/*.sln");
var solutionPaths = solutions.Select(solution => solution.GetDirectory());
var projects = GetFiles("./../src/**/*.csproj");
var outputDir = MakeAbsolute(Directory("./../artifacts/"));
object tokens = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup((context) =>
{
    // Executed BEFORE the first task.
	Information("OutDir {0}", outputDir);
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in solutionPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
	
	CleanDirectories(outputDir.FullPath);
});

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
        NuGetRestore(solution);
    }
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);
        MSBuild(solution, settings =>
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                .WithProperty("TreatWarningsAsErrors","true")
				.WithProperty("OutDir", outputDir.FullPath)
                .WithTarget("Build")
                .SetConfiguration(configuration));
    }
});

Task("Provision")
    .Description("Azure resources provisioning")
    .Does(() =>
{
	if(Environment.UserInteractive && string.IsNullOrEmpty(adminSqlDatabasePassword))
	{
	   Console.WriteLine("Enter new Azure SQL Database admin password:");
	   adminSqlDatabasePassword = Console.ReadLine();
	}
	
    var results = StartPowershellFile("azure/provision.ps1", new PowershellSettings()
		//.SetFormatOutput()
		.SetLogOutput()
		.WithArguments(args =>
			{
				args.Append("Environment", environment)
					.Append("-Verbose")
					.Append("ErrorAction", "Stop")
					//.AppendSecret("Password", adminSqlDatabasePassword)
					.Append("Password", adminSqlDatabasePassword)
					;
			}));
					
	tokens = results.Last();

	var returnCode = int.Parse(results[0].BaseObject.ToString());
    if (returnCode != 0) 
	{
         throw new ApplicationException("Script failed to execute");
    }
});

Task("TransformConfig")
    .Does(() => {
		
		var path = outputDir.FullPath + @"/appsettings.json";
		var target = outputDir.FullPath + @"/appsettings.json";
		var transformations = new TransformationCollection {
            { "azure/keyVaultBaseUrl", (string)getResultData(tokens, "AzureKeyVaults[0].vaultBaseUrl") },
            { "azure/clientId", getResultData(tokens, "AzureCommon.ApplicationId").ToString() },
			{ "azure/clientSecret", getResultData(tokens, "AzureCommon.ApplicationClientSecret").ToString() }
          };
		
        TransformConfig(path, target, transformations);
    });
	
Task("Publish")
	.IsDependentOn("Build")
    .IsDependentOn("Provision")
	.IsDependentOn("TransformConfig")
    .Does(() =>
{
	var connString = (string)getResultData(tokens, "AzureSqlDatabases[0].connectionString");
    var dbName = (string)getResultData(tokens, "AzureSqlDatabases[0].TemplateParams.SqlDatabaseName");
	var file = new FilePath(outputDir.FullPath + @"\RomMaster.Server.Database.dacpac");
    var settings = new PublishDacpacSettings { 
		GenerateDeploymentScript = true
	};

    PublishDacpacFile(connString, dbName, file, settings);
});

Func<object, string, object> getResultData = (result, path) => {
	var properties = path.Split('.');
	var property = properties.First().Split('[').First();
	int? index = null;

	foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(properties.First(), @"\[(?<INDEX>\d+)\]"))
	{
		if (match.Success)
		{
			index = int.Parse(match.Groups["INDEX"].Value);
		}

		break;
	}

	object value = null;
	if (result is PSObject)
	{
		value = ((PSObject)result).Properties[property].Value;
	} 
	else if (result is System.Collections.Hashtable)
	{
		value = ((System.Collections.Hashtable)result)[property];
	}

	if (value is System.Management.Automation.PSObject)
	{
		value = ((System.Management.Automation.PSObject)value).BaseObject;
	}

	if (value is object[] && index.HasValue)
	{
		value = ((object[])value)[index.Value];
	}

	if (properties.Count() == 1)
	{
		return value;
	}
	
	return getResultData(value, string.Join(".", properties.Skip(1)));
};

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("This is the default task which will be ran if no specific target is passed in.")
    .IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);