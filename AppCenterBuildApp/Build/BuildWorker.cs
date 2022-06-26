using AppCenterBuildApp.API;
using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AppCenterBuildApp.Build
{
    internal class BuildWorker : IDisposable
    {
        private readonly AppCenterApiClient client;
        private Timer timer = new Timer(3000);
        private BuildInfo currentBuildInfo;

        private bool building;
        /// <summary>
        /// Indicates whether build is running
        /// </summary>
        public bool IsBuilding => building;

        private string targetBranchName;
        /// <summary>
        /// Target branch name
        /// </summary>
        public string TargetBranchName => targetBranchName;

        private AppInfo targetAppInfo;
        /// <summary>
        /// Target app info
        /// </summary>
        public AppInfo TargetApp => targetAppInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="app"></param>
        /// <param name="branchName"></param>
        public BuildWorker(AppCenterApiClient client, AppInfo app, string branchName)
        {
            this.client = client;
            targetAppInfo = app;
            targetBranchName = branchName;
            timer.Elapsed += OnUpdateBuildInfoTimer;
            //TODO Add logger
        }

        /// <summary>
        /// Initiate new build task
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            if (building)
                await CancelAsync();

            building = true;
            var buildInfo = await client.CreateBuildAsync(targetAppInfo, targetBranchName);
            OnNewBuildInfoReceived(buildInfo);
            StateChanged?.Invoke(this, new BuildStateEventArgs(BuildState.None, currentBuildInfo));
            timer.Start();
        }

        /// <summary>
        /// Cancel current build, if it's running
        /// </summary>
        /// <returns></returns>
        public async Task CancelAsync()
        {
            if (!building || currentBuildInfo == null)
                return;
            await client.CancelBuildAsync(targetAppInfo, currentBuildInfo.Id);
            ResetCurrentBuildData();
        }


        /// <summary>
        /// Build state changed event
        /// </summary>
        public event BuildStateChangedEventHandler StateChanged;

        private void OnNewBuildInfoReceived(BuildInfo buildInfo)
        {
            BuildState prevBuildState = currentBuildInfo?.GetBuildState() ?? BuildState.None;
            BuildState newBuildState = buildInfo?.GetBuildState() ?? BuildState.None;
            if (prevBuildState == newBuildState)
                return;

            currentBuildInfo = buildInfo;
            StateChanged?.Invoke(this, new BuildStateEventArgs(prevBuildState, buildInfo));
        }

        private bool updatingBuildInfo = false;
        private async Task UpdateCurrentBuildInfo()
        {
            if (!building || currentBuildInfo == null || updatingBuildInfo)
                return;
            try
            {
                updatingBuildInfo = true;

                var buildInfo = await client.GetBuildInfoAsync(targetAppInfo, currentBuildInfo.Id);

                OnNewBuildInfoReceived(buildInfo);

                if (!string.IsNullOrEmpty(buildInfo.Result))
                    ResetCurrentBuildData();
            }
            catch (Exception)
            {
                //TODO add error to the logger
            }
            finally
            {
                updatingBuildInfo = false;
            }


        }

        private void ResetCurrentBuildData()
        {
            timer.Stop();
            building = false;
            currentBuildInfo = null;
        }

        private async void OnUpdateBuildInfoTimer(object sender, ElapsedEventArgs e)
        {
            await UpdateCurrentBuildInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ResetCurrentBuildData();
        }
    }

    /// <summary>
    /// Build state changed delegate
    /// </summary>
    /// <param name="sender">Build worker</param>
    /// <param name="args">Event args</param>
    internal delegate void BuildStateChangedEventHandler(BuildWorker sender, BuildStateEventArgs args);

    internal class BuildStateEventArgs : EventArgs
    {
        /// <summary>
        /// Previous state of build
        /// </summary>
        public BuildState PrevState { get; private set; }
        /// <summary>
        /// Current build info
        /// </summary>
        public BuildInfo BuildInfo { get; private set; }
        /// <summary>
        /// New state of build
        /// </summary>
        public BuildState NewState => BuildInfo.GetBuildState();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevState"></param>
        /// <param name="newState"></param>
        public BuildStateEventArgs(BuildState prevState, BuildInfo newBuildInfo)
        {
            PrevState = prevState;
            BuildInfo = newBuildInfo;
        }
    }
}
