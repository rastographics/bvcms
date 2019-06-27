namespace CmsWeb.Code
{
    public static class XmlHelper
    {
        public static string RemoveTags(string xml, string tag)
        {
            if (xml.IndexOf(tag) != -1)
            {
                xml = xml.Remove(xml.IndexOf(tag), tag.Length);
                return RemoveTags(xml, tag);
            }
            else
            {
                return xml;
            }
        }
    }
}
