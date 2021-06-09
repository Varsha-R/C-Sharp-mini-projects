using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDeserialize
{
    public static class SubSettingExtension
    {
        public static string GetString(this List<SubSettings> subSetting)
        {
            if (subSetting != null && subSetting.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (SubSettings item in subSetting)
                {
                    builder.Append(item.ToString());
                    builder.Append(Environment.NewLine);
                }
                return builder.ToString();
            }
            return null;
        }
    }
}
