using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace DollarEnv.Module
{
    [Cmdlet("Import", "DotEnvFile")]
    public class ImportDotEnvFileCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string DotEnvFile { get; set; }

        protected override void ProcessRecord()
        {
            var filePath = Path.GetFullPath(DotEnvFile);
            using var fileStream = new FileStream(filePath, FileMode.Open);

            var variables = FileParser.ParseVariables(fileStream);
            
            WriteVerbose($"Read {variables.Count} variables from '${filePath}'");

            foreach (var variable in variables)
            {
                WriteDebug($"Setting {variable.Key}");
                Environment.SetEnvironmentVariable(variable.Key, variable.Value);
            }
        }
    }
}
