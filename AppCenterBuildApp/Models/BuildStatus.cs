using System;
using System.Text;
using System.Text.Json.Serialization;

namespace AppCenterBuildApp.Models
{
    /// <summary>
    /// Build status
    /// </summary>
    public enum BuildStatus
    {
        /// <summary>
        /// Build queued, but not started
        /// </summary>
        NotStarted,
        /// <summary>
        /// Build is in progress
        /// </summary>
        InProgress,
        /// <summary>
        /// Build cancellation is requested by user
        /// </summary>
        Cancelling,
        /// <summary>
        /// Build task is completed.
        /// </summary>
        Completed
    }
}
