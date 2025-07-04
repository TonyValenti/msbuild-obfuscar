using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Obfuscar.MsBuild.Tasks {
    internal static class ObfuscateArgsExtensions {

        private static string GetArgName(string Input) {
            var ret = $@"{{{{{Input}}}}}";

            return ret;
        }

        public static string Replace(this string This, ObfuscateArgs Args) {
            var ret = This;

            var Dictionary = Args.GetExportableArgs();

            foreach (var item in Dictionary) {
                var ArgName = GetArgName(item.Key);

                var Find = Regex.Escape(ArgName);
                var Replace = item.Value;

                ret = Regex.Replace(ret, Find, Replace, RegexOptions.IgnoreCase);
            }

            if (Args.Obfuscator_ConfigTemplate_ProjectReferences_Append) {
                var Doc = XDocument.Parse(ret);
                var Root = Doc.Root;

                foreach (var item in Args.ProjectReferencedFolders) {
                    var Node = new XElement("AssemblySearchPath");
                    var Attribute1 = new XAttribute("path", item);
                    Node.Add(Attribute1);

                    Root.Add(Node);
                }

                ret = Doc.ToString();

            }



            return ret;
        }

        public static ObfuscateArgs GetArgs(this Obfuscate This) {
            var InPath = This.TargetDir;
            var Module = System.IO.Path.Combine(InPath, This.TargetFileName);


            var RandomNumber = default(int);
            {
                var ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
                var rnd = new Random(ProcessId ^ System.Environment.TickCount);
                RandomNumber = rnd.Next();
            }

            {
                using var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
                var Bytes = new byte[sizeof(int)];
                rnd.GetBytes(Bytes);
                RandomNumber = BitConverter.ToInt32(Bytes, 0);
            }



            var OutPath = System.IO.Path.Combine(This.TargetDir, $@"obfuscated_{RandomNumber}");
            var OutPathConfig = System.IO.Path.Combine(OutPath, $@"Obfuscar.g.xml");

            var ProjectReferencedFiles = new List<string>();
            {
                foreach (var item in This.ProjectReferences) {
                    var ItemName = item.ItemSpec;
                    if (System.IO.File.Exists(ItemName)) {
                        ProjectReferencedFiles.Add(ItemName);
                    }
                }
                ProjectReferencedFiles = ProjectReferencedFiles.Distinct().OrderBy(x => x.ToLower()).ToList();
            }

            var ProjectReferencedFolders = new List<string>();
            {
                foreach (var item in This.ProjectReferences) {
                    var ItemName = System.IO.Path.GetDirectoryName(item.ItemSpec);
                    if (System.IO.Directory.Exists(ItemName)) {
                        ProjectReferencedFolders.Add(ItemName);
                    }
                }
                ProjectReferencedFolders = ProjectReferencedFolders.Distinct().OrderBy(x => x.ToLower()).ToList();
            }

            var Obfuscator_Targets = new[]{
                This.Obfuscator_Targets,
                "Debug,Release",
            }.WhereIsNotBlank().Coalesce();

            var Obfuscator_ConfigTemplate = string.Empty;

            var Obfuscator_ConfigTemplates = new List<string>();
            {
                var ConfigFileName1 = $@"Obfuscar.{Obfuscator_Targets}.xml";
                var ConfigFileName2 = $@"Obfuscar.xml";

                Obfuscator_ConfigTemplates.Add(This.Obfuscator_ConfigTemplate);
                Obfuscator_ConfigTemplates.Add(System.IO.Path.Combine(This.ProjectDir, This.Obfuscator_ConfigTemplate));
                Obfuscator_ConfigTemplates.Add(System.IO.Path.Combine(This.SolutionDir, This.Obfuscator_ConfigTemplate));

                Obfuscator_ConfigTemplates.Add(System.IO.Path.Combine(This.ProjectDir, ConfigFileName1));
                Obfuscator_ConfigTemplates.Add(System.IO.Path.Combine(This.ProjectDir, ConfigFileName2));

                Obfuscator_ConfigTemplates.Add(System.IO.Path.Combine(This.SolutionDir, ConfigFileName1));
                Obfuscator_ConfigTemplates.Add(System.IO.Path.Combine(This.SolutionDir, ConfigFileName2));

                foreach (var item in Obfuscator_ConfigTemplates) {
                    if (System.IO.File.Exists(item)) {
                        Obfuscator_ConfigTemplate = item;
                        break;
                    }
                }

            }


            var ret = new ObfuscateArgs() {
                Obfuscator_Path = This.Obfuscator_Path,
                Obfuscator_ConfigTemplate = This.Obfuscator_ConfigTemplate,
                Obfuscator_ConfigTemplate_ProjectReferences_Append = This.Obfuscator_ConfigTemplate_ProjectReferences_Append,

                Obfuscator_Targets = Obfuscator_Targets.SplitComma().ToHashSet(StringComparer.InvariantCultureIgnoreCase),


                SolutionDir = This.SolutionDir,
                SolutionFileName = This.SolutionFileName,
                SolutionName = This.SolutionName,

                ConfigurationName = This.ConfigurationName,

                ProjectDir = This.ProjectDir,
                ProjectFileName = This.ProjectFileName,
                ProjectName = This.ProjectName,

                TargetDir = This.TargetDir,
                TargetFileName = This.TargetFileName,
                TargetName = This.TargetName,

                InPath = InPath,
                Module = Module,
                OutPath = OutPath,
                OutPathConfig = OutPathConfig,

                ProjectReferencedFiles = ProjectReferencedFiles,
                ProjectReferencedFolders = ProjectReferencedFolders,
            };

            return ret;
        }

        public static IDictionary<string, string> GetExportableArgs(this ObfuscateArgs This) {
            var ret = new Dictionary<string, string>() {
                {ObfuscateArgNames.Default.Obfuscator_Path, This.Obfuscator_Path },
                {ObfuscateArgNames.Default.Obfuscator_ConfigTemplate, This.Obfuscator_ConfigTemplate},
                {ObfuscateArgNames.Default.Obfuscator_ConfigTemplate_ProjectReferences_Append, This.Obfuscator_ConfigTemplate_ProjectReferences_Append.ToString()},

                {ObfuscateArgNames.Default.InPath, This.InPath },
                {ObfuscateArgNames.Default.Module, This.Module },
                
                {ObfuscateArgNames.Default.OutPath, This.OutPath },
                {ObfuscateArgNames.Default.OutPathConfig, This.OutPathConfig },

                {ObfuscateArgNames.Default.ConfigurationName, This.ConfigurationName },

                {ObfuscateArgNames.Default.SolutionDir, This.SolutionDir },
                {ObfuscateArgNames.Default.SolutionFileName, This.SolutionFileName },
                {ObfuscateArgNames.Default.SolutionName, This.SolutionName },
                
                {ObfuscateArgNames.Default.ProjectDir, This.ProjectDir },
                {ObfuscateArgNames.Default.ProjectFileName, This.ProjectFileName },
                {ObfuscateArgNames.Default.ProjectName, This.ProjectName },
                
                {ObfuscateArgNames.Default.TargetDir, This.TargetDir },
                {ObfuscateArgNames.Default.TargetFileName, This.TargetFileName },
                {ObfuscateArgNames.Default.TargetName, This.TargetName },
            };

            return ret;
        }
    }
}
