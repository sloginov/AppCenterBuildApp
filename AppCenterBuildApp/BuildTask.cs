using AppCenterBuildApp.API;
using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AppCenterBuildApp
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class BuildTask
    {
        private readonly AppCenterApiClient client;
        /// <summary>
        /// Build task options
        /// </summary>
        public BuildTaskOptions Options { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">App Center API client instance</param>
        /// <param name="options">Build task options</param>
        public BuildTask(AppCenterApiClient client, BuildTaskOptions options)
        {
            this.client = client;
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            Options = options;
        }

        /// <summary>
        /// Perform new build
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BuildInfo> BuildAsync(CancellationToken? cancellationToken = null)
        {
            var appOwnerName = Options.OwnerName;
            var targetAppInfo = Options.AppName;
            var targetBranchName = Options.BranchName;

            var buildInfo = await client.CreateBuildAsync(appOwnerName, targetAppInfo, targetBranchName);
            var buildId = buildInfo.Id;
            DateTime? firstApiErrorTime = null;

            while (true)
            {
                if (buildInfo.Status == BuildStatus.Completed)
                {
                    return buildInfo;
                }

                if (cancellationToken?.IsCancellationRequested == true && buildInfo.Status != BuildStatus.Cancelling)
                {
                    //Request build cancel and wait for cancellation finished
                    await client.CancelBuildAsync(appOwnerName, targetAppInfo, buildId);

                    //TODO: consider exiting method immediately with actual build info
                    //buildInfo = await client.GetBuildInfoAsync(appOwnerName, targetAppInfo, buildId);
                    //return buildInfo;
                }
                await Task.Delay(Options.UpdateBuildStatusPeriod);

                try
                {
                    buildInfo = await client.GetBuildInfoAsync(appOwnerName, targetAppInfo, buildId);
                    firstApiErrorTime = null;
                }
                catch (Exception ex)
                {
                    //TODO: log error instead of console
                    Console.WriteLine(ex.Message);
                    if (firstApiErrorTime == null)
                        firstApiErrorTime = DateTime.Now;

                    //throw error if timeout is elapsed
                    if (DateTime.Now - firstApiErrorTime.Value >= Options.AppCenterApiErrorTimeout)
                        throw ex;
                    Console.WriteLine("Retrying request build info...");
                }
            }
        }
    }

}
