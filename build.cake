///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var artifactsDir = Directory("./artifacts");

FilePath        ngPath      = Context.Tools.Resolve("ng.cmd");
FilePath        yarnPath     = Context.Tools.Resolve("yarn.cmd");
DirectoryPath   outputPath  = MakeAbsolute( artifactsDir + Directory("wwwroot"));
DirectoryPath   uiPath  = MakeAbsolute(Directory("./src/Sherlock.Ng/"));
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
// Helpers
///////////////////////////////////////////////////////////////////////////////
Action<FilePath, ProcessArgumentBuilder, DirectoryPath> ExecuteInDirectory => (path, args, directory) => {
    var result = StartProcess(
        path,
        new ProcessSettings {
            Arguments = args,
            WorkingDirectory = directory
        });

    if(0 != result)
    {
        throw new Exception($"Failed to execute tool {path.GetFilename()} ({result})");
    }
};

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("yarn::install")
    .Does( ()=> {
    ExecuteInDirectory(
        yarnPath,
        new ProcessArgumentBuilder().Append("install"),
        uiPath
    );
});

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
.IsDependentOn("yarn::install")
.Does(()=>{
    ExecuteInDirectory(
        ngPath,
        new ProcessArgumentBuilder()
            .Append("build")
            .Append("--prod")
            .Append("--build-optimizer")
            .Append("--output-path")
            .AppendQuoted(outputPath.FullPath),
        uiPath
    );
});

Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Ui::Publish")
    .IsDependentOn("Host::Publish")
.Does(() => {
   Information("Published on " + artifactsDir);
});

RunTarget(target);