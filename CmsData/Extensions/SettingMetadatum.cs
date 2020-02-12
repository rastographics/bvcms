namespace CmsData
{
    public partial class SettingMetadatum 
    {
        public string GetValue()
        {
            var value = Setting?.SettingX;
            return string.IsNullOrEmpty(value) ? DefaultValue : value;
        }
    }
}
