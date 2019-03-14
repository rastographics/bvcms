using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using CmsData.API;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;

namespace CmsData
{public partial class PythonModel
    {
        // ReSharper disable InconsistentNaming
        private readonly CMSDataContext db;
        private Dictionary<string, object> dictionary { get; }

        public dynamic instance { get; set; }

        public PythonModel(string dbname)
        {
            dictionary = new Dictionary<string, object>();
            Data = new DynamicData(dictionary);
            db = DbUtil.Create(dbname);
        }
        
        public PythonModel(string dbname, Dictionary<string, object> dict)
        {
            dictionary = dict;
            Data = new DynamicData(dictionary);
            db = DbUtil.Create(dbname);
        }

        /// <summary>
        /// Construct with a data context object
        /// </summary>
        /// <param name="dbContext"></param>
        
        public PythonModel(CMSDataContext dbContext)
        {
            db = dbContext.Copy();
            dictionary = new Dictionary<string, object>();
            Data = new DynamicData(dictionary);
        }

        public PythonModel(CMSDataContext dbContext, Dictionary<string, object> dict)
        {
            db = dbContext.Copy();
            dictionary = dict;
            Data = new DynamicData(dictionary);
        }

        /// <summary>
        ///     This constructor creates an instance of the class named classname, and is called with pe.instance.Run().
        ///     It supports the old style of MorningBatch and RegisterEvent.
        /// </summary>
        public PythonModel(string dbname, string classname, string script)
            : this(dbname)
        {
            var engine = Python.CreateEngine();
            var pc = HostingHelpers.GetLanguageContext(engine) as PythonContext;
            var hooks = pc?.SystemState.Get__dict__()["path_hooks"] as List;
            hooks?.Clear();
            var searchPaths = engine.GetSearchPaths();
            searchPaths.Add(ConfigurationManager.AppSettings["PythonLibPath"]);
            engine.SetSearchPaths(searchPaths);
            var sc = engine.CreateScriptSourceFromString(script);

            var code = sc.Compile();
            var scope = engine.CreateScope();
            scope.SetVariable("model", this);
            code.Execute(scope);

            dynamic Event = scope.GetVariable(classname);

            instance = Event();
        }

        QueryFunctions QueryFunctions()
        {
            return new QueryFunctions(db, dictionary);
        }

        public string DatabaseName => db.Host;

        private CMSDataContext NewDataContext()
        {
            return DbUtil.Create(db.Host);
        }

        public void DebugWriteLine(object o)
        {
            Debug.WriteLine(o);
        }

        public void LogToContent(string file, string text)
        {
            db.Log2Content(file, text);
        }

        public string RunScript(string script)
        {
            try
            {
                Output = ExecutePython(script, this);
            }
            catch (Exception ex)
            {
                Output = ex.Message;
            }
            return Output;
        }
        public string RunScriptFile(string script)
        {
            try
            {
                Output = ExecutePython(script, this, fromFile: true);
            }
            catch (Exception ex)
            {
                Output = ex.Message;
            }
            return Output;
        }

        public static string RunScript(string dbname, string script)
        {
            return ExecutePython(script, new PythonModel(dbname));
        }
        public static string RunScript(string dbname, string script, DateTime time)
        {
            var pe = new PythonModel(dbname) {ScheduledTime = time.ToString("HHmm")};
            return ExecutePython(script, pe);
        }

        public static string ExecutePython(string script, PythonModel model, bool fromFile = false)
        {
            var engine = fromFile
                ? Python.CreateEngine(new Dictionary<string, object> { ["Debug"] = true })
                : Python.CreateEngine();
            var pc = HostingHelpers.GetLanguageContext(engine) as PythonContext;
            var hooks = pc?.SystemState.Get__dict__()["path_hooks"] as List;
            hooks?.Clear();
            var searchPaths = engine.GetSearchPaths();
            searchPaths.Add(ConfigurationManager.AppSettings["PythonLibPath"]);
            engine.SetSearchPaths(searchPaths);

            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            {
                engine.Runtime.IO.SetOutput(ms, sw);
                engine.Runtime.IO.SetErrorOutput(ms, sw);

                try
                {
                    var sc = fromFile
                        ? engine.CreateScriptSourceFromFile(script)
                        : engine.CreateScriptSourceFromString(script);
                    var code = sc.Compile();

                    var scope = engine.CreateScope();
                    scope.SetVariable("model", model);
                    scope.SetVariable("Data", model.Data);

                    var qf = new QueryFunctions(model.db, model.dictionary);
                    scope.SetVariable("q", qf);
                    code.Execute(scope);

                    ms.Position = 0;

                    using (var sr = new StreamReader(ms))
                    {
                        var s = sr.ReadToEnd();
                        return s.Replace("\r\r\n", "\n");
                    }
                }
                catch (Exception ex)
                {
                    var err = engine.GetService<ExceptionOperations>().FormatException(ex);
                    throw new Exception(err);
                }
            }
        }

        public static string ExecutePythonFilex(string dbname, string file)
        {
            var model = new PythonModel(dbname);
            return ExecutePython(file, model, fromFile: true);
        }
    }
}
