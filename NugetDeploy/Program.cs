using System;

// Copyright (c) Dawid Sienkiewicz and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetPush
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new AppSettings();

            if (appSettings.Parse(args))
            {
                var nugetFiles = new NugetPackFiles();

                if (nugetFiles.Prepare(appSettings.PackagesDir))
                {
                    int exitCode = nugetFiles.Push(appSettings.ApiKey, appSettings.Endpoint);

                    if (exitCode != 0)
                    {
                        Console.WriteLine($"error: push exit code = {exitCode}");
                    }
                }
            }

            if (appSettings.Wait)
            {
                Console.WriteLine("press any key...");

                Console.ReadKey();
            }
        }
    }
}
