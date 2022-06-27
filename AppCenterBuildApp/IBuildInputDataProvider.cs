using System;
using System.Collections.Generic;
using System.Text;

namespace AppCenterBuildApp
{
    /// <summary>
    /// Provides input data for build application via App Center
    /// </summary>
    internal interface IBuildInputDataProvider
    {
        /// <summary>
        /// Get input data for build
        /// </summary>
        /// <returns></returns>
        AppCenterBuildInputData GetAppCenterBuildInputData();
    }
}
