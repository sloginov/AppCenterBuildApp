using System;
using System.Collections.Generic;

namespace AppCenterBuildApp.Models
{
    /// <summary>
    /// The branch build status
    /// </summary>
    public class BranchStatus
    {
        /// <summary>
        /// Is branch build configured
        /// </summary>
        public bool Configured { get; set; }

        /// <summary>
        /// Is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Branch info
        /// </summary>
        public BranchInfo Branch { get; set; }

        /// <summary>
        /// Last build info
        /// </summary>
        public BuildInfo LastBuild { get; set; }
    }

    /// <summary>
    /// The branch build core properties
    /// </summary>
    public class BranchInfo
    {
        /// <summary>
        /// Branch name
        /// </summary>
        public string Name { get; set; }
    }

}
