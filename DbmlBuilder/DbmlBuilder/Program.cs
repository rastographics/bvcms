using System;
using System.Collections;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using System.Configuration;
using DbmlBuilder.Utilities;
using DbmlBuilder.CodeGenerator;
using System.Windows.Forms;
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace DbmlBuilder
{
    internal class Program
    {
        private static Arguments arguments;
        private static readonly CodeGenerator.Compiler compiler = new CodeGenerator.Compiler();

        private static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            arguments = new Arguments(Environment.CommandLine);

            try
            {
                GenerateAll();

                if (compiler.Templates.Count > 0)
                {
                    Console.WriteLine("Running Compiler...");
                    compiler.Run();
                    Console.WriteLine("Writing Files...");
                    foreach (Template template in compiler.Templates)
                        using (StreamWriter sw = File.CreateText(template.OutputPath))
                            sw.Write(template.FinalCode);

                    Console.WriteLine("Done!");
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Error Message: {0}", x);
                Console.ReadKey();
            }
            timer.Stop();
            Console.WriteLine("Generated files are in " + GetOutputDirectory());
            Console.WriteLine("Execution Time: " + timer.ElapsedMilliseconds/1000 + " seconds");
            Console.WriteLine("Press a key to exit");
            Console.ReadKey();
        }

        #region Provider Startup

        private static string GetConfigPath()
        {
            string directory = Directory.GetCurrentDirectory();
            string configPath = Path.Combine(directory, "Web.config");
            if (File.Exists(configPath))
                return configPath;
            configPath = Path.Combine(directory, "DbmlBuilder.exe.config");
            if (File.Exists(configPath))
                return configPath;
            return null;
        }

        private static void SetProvider()
        {
            string configPath = GetConfigPath();
            Console.WriteLine("Setting ConfigPath: '{0}'", configPath);
            SetProvider(configPath);
        }

        private static void SetProvider(string appConfigPath)
        {
            if (!File.Exists(appConfigPath))
                throw new Exception("There's no config file present at " + appConfigPath);
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            Console.WriteLine("Building configuration from " + Path.Combine(Directory.GetCurrentDirectory(), appConfigPath));
            fileMap.ExeConfigFilename = appConfigPath;
            Configuration subConfig = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            try
            {
                DbmlBuilderSection section = (DbmlBuilderSection)subConfig.GetSection("DbmlBuilderService");

                if (section != null)
                {
                    Db.ConfigSection = section;
                    string connectionStringName = section.Providers[0].Parameters["connectionStringName"];
                    if (connectionStringName == null)
                        throw new ConfigurationErrorsException("The Parameter 'connectionStringName' was not specified");
                    ConnectionStringSettings connSettings = subConfig.ConnectionStrings.ConnectionStrings[connectionStringName];
                    if (connSettings == null)
                        throw new ConfigurationErrorsException(string.Format(
                            "ConnectionStrings section missing connection string with the name '{0}'", connectionStringName));

                    Db.Service = (SqlDataProvider)ProvidersHelper.InstantiateProvider(
                        section.Providers[0], typeof(SqlDataProvider));
                    Db.Service.DefaultConnectionStringName = connectionStringName;
                    Db.Service.DefaultConnectionString = connSettings.ConnectionString;

                }
            }
            catch (ConfigurationErrorsException x)
            {
                //let the user know the config was problematic...
                Console.WriteLine("There is an error with your config file. '{0}'", x.Message);
            }
        }

        private static string GetOutputDirectory()
        {
            string outDir = Path.Combine(Directory.GetCurrentDirectory(), "Generated");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            return outDir;
        }

        #endregion

        #region Utility

        private static void OutputFile(string filePath, string fileText)
        {
            using (StreamWriter sw = File.CreateText(filePath))
                sw.Write(fileText);
        }

        private static string GetArg(string argSwitch)
        {
            return arguments[argSwitch] ?? string.Empty;
        }

        #endregion

        #region Generators


        private static void GenerateAll()
        {
            SetProvider();
            GenerateContext();
            GenerateTables();
            GenerateViews();
            GenerateFunctions();
        }

        private static string GetOutSubDir()
        {
            string subdir = String.Empty;
            return GetOutSubDir(subdir);
        }
        private static string GetOutSubDir(string subdir)
        {
            string outDir = GetOutputDirectory();
            if (outDir == string.Empty)
                outDir = Directory.GetCurrentDirectory();

            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            if (subdir != string.Empty)
            {
                outDir = Path.Combine(outDir, subdir);
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);
            }
            return outDir;
        }

        private static void GenerateTables()
        {
            string fileExt = FileExtension.DOT_CS;

            string[] tables = Db.Service.GetTableNameList();
            string message = "Generating Table Entities for " + Db.Service.Name + " (" + tables.Length + " total)";

            Console.WriteLine(message);
            string outDir = GetOutSubDir();

            foreach (string tbl in tables)
            {
                string className = Db.Service.GetSchema(tbl, TableType.Table).ClassName;
                Template tt = CodeService.BuildClassTemplate(tbl);
                tt.OutputPath = Path.Combine(outDir, className + fileExt);
                compiler.AddTemplate(tt);
            }
            Console.WriteLine("Finished");
        }
        private static void GenerateContext()
        {
            string fileExt = FileExtension.DOT_CS;

            string outDir = GetOutputDirectory();
            if (outDir == string.Empty)
                outDir = Directory.GetCurrentDirectory();
            string outPath = Path.Combine(outDir, "Context" + fileExt);
            Console.WriteLine("Generating Structs to " + outPath);
            Template tt = CodeService.BuildContextTemplate();
            tt.OutputPath = outPath;
            compiler.AddTemplate(tt);

            Console.WriteLine("Finished");
        }


        private static void GenerateViews()
        {
            string fileExt = FileExtension.DOT_CS;


            //get the view list
            string[] views = Db.Service.GetViewNameList();
            string outDir = GetOutSubDir("Views");

            foreach (string tbl in views)
            {
                string className = Db.Service.GetSchema(tbl, TableType.View).ClassName;
                Template tt = CodeService.BuildViewTemplate(tbl);
                tt.OutputPath = Path.Combine(outDir, className + fileExt);
                compiler.AddTemplate(tt);
            }
        }
        private static void GenerateFunctions()
        {
            string fileExt = FileExtension.DOT_CS;

            string[] functions = Db.Service.GetFunctionNameList();
            string outDir = GetOutSubDir("Functions");

            foreach (string tbl in functions)
            {
                string n = Regex.Split(tbl, @"\.")[1];
                string className = Db.Service.GetSchema(n, TableType.Function).ClassName;
                Template tt = CodeService.BuildViewTemplate(n);
                tt.OutputPath = Path.Combine(outDir, className + fileExt);
                compiler.AddTemplate(tt);
            }
        }

        #endregion
    }
}
