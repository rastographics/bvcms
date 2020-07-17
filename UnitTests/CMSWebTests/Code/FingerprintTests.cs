using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using SharedTestFixtures;
using System.Text.RegularExpressions;
using CmsData;
using Shouldly;

namespace Tests
{
    [Collection(Collections.Miscellaneous)]
    public class FingerprintTests
    {
        [Fact]
        public void CssTest()
        {
            var fingerprints = ReadFingerprintsJson();
            var references = FindAllFingerprintReferences();
            foreach(var value in references)
            {
                fingerprints.ShouldContain(value, $"Fingerprints.json entry is missing for {value}");
            }
        }

        private List<string> FindAllFingerprintReferences()
        {
            var regex = new Regex(@"@Fingerprint\.(?:Css|CssPrint|Script)\(""(.+)""\)", RegexOptions.Multiline | RegexOptions.Compiled);
            var references = new List<string>();
            var dir = Path.GetFullPath(@"..\..\..\..\CmsWeb");
            foreach (var file in new DirectoryInfo(dir).GetFiles("*.cshtml", SearchOption.AllDirectories))
            {
                var matches = regex.Matches(File.ReadAllText(file.FullName));
                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1 && !references.Contains(match.Groups[1].Value))
                    {
                        references.Add(match.Groups[1].Value);
                    }
                }
            }
            return references;
        }

        private List<string> ReadFingerprintsJson()
        {
            var file = Path.GetFullPath(@"..\..\..\..\CmsWeb\Content\fingerprints.json");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file)).Keys.ToList();
        }
    }
}
