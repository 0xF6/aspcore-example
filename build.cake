#tool "nuget:?package=NUnit.ConsoleRunner"
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

Task("Clean")
    .Does(() =>
{
  CleanDirectory("./out");
  CleanDirectories($"./test/**/bin/{configuration}");
  CleanDirectories($"./test/**/obj");
  CleanDirectories("./WebApp/**/obj");
  CleanDirectories($"./WebApp/**/bin/{configuration}");
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreRestore("./WebApp.sln", new DotNetCoreRestoreSettings
    {
        Verbosity = DotNetCoreVerbosity.Minimal,
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    });
});
Task("Build")
    .Does(() =>
{
    // Build the solution.
    var path = MakeAbsolute(new DirectoryPath("./WebApp.sln"));
    DotNetCoreBuild(path.FullPath, new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        NoRestore = true
    });
});

Task("Copy-Files")
    .Does(() =>
{

    DotNetCorePublish("./WebApp/WebApp.csproj", new DotNetCorePublishSettings
      {
          NoRestore = true,
          Configuration = configuration,
          OutputDirectory = $"./out/webapp"
      });
    
});
Task("Test")
  .Does(() =>
  {
    var settings = new DotNetCoreTestSettings
    {
      Verbosity = DotNetCoreVerbosity.Minimal,
    };
    DotNetCoreTest("./test/unit.test/unit.test.csproj", settings);
  });
Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Files")
    .IsDependentOn("Test");
RunTarget(target);