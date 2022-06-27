using AppCenterBuildApp.API;
using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppCenterBuildApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string inputFileName = "inputData.json";
                if (args.Length > 0 && File.Exists(args[0]))
                    inputFileName = args[0];

                if (!File.Exists(inputFileName))
                {
                    Console.WriteLine($"Input json file '{Path.GetFullPath(inputFileName)}' not found!");
                    return;
                }
                await StartBuildForAllAppBranches(new JsonFileBuildInputDataProvider(inputFileName));

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task StartBuildForAllAppBranches(IBuildInputDataProvider inputDataProvider)
        {
            var inputData = inputDataProvider.GetAppCenterBuildInputData();
            AppCenterApiClient client = new AppCenterApiClient(inputData.ApiToken);
            Console.WriteLine("Retrieving apps...");
            var appList = await client.GetAppsAsync();
            var app = appList.FirstOrDefault(app => string.Equals(app.Name, inputData.TargetAppName, StringComparison.InvariantCultureIgnoreCase));
            if (app == null)
            {
                Console.WriteLine($"Target app with name '{inputData.TargetAppName}' not found!");
                return;
            }

            string appName = app.Name;
            string ownerName = app.Owner.Name;

            Console.WriteLine($"Retrieving branch names for app '{appName}'...");

            var branches = await client.GetBranchListAsync(app);
            var brancheNames = branches.Select(branch => branch.Branch.Name).ToList();

            Console.WriteLine($"App '{appName}' branch names received: {Environment.NewLine}{string.Join(", ", brancheNames)}");
            Console.WriteLine();

            List<BuildTask> buildWorkers = new List<BuildTask>(from branchName in brancheNames select new BuildTask(client, new BuildTaskOptions(ownerName, appName, branchName)));

            foreach (var buildWorker in buildWorkers)
            {
                var branchName = buildWorker.Options.BranchName;
                Console.WriteLine($"{branchName} - starting build...");
                try
                {
                    var buildInfo = await buildWorker.BuildAsync();

                    //TODO: Ensure buildInfo.StartTime and buildInfo.FinishTime have values
                    TimeSpan buildDuration = buildInfo.FinishTime.Value - buildInfo.StartTime.Value;
                    string buildResultString = buildInfo.Result == BuildResult.Succeeded ? "completed" : "failed";
                    string buildLogLink = $"https://appcenter.ms/users/{buildWorker.Options.OwnerName}/apps/{buildWorker.Options.AppName}/build/branches/{buildWorker.Options.BranchName}/builds/{buildInfo.Id}";

                    Console.WriteLine();
                    Console.WriteLine($"{branchName} {buildResultString} in {buildDuration.TotalSeconds:F0} seconds. Link to build logs: {buildLogLink}");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during building branch {branchName} occurred: {ex.Message}");
                }
            }
        }
    }
}
