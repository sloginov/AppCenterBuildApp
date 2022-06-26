using System;
using System.Collections.Generic;
using System.Text;

namespace AppCenterBuildApp.Build
{
    /// <summary>
    /// Provides input data for build application via App Center
    /// </summary>
    internal interface IBuildInputDataProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AppCenterBuildInputData GetAppCenterBuildInputData();
    }

    internal class AppCenterBuildInputData
    {
        internal AppCenterBuildInputData(string apiToken, string targetAppName)
        {
            ApiToken = apiToken;
            TargetAppName = targetAppName;
        }

        /// <summary>
        /// Token for access to App Center API
        /// </summary>
        internal string ApiToken { get; private set; }
        /// <summary>
        /// Target app name for build
        /// </summary>
        internal string TargetAppName { get; private set; }
    }
}
