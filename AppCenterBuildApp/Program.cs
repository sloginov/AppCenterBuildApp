using AppCenterBuildApp.API;
using AppCenterBuildApp.BuildWorker;
using AppCenterBuildApp.Models;
using Newtonsoft.Json;
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
            var app = appList.FirstOrDefault(app => app.Name == inputData.TargetAppName);
            if (app == null)
            {
                Console.WriteLine($"Target app with name '{inputData.TargetAppName}' not found!");
                return;
            }

            Console.WriteLine($"Retrieving branch names for app '{app.Name}'...");

            var branches = await client.GetBranchListAsync(app);
            var brancheNames = branches.Select(branch => branch.Branch.Name).ToList();

            Console.WriteLine($"App '{app.Name}' branch names received: {Environment.NewLine}{string.Join(", ", brancheNames)}");
            Console.WriteLine();

            List<BuildTask> buildWorkers = new List<BuildTask>(from branchName in brancheNames select new BuildTask(client, app, branchName));
            foreach (var buildWorker in buildWorkers)
            {
                buildWorker.StateChanged += (sender, args) =>
                {
                    if (args.NewState == BuildState.InProgress)
                        Console.WriteLine($"{buildWorker.TargetBranchName} - build started.");
                    
                    if (!args.NewState.IsCompletedState())
                        return;

                    var buildInfo = args.BuildInfo;

                    TimeSpan buildDuration = buildInfo.FinishTime.Value - buildInfo.StartTime.Value;
                    string buildResultString = args.NewState == BuildState.Succeded ? "completed" : "failed";
                    string buildLogLink = $"https://appcenter.ms/users/{sender.TargetApp.Owner.Name}/apps/{sender.TargetApp.Name}/build/branches/{sender.TargetBranchName}/builds/{buildInfo.Id}";
                    Console.WriteLine();
                    Console.WriteLine($"{sender.TargetBranchName} {buildResultString} in {buildDuration.TotalSeconds:F0} seconds. Link to build logs: {buildLogLink}");
                    Console.WriteLine();
                };
                await buildWorker.StartAsync();
                Console.WriteLine($"{buildWorker.TargetBranchName} - build queued.");
                break;
            }
        }
    }
}
