using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExitGames.Logging;

namespace Photon.Common.Plugins
{
    public static class AssemblyResolver
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly List<string> exitGamesLibsList = new List<string>(); 

        static AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAsseblyResolve;
        }

        public static void AddExitGamesLibs(IEnumerable<string> libs)
        {
            lock (exitGamesLibsList)
            {
                foreach (var lib in libs.Where(lib => !exitGamesLibsList.Contains(lib)))
                {
                    exitGamesLibsList.Add(lib);
                }
            }
        }

        private static bool IsExitGamesLib(string name)
        {
            return exitGamesLibsList.Any(name.Contains);
        }

        static private Assembly OnAsseblyResolve(object sender, ResolveEventArgs args)
        {
            if (IsExitGamesLib(args.Name))
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Loading assembly '{0}' from Application context", args.Name);
                }
                var exitGamesAssembly = Assembly.Load(args.Name);
                if (exitGamesAssembly != null)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Assembly '{0}' are loaded successfully from Application context", args.Name);
                    }
                }
                else
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Failed to load assembly '{0}' from Application context", args.Name);
                    }
                }
                return exitGamesAssembly;
            }

            var requestingAssemblyPath = args.RequestingAssembly.Location;
            var path = Path.GetDirectoryName(requestingAssemblyPath);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var asmName = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
            path = Path.Combine(path, asmName);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Loading assembly '{0}' from path '{1}'", args.Name, path);
            }
            var asm = Assembly.LoadFile(path);
            if (asm != null)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Assembly '{0}' are loaded successfully from '{1}'", args.Name, path);
                }
            }
            else
            {
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Failed to load assembly '{0}' from path '{1}'", args.Name, path);
                }
            }
            return asm;
        }
    }
}
