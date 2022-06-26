using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AppCenterBuildApp.Models
{
    /// <summary>
    /// Basic app info
    /// </summary>
    public class AppInfo
    {
        /// <summary>
        /// The unique ID(UUID) of the app
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the app used in URLs
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the owner
        /// </summary>
        public UserInfo Owner { get; set; }

    }

    /// <summary>
    /// Basic user info
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// The unique ID(UUID) of the user
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique name that used to identify the owner
        /// </summary>
        public string Name { get; set; }
    }
}
