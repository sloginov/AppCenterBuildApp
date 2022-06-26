using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AppCenterBuildApp.Models
{
    /// <summary>
    /// Build information
    /// </summary>
    public class BuildInfo
    {
        /// <summary>
        /// The build Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The build number
        /// </summary>
        public string BuildNumber { get; set; }

        /// <summary>
        /// The time the build was queued
        /// </summary>
        public DateTime QueueTime { get; set; }

        /// <summary>
        /// The time the build was started
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// The time the build was finished
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// The time the build status was last changed
        /// </summary>
        public DateTime? LastChangedDate { get; set; }

        /// <summary>
        /// The build status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The build result
        /// </summary>
        public string Result { get; set; }


        /// <summary>
        /// The source branch name
        /// </summary>
        public string SourceBranch { get; set; }

        /// <summary>
        /// The source SHA
        /// </summary>
        public string SourceVersion { get; set; }
    }

    /// <summary>
    /// Build state
    /// </summary>
    public enum BuildState
    {
        /// <summary>
        /// Undefined state
        /// </summary>
        None,
        /// <summary>
        /// Build not started
        /// </summary>
        NotStarted,
        /// <summary>
        /// /Build is in progress
        /// </summary>
        InProgress,
        /// <summary>
        /// Build is cancelling
        /// </summary>
        Cancelling,
        /// <summary>
        /// Build is completed successfuly
        /// </summary>
        Succeded,
        /// <summary>
        /// Build is cancelled by user
        /// </summary>
        Cancelled,
        /// <summary>
        /// Build is failed with error or cancelled
        /// </summary>
        Failed
    }
}
