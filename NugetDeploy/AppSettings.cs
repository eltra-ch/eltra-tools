using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

// Copyright (c) Dawid Sienkiewicz and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetPush
{
    class AppSettings
    {
        public string ApiKey { get; set; }
        public string PackagesDir { get; set; }
        public string Endpoint { get; set; }
        public bool Wait { get; set; }

        public bool Parse(string[] args)
        {
            bool result = false;

            try
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .AddCommandLine(args)
                                .Build();

                ApiKey = Configuration.GetValue<string>("AppSettings:NugetApiKey");
                PackagesDir = Configuration.GetValue<string>("AppSettings:PackagesDir");
                Wait = Configuration.GetValue<bool>("AppSettings:Wait");
                Endpoint = Configuration.GetValue<string>("AppSettings:Endpoint");

                Console.WriteLine($"command: use key: {ApiKey}");
                Console.WriteLine($"command: endpoint: {Endpoint}");
                Console.WriteLine($"command: packages dir: {PackagesDir}");
                Console.WriteLine($"command: wait for user action: {Wait}");
                
                result = !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(PackagesDir);

                if(!result)
                {
                    Console.WriteLine("error: something is missing there...");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }
    }
}
