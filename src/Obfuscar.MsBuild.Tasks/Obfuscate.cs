using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using Obfuscar.MsBuild;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Obfuscar.MsBuild.Tasks {

    public class Obfuscate : Microsoft.Build.Utilities.Task {

        public string Obfuscator_Path { get; set; } = string.Empty;
        public string Obfuscator_ConfigTemplate { get; set; } = string.Empty;
        public bool Obfuscator_ConfigTemplate_ProjectReferences_Append { get; set; } = true;

        public string Obfuscator_Targets { get; set; } = string.Empty;

        public string ConfigurationName { get; set; } = string.Empty;

        public string SolutionDir { get; set; } = string.Empty;
        public string SolutionFileName { get; set; } = string.Empty;
        public string SolutionName { get; set; } = string.Empty;

        public string ProjectDir { get; set; } = string.Empty;
        public string ProjectFileName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;

        public ITaskItem[] ProjectReferences { get; set; } = Array.Empty<ITaskItem>();

        public string TargetDir { get; set; } = string.Empty;
        public string TargetFileName { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;

        public bool Diagnostics_TempFolder_Delete { get; set; } = true;


        public override bool Execute() {
            var ret = true;

            var Args = this.GetArgs();

            Log.LogMessage(MessageImportance.High, $@"Obfuscate - v{InternalAssemblyInfo.AssemblyVersion} @ {InternalAssemblyInfo.AssemblyBuildDate}");
            Log.LogMessage(MessageImportance.High, $@"-----------");
            Log.LogMessage(MessageImportance.High, $@"Obfuscator                = {Args.Obfuscator_Path}");
            Log.LogMessage(MessageImportance.High, $@"Obfuscator_ConfigTemplate = {Args.Obfuscator_ConfigTemplate}");
            Log.LogMessage(MessageImportance.High, $@"ConfigurationName         = {Args.ConfigurationName}");
            Log.LogMessage(MessageImportance.High, $@"ProjectDir                = {Args.ProjectDir}");
            Log.LogMessage(MessageImportance.High, $@"TargetDir                 = {Args.TargetDir}");
            Log.LogMessage(MessageImportance.High, $@"TargetFileName            = {Args.TargetFileName}");

            if (!Args.Obfuscator_Targets.Contains(Args.ConfigurationName)) {
                Log.LogMessage(MessageImportance.High, $@"'{Args.ConfigurationName}' is not one of '{Args.Obfuscator_Targets.JoinComma()}'.  Not Obfuscating.");
                return ret;
            }


            var ConfigTemplate = Resources.Defaults.ResourcePackage.Obfuscar;
            if (System.IO.File.Exists(Obfuscator_ConfigTemplate)) {
                ConfigTemplate = System.IO.File.ReadAllText(Obfuscator_ConfigTemplate)
                ;
            } else {
                Log.LogMessage(MessageImportance.High, $@"Path to {ObfuscateArgNames.Default.Obfuscator_ConfigTemplate} does not exist.  Using a default template.");
            }

            var Config = ConfigTemplate.Replace(Args);

            Log.LogMessage(MessageImportance.High, $@"Creating {Args.OutPathConfig}:");
            Log.LogMessage(MessageImportance.High, $@"FROM TEMPLATE:");
            Log.LogMessage(MessageImportance.High, ConfigTemplate);
            Log.LogMessage(MessageImportance.High, $@"NEW CONTENT:");
            Log.LogMessage(MessageImportance.High, Config);


            var CreateDirectoryTask = new MakeDir() {
                Directories = Args.OutPath.ToTaskList(),
            }.Initialize(this);
            var CreateDirectoryResult = CreateDirectoryTask.Execute();

            System.IO.File.WriteAllText(Args.OutPathConfig, Config);

            var ExecObfuscatorTask = new Exec() {
                Command = $@"""{Args.Obfuscator_Path}"" ""{Args.OutPathConfig}""",
            }.Initialize(this);
            var ExecObfuscatorResult = ExecObfuscatorTask.Execute();


            var FilesToCopyTask = new CreateItem() {
                Include = $@"{Args.OutPath}\*.*".ToTaskList(),
            }.Initialize(this);
            var FilesToCopyResult = FilesToCopyTask.Execute();
            var FilesToCopy = FilesToCopyTask.Include;


            var MoveResult = false;
            var MoveAttempts = 0;
            var MoveAttemptsMax = 10;

            while (MoveResult == false && MoveAttempts < MoveAttemptsMax) {
                MoveAttempts += 1;

                var MoveTask = new Copy() {
                    SourceFiles = FilesToCopy,
                    DestinationFolder = Args.InPath.ToTaskItem(),
                    OverwriteReadOnlyFiles = true,
                }.Initialize(this);

                MoveResult |= MoveTask.Execute();

                if (!MoveResult) {
                    Thread.Sleep(1000);
                }

            }

            bool DeleteDirectoryResult;
            if (!this.Diagnostics_TempFolder_Delete) {
                DeleteDirectoryResult = true;
            } else {
                var DeleteDirectoryTask = new RemoveDir() {
                    Directories = Args.OutPath.ToTaskList(),
                }.Initialize(this);
                DeleteDirectoryResult = DeleteDirectoryTask.Execute();
            }

            ret = ret
                && CreateDirectoryResult
                && ExecObfuscatorResult
                && FilesToCopyResult
                && MoveResult
                && DeleteDirectoryResult
                ;

            return ret;
        }
    }
}
