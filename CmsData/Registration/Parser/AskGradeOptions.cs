using System.Linq;
using System.Text;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public void Output(StringBuilder sb, AskGradeOptions ask)
		{
			if (ask.list.Count == 0)
				return;
			AddValueNoCk(0, sb, "AskGradeOptions", ask.Label);
			foreach (var g in ask.list)
			{
			    AddValueCk(1, sb, g.Description);
			    AddValueCk(2, sb, "Code", g.Code);
			}
		    sb.AppendLine();
		}
		public AskGradeOptions ParseAskGradeOptions()
		{
			var go = new AskGradeOptions();
			go.Label = GetString("AskGradeOptions");
			if (curr.indent == 0)
				throw GetException("expected indented Options");
			var startindent = curr.indent;
			while (curr.indent == startindent)
			{
				if (curr.kw != RegKeywords.None)
					throw GetException("expected description only");
				var option = new AskGradeOptions.GradeOption();
				option.Description = GetLine();
				if (curr.indent <= startindent)
					throw GetException("expected greater indent");
				if (curr.kw != Parser.RegKeywords.Code)
					throw GetException("expected Code");
				var code = GetNullInt();
				if (!code.HasValue)
				{
					lineno--;
					throw GetException("expected integer code");
				}
				option.Code = code.Value;

                if(go.list.All(gg => gg.Code != option.Code))
    				go.list.Add(option);
			}
			return go;
		}
	}
}
