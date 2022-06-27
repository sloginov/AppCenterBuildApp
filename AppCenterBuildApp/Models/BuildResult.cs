using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AppCenterBuildApp.Models
{
    /// <summary>
    /// App center build results
    /// </summary>
    public enum BuildResult
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Build succeeded
        /// </summary>
        Succeeded,
        /// <summary>
        /// Build cancelled by user
        /// </summary>
        Canceled,
        /// <summary>
        /// Build failed
        /// </summary>
        Failed
    }
}
