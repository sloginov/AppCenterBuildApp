using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppCenterBuildApp.Build
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

        private string ReadParameter(JObject jsonObject, string parameterName)
        {
            string parameterValue = jsonObject.GetValue(parameterName, StringComparison.InvariantCultureIgnoreCase)?.Value<string>();
            if (string.IsNullOrWhiteSpace(parameterValue))
                throw new InputDataParameterNotFoundException(parameterName);
            return parameterValue;
        }

        public AppCenterBuildInputData GetAppCenterBuildInputData()
        {
            using (StreamReader file = File.OpenText(inputFileName))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                string apiToken = ReadParameter(jsonObject, "ApiToken");
                string targetApp = ReadParameter(jsonObject, "TargetApp");

                return new AppCenterBuildInputData(apiToken, targetApp);
            }
        }
    }

    internal class InputDataParameterNotFoundException : Exception
    {
        internal InputDataParameterNotFoundException(string parameterName) : base($"Input data must contain parameter '{parameterName}'!") { }
    }
}
