using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCenterBuildApp
{
    internal class BuildTaskOptions
    {
        /// <summary>
        /// Target app owner name
        /// </summary>
        public string OwnerName { get; }
        /// <summary>
        /// Target app name
        /// </summary>
        public string AppName { get; }
        /// <summary>
        /// Target branch name
        /// </summary>
        public string BranchName { get; }
        /// <summary>
        /// Timeout if api errors occurred
        /// </summary>
        public TimeSpan AppCenterApiErrorTimeout { get; set; } = TimeSpan.FromSeconds(60);

        private TimeSpan updateBuildStatusPeriod = TimeSpan.FromSeconds(3);
        /// <summary>
        /// The period for updating build status. Minimum values is 500 ms. Default is 3000 ms.
        /// </summary>
        public TimeSpan UpdateBuildStatusPeriod
        {
            get => updateBuildStatusPeriod;
            set
            {
                updateBuildStatusPeriod = value;
                var minPeriod = TimeSpan.FromMilliseconds(500);
                if (updateBuildStatusPeriod < minPeriod)
                    AppCenterApiErrorTimeout = minPeriod;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="branchName"></param>
        public BuildTaskOptions(string ownerName, string appName, string branchName)
        {
            if (string.IsNullOrWhiteSpace(ownerName))
                throw new ArgumentNullException(nameof(ownerName));
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentNullException(nameof(appName));
            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentNullException(nameof(branchName));
            AppName = appName;
            OwnerName = ownerName;
            BranchName = branchName;
        }
    }
}
