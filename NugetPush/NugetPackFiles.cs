using System;
using System.Diagnostics;
using System.IO;

// Copyright (c) Dawid Sienkiewicz and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetPush
{
    class NugetPackFiles
    {
        private string _location { get; set; }

        private string _fileName { get; set; }

        public bool Prepare(string packagesDir)
        {
            bool result = false;

            if(GetAbsoluteDir(packagesDir, out var absoluteDir))
            {
                _location = absoluteDir;

                if(GetLatestFileName(absoluteDir, out var fileName))
                {
                    _fileName = fileName;
                    result = true;
                }
            }

            return result;
        }


        private bool GetAbsoluteDir(string packagesDir, out string absoluteDir)
        {
            bool result = false;

            absoluteDir = packagesDir;

            try
            {
                if (!Path.IsPathRooted(absoluteDir))
                {
                    absoluteDir = Path.Combine(Directory.GetCurrentDirectory(), packagesDir);
                }

                Console.WriteLine($"info: absolute dir: {absoluteDir}");

                if (Directory.Exists(absoluteDir))
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine($"error: path {absoluteDir} doesn't exist!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        private bool GetLatestFileName(string absoluteDir, out string latestFileName)
        {
            bool result = false;

            latestFileName = string.Empty;

            try
            {
                var files = Directory.GetFiles(absoluteDir, "*.nupkg");

                FileInfo latest = null;

                foreach (var file in files)
                {
                    var fi = new FileInfo(file);

                    if (latest == null)
                    {
                        latest = fi;
                    }
                    else if (fi.CreationTime > latest.CreationTime)
                    {
                        latest = fi;
                    }
                }

                if (latest != null)
                {
                    latestFileName = latest.Name;

                    Console.WriteLine($"info: latest package, name: {latestFileName}");

                    result = true;
                }
                else
                {
                    Console.WriteLine($"error: no nupkg file found!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return result;
        }

        private bool ChangeCurrentDir(out string oldDir)
        {
            bool result = false;

            oldDir = string.Empty;

            try
            {
                oldDir = Directory.GetCurrentDirectory();

                Directory.SetCurrentDirectory(_location);

                result = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        private bool RestoreCurrentDir(string path)
        {
            bool result = false;

            try
            {
                Directory.SetCurrentDirectory(path);

                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public int Push(string apiKey, string endpoint)
        {
            int result = 0;

            try
            {
                if (ChangeCurrentDir(out string oldDir))
                {
                    using (Process compiler = new Process())
                    {
                        string args = $"nuget push {_fileName} -k {apiKey} -s {endpoint}";

                        compiler.StartInfo.FileName = "dotnet";
                        compiler.StartInfo.Arguments = args;
                        compiler.StartInfo.UseShellExecute = false;
                        compiler.StartInfo.RedirectStandardOutput = true;

                        compiler.Start();

                        Console.WriteLine(compiler.StandardOutput.ReadToEnd());

                        compiler.WaitForExit();

                        result = compiler.ExitCode;
                    }

                    RestoreCurrentDir(oldDir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }
    }
}
