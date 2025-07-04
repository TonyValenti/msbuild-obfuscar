using System;
using System.Collections.Generic;

namespace Obfuscar.MsBuild.Tasks {
    internal partial class ObfuscateArgs {
        public string Obfuscator_Path { get; set; } = string.Empty;
        public string Obfuscator_ConfigTemplate { get; set; } = string.Empty;
        public bool Obfuscator_ConfigTemplate_ProjectReferences_Append { get; set; }

        public IReadOnlyCollection<string> Obfuscator_Targets { get; set; } = Array.Empty<string>();


        public string ConfigurationName { get; set; } = string.Empty;
        

        public string SolutionDir { get; set; } = string.Empty;
        public string SolutionFileName { get; set; } = string.Empty;
        public string SolutionName { get; set; } = string.Empty;

        public string ProjectDir { get; set; } = string.Empty;
        public string ProjectFileName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public IReadOnlyCollection<string> ProjectReferencedFiles { get; set; } = Array.Empty<string>();
        public IReadOnlyCollection<string> ProjectReferencedFolders { get; set; } = Array.Empty<string>();


        public string TargetDir { get; set; } = string.Empty;
        public string TargetFileName { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;

        public string InPath { get; set; } = string.Empty;
        public string OutPath { get; set; } = string.Empty;
        public string OutPathConfig { get; set; } = string.Empty;

        public string Module { get; set; } = string.Empty;
    }
}
