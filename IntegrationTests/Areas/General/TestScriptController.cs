using CmsData;
using IntegrationTests.Support;
using Shouldly;
using UtilityExtensions;
using Xunit;
using IntegrationTests.Resources;

namespace IntegrationTests.Areas.Manage
{
    [Collection("WebApp Collection")]
    public class TestScriptController : AccountTestBase
    {
        [Fact]
        public void PyscriptWithoutParameters()
        {
            username = RandomString();
            password = RandomString();
            CreateUser(roles: new[] { "Access", "Edit", "Admin" });
            Login();
            db.WriteContentPython("HelloWorld", "print 'Hello World'");
            Open($"{rootUrl}PyScript/HelloWorld");
            PageSource.ShouldContain("Hello World");
        }
        [Fact]
        public void PyscriptWithParameters()
        {
            username = RandomString();
            password = RandomString();
            CreateUser(roles: new[] { "Access", "Edit", "Admin" });
            Login();
            db.WriteContentPython("HelloWorld", "print 'parameters {} {}'.format(Data.p1, Data.p2)");
            Open($"{rootUrl}PyScript/HelloWorld/testing/123");
            PageSource.ShouldContain("parameters testing 123");
        }
        [Fact]
        public void PythonSearchNamesTest()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(roles: new[] { "Access", "Edit", "Admin" });
            var person = user.Person;
            Login();
            Open($"{rootUrl}PythonSearch/Names?term={person.FirstName.Truncate(4)} {person.LastName.Truncate(4)}");
            PageSource.ShouldContain($"{user.Person.LastName}, {user.Person.FirstName}");
        }
        [Fact]
        public void RunScriptTest()
        {
            username = RandomString();
            password = RandomString();
            CreateUser(roles: new[] { "Access", "Edit", "Admin" });
            Login();
            db.WriteContentSql("TestSql", "select 'Hello World'");
            Open($"{rootUrl}RunScript/TestSql");
            PageSource.ShouldContain("Hello World");
        }
        /// <summary>
        /// This PyScriptFormTest exercises a ton of methods in PythonModel
        /// including DynamicData, Forms, Json,
        /// and ScriptController PyScriptForm GET and POST Actions
        /// </summary>
        [Fact]
        public void PyScriptFormTest()
        {
            db.WriteContentText("jsondata", @"{'Records':[
    { 'Id': 1, 'Name': 'John Adams', 'City': 'Boston', 'Work': 'attorney' },
    { 'Id': 2, 'Name': 'Betsy Ross', 'City': 'Philadelphia', 'Work': 'seamstress' }
]}");
            db.WriteContentPython("record", R.PyScriptFormTest);
            username = RandomString();
            password = RandomString();
            CreateUser(roles: new[] { "Access", "Edit", "Admin" });
            Login();
            Open($"{rootUrl}PyScriptForm/Record/display/2");
            PageSource.ShouldContain("Philadelphia");

            Find(id: "submitit").Click();
            WaitForElement("input[name=City]");
            Find(name: "City").Clear();
            Find(name: "City").SendKeys("New York");
            Find(id: "submitit").Click();
            PageSource.ShouldContain("New York");

            /* need a new data context
             * since changes were made in another process
             * and the values will still be cached in the current data context
             */
            db = CMSDataContext.Create(Host);
            var json = db.ContentText("jsondata", null);
            json.ShouldContain("New York");
        }

    }
}
