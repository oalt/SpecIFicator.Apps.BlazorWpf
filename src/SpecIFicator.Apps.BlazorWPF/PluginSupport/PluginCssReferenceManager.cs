using SpecIFicator.Framework.PluginManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SpecIFicator.Apps.BlazorWPF.PluginSupport
{
    // copy all bundle css files of plugins to the wwwroot/.../pluginStyles driectory and create a pluginStyles.css file.
    internal class PluginCssReferenceManager
    {
        private static string _cssTargetPath = AssemblyDirectory + "\\wwwroot\\_content\\SpecIFicator.Framework\\css\\pluginStyles\\";

        private PluginCssReferenceManager() 
        {
        }

        public static void CreateCssReferences()
        {
            try
            {
                List<string> cssFileNames = new List<string>();

                string pluginPath = PluginManager.PluginPath;

                DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);

                if (pluginDirectory.Exists)
                {
                    DirectoryInfo[] pluginDirectories = pluginDirectory.GetDirectories();

                    foreach (DirectoryInfo pluginDirectoryInfo in pluginDirectories)
                    {
                        FileInfo[] cssFiles = pluginDirectoryInfo.GetFiles("*.bundle.scp.css");

                        foreach (FileInfo cssFile in cssFiles)
                        {
                            cssFileNames.Add(cssFile.Name);

                            if (IsCopyRequired(cssFile))
                            {

                                File.Copy(cssFile.FullName, _cssTargetPath + "\\" + cssFile.Name, true);

                            }
                        }
                    }
                }

                CreateAndSavePluginStylesFile(cssFileNames);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private static bool IsCopyRequired(FileInfo cssSourceFileInfo)
        {
            bool result = true;

            DirectoryInfo tartgetDirectory = new DirectoryInfo(_cssTargetPath);

            if (tartgetDirectory.Exists)
            {
                FileInfo[] cssTargetFiles = tartgetDirectory.GetFiles("*.bundle.scp.css");
                foreach(FileInfo targetFileInfo in cssTargetFiles)
                {
                    if(targetFileInfo.Name == cssSourceFileInfo.Name)
                    {
                        if(targetFileInfo.CreationTime.Equals(cssSourceFileInfo.CreationTime))
                        {
                            result = false;
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static void CreateAndSavePluginStylesFile(List<string> cssFiles)
        {
            string content = "";

            foreach(string cssFile in cssFiles)
            {
                content += "@import '" + cssFile + "';\r\n";
            }

            if(Directory.Exists(_cssTargetPath)) 
            {
                File.WriteAllText(_cssTargetPath + "/pluginStyles.css", content);
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
