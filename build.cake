#addin "Cake.Yarn"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var artifactsDir = Directory("./artifacts");
var uiPath  = MakeAbsolute(Directory("./src/Sherlock.Ng/"));

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
.Does(()=>{
     CleanDirectory(artifactsDir);
});

Task("Build")
.Does(()=>{
    DotNetCoreBuild("src/Sherlock.sln");
});

Task("Host::Publish")
.Does(() => {
    var settings = new DotNetCorePublishSettings
    {
        Configuration = "Release",
        OutputDirectory = artifactsDir
    }; 
   DotNetCorePublish("./src/Sherlock.Host/Sherlock.Host.csproj", settings);
});

Task("Ui::Publish")
.Does(()=>{
    Yarn.FromPath(uiPath).Install();
    Yarn.FromPath(uiPath).RunScript("prodbuild");
});

Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Ui::Publish")
    .IsDependentOn("Host::Publish")
.Does(() => {
   Information("Published on " + artifactsDir);
});

RunTarget(target);