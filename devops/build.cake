////////////////////////////////////
// INSTALL TOOLS
////////////////////////////////////
// #tool nuget:?package=xunit.runner.console&version=2.3.1
// #tool nuget:?package=OpenCover&version=4.6.519
#tool nuget:?package=xunit.runner.console
#tool nuget:?package=OpenCover

////////////////////////////////////
// INSTALL ADDINS
////////////////////////////////////
#addin "Cake.Powershell"
#addin nuget:?package=MagicChunks&version=2.0.0.119
#addin "Cake.WebDeploy"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var environment = Argument<string>("environment", "");
var user = Argument<string>("user", "");
var password = Argument<string>("password", "");
var connectionString = Argument<string>("connectionString", "");
var publishingUsername = Argument<string>("publishingUsername", "");
var publishingPassword = Argument<string>("publishingPassword", "");
var appInsightsInstrumentationKey = Argument<string>("appInsightsInstrumentationKey", "");
var buildNumber = Argument<string>("buildNumber", "");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var solutions = GetFiles("./../src/*.sln");
var outputDir = MakeAbsolute(Directory("./../artifacts/"));
//var deployDir = MakeAbsolute(Directory("./../"));
object tokens = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup((context) =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown((context) =>
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
	var solutionPaths = solutions.Select(solution => solution.GetDirectory());
	
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
				.WithProperty("DeleteExistingFiles", "true")
				.WithProperty("TreatWarningsAsErrors", "true")
				.WithProperty("OutDir", System.IO.Path.Combine(outputDir.FullPath, "bin"))
				.WithTarget("Build")
				.SetConfiguration(configuration));
    }
});

Task("Test")
	.Description("Executes xUnit tests.")
	.IsDependentOn("Build")
	.Does(() =>
{
	var projects = GetFiles("./../src/**/*.Tests.csproj");
	foreach(var project in projects)
    {
		Information("Testing {0}", project);
		DotNetCoreTest(project.FullPath);
	}
	
	return;
	
	OpenCover(
		tool => tool.XUnit2(
		System.IO.Path.Combine(outputDir.FullPath, "bin", "*.Tests.dll"),
		new XUnit2Settings
		{
			XmlReport = true,
			OutputDirectory = System.IO.Path.Combine(outputDir.FullPath, "bin"),
			ShadowCopy = false
		}),
		System.IO.Path.Combine(outputDir.FullPath, "bin", "Coverage.xml"),
		new OpenCoverSettings
		{ 
			ReturnTargetCodeOffset = 0
		}
			.WithFilter("+[RomMaster.*]*")
			.WithFilter("-[*Tests*]*")
		);
});

Task("Package")
    .Description("Publish web application to file system.")
    .IsDependentOn("Test")
    .Does(() =>
{
	/*
	var webCsproj = MakeAbsolute(File("./../src/HumanCapital.Web/HumanCapital.Web.csproj"));
		
	Information("Web publishing to file system {0}", webCsproj);
	MSBuild(webCsproj, settings =>
		settings.SetPlatformTarget(PlatformTarget.MSIL)
			.WithProperty("DeployOnBuild","True")
			.WithProperty("DeployDefaultTarget","WebPublish")
			.WithProperty("WebPublishMethod","FileSystem")				
			.WithProperty("DeleteExistingFiles","True")
			.WithProperty("PublishUrl", System.IO.Path.Combine(outputDir.FullPath, "bin", "_PublishedWebsites", "HumanCapital.Web_Package"))
			.SetConfiguration(configuration));
	*/
});

Task("Copy-DeployScripts")
    .Description("Builds package with deployment scripts")
	.IsDependentOn("Package")
    .Does(()=>
{
	/*
        var outputDeployDir = System.IO.Path.Combine(outputDir.FullPath, "deploy");
        CreateDirectory(outputDeployDir);
        CreateDirectory(System.IO.Path.Combine(outputDeployDir, "tools"));
		
        CopyDirectory("azure", System.IO.Path.Combine(outputDeployDir, "azure"));
		CopyFileToDirectory("bootstrapper.ps1", outputDeployDir);
		CopyFileToDirectory("build.cake", outputDeployDir);
		CopyFileToDirectory("deploy.cmd", outputDeployDir);
		CopyFileToDirectory("tools\\packages.config", System.IO.Path.Combine(outputDeployDir, "tools"));
	*/
 });

Task("Provision")
    .Description("Azure resources provisioning")
    .Does(() =>
{
    var results = StartPowershellFile("../../../devops/Azure/provision.ps1", new PowershellSettings()
		.SetLogOutput()
		.WithArguments(args =>
			{
				args.Append("Environment", $"\"{environment}\"");
				
				if(!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
				{
					args.Append("UserName", $"\"{user}\"")
						.Append("UserPassword", $"\"{password}\"");
				}
				
				if(!String.IsNullOrEmpty(connectionString))
				{
					args.Append("ConnectionString", $"\"{connectionString}\"");				
				}

				if(!String.IsNullOrEmpty(publishingUsername) && !String.IsNullOrEmpty(publishingPassword))
				{
					args.Append("PublishingUsername", $"\"{publishingUsername}\"")
						.Append("PublishingPassword", $"\"{publishingPassword}\"");
					
				}
				
				if(!String.IsNullOrEmpty(appInsightsInstrumentationKey))
				{
					args.Append("AppInsightsInstrumentationKey", $"\"{appInsightsInstrumentationKey}\"");				
				}				
				
				args.Append("-Verbose")
					.Append("ErrorAction", "Stop");
			}));
					
	tokens = results.Last();

	var returnCode = int.Parse(results[0].BaseObject.ToString());
    if (returnCode != 0) 
	{
         throw new ApplicationException("Script failed to execute");
    }
});

Task("TransformConfig")
    .Description("Transform web.config")
	.IsDependentOn("Provision")
    .Does(() => 
{	
	// Transform web.config	
	var path = System.IO.Path.Combine(outputDir.FullPath, "bin");
	var config = System.IO.Path.Combine(path, "appsettings.json");
		
	var transformations = new TransformationCollection {
            { "azure/keyVaultBaseUrl", (string)getResultData(tokens, "AzureKeyVaults[0].vaultBaseUrl") },
			{ "azure/clientId", getResultData(tokens, "Credentials.applicationId").ToString() },
			{ "azure/clientSecret", (string)getResultData(tokens, "Credentials.ApplicationClientSecret") }
          };
		
        TransformConfig(config, config, transformations);
});

Task("Publish")
	.IsDependentOn("TransformConfig")
    .Does(() =>
{
	/*
	var connString = (string)getResultData(tokens, "AzureSqlDatabases[0].connectionString");
	var file = new FilePath(outputDir.FullPath + @"\RomMaster.Server.SqlDatabase.dacpac");

	SqlPackagePublish(settings => 
    {
        settings.SourceFile = file;
		settings.TargetConnectionString = connString;
    });
	*/
                              

		var siteName = getResultData(tokens, "AzureAppServices.TemplateParams.AppServiceName").ToString();
		var userName = getResultData(tokens, "AzureAppServices.publishingUsername").ToString();
		
		if(!userName.StartsWith("$"))
		{
			userName = $"${userName}";
		}
		
		DeployWebsite(new DeploySettings()
			.FromSourcePath(System.IO.Path.Combine(outputDir.FullPath, "bin", "_PublishedWebsites", "HumanCapital.Web_Package"))
			.UseSiteName(siteName)
			.UseComputerName("https://" + siteName + ".scm.azurewebsites.net:443/msdeploy.axd?site=" + siteName)
			.UseUsername(userName)
			.UsePassword(getResultData(tokens, "AzureAppServices.publishingPassword").ToString())
			.SetDelete(false)
			);
});

Task("AfterPublish")
	.IsDependentOn("Publish")
    .Does(() =>
{
/*
    var results = StartPowershellFile(System.IO.Path.Combine(outputDir.FullPath, "deploy", "azure", "cmdlet", "Execute-KuduCommand.ps1"), new PowershellSettings()
		.SetLogOutput()
		.WithArguments(args =>
			{
				args.Append("Sitename", getResultData(tokens, "AzureAppServices.TemplateParams.AppServiceName").ToString())
					.Append("Password", getResultData(tokens, "AzureAppServices.publishingPassword").ToString())
					.Append("Command", $"\"{getResultData(tokens, "KuduConsole.Command").ToString()}\"")
					.Append("Directory", getResultData(tokens, "KuduConsole.Directory").ToString());
			}));

	var returnCode = int.Parse(results[0].BaseObject.ToString());
    if (returnCode != 0) 
	{
         throw new ApplicationException("Script failed to execute");
    }
	*/
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
    .IsDependentOn("BuildAll");
	
Task("BuildAll")
    .Description("Build umbraco and mocks solutions")
    .IsDependentOn("Copy-DeployScripts");
	
Task("DeployAll")
    .Description("Deploy umbraco and mocks on Azure Cloud.")
    .IsDependentOn("AfterPublish");
	
///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////
RunTarget(target);