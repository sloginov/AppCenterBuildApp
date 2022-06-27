using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace AppCenterBuildApp
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class JsonFileBuildInputDataProvider : IBuildInputDataProvider
    {
        private string inputFileName;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inputFileName">Relative or absolute path to the file with build input data in JSON format</param>
        public JsonFileBuildInputDataProvider(string inputFileName)
        {
            this.inputFileName = inputFileName;
        }

        private string ReadParameter(JsonDocument jsonDocument, string parameterName)
        {
            try
            {
                return jsonDocument.RootElement.GetProperty(parameterName).GetString();
            }
            catch (Exception ex)
            {
                throw new InputDataParameterException(parameterName, ex);
            }
        }

        public AppCenterBuildInputData GetAppCenterBuildInputData()
        {
            using (Stream file = File.OpenRead(inputFileName))
            using (JsonDocument jsonDocument = JsonDocument.Parse(file))
            {
                string apiToken = ReadParameter(jsonDocument, "ApiToken");
                string targetApp = ReadParameter(jsonDocument, "TargetApp");
                return new AppCenterBuildInputData(apiToken, targetApp);
            }
        }
    }

}
