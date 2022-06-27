using System;
using System.Text.Json.Serialization;

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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BuildStatus Status { get; set; }

        /// <summary>
        /// The build result
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BuildResult Result { get; set; }


        /// <summary>
        /// The source branch name
        /// </summary>
        public string SourceBranch { get; set; }

        /// <summary>
        /// The source SHA
        /// </summary>
        public string SourceVersion { get; set; }
    }
}
