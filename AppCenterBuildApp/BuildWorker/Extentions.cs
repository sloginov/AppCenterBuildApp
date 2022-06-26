using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCenterBuildApp.BuildWorker
{
    public static class BuildInfoExtentions
    {
        /// <summary>
        /// Returns BuildState for build info
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <returns></returns>
        public static BuildState GetBuildState(this BuildInfo buildInfo)
        {
            string buildStatus = buildInfo.Status.ToLower();
            if (buildStatus == "notstarted")
                return BuildState.NotStarted;
            else if (buildStatus == "inprogress")
                return BuildState.InProgress;
            else if (buildStatus == "cancelling")
                return BuildState.Cancelling;
            else if (buildStatus == "completed")
            {
                var buildResult = buildInfo.Result.ToLower();
                if (buildResult == "succeded")
                    return BuildState.Succeded;
                else if (buildResult == "canceled")
                    return BuildState.Cancelled;
                else
                    return BuildState.Failed;
            }
            return BuildState.None;
        }

        /// <summary>
        /// Check if build state is completed
        /// </summary>
        /// <param name="buildState"></param>
        /// <returns></returns>
        public static bool IsCompletedState(this BuildState buildState)
        {
            return buildState == BuildState.Succeded || buildState == BuildState.Cancelled || buildState == BuildState.Failed;
        }
    }


}
