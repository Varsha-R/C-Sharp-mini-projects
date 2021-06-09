using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDeserialize
{
    public class SubSettings
    {
        public string SettingId { get; set; }
        public string SettingNameKey { get; set; }
        public string SettingNameValue { get; set; }
        public string SettingDescriptionKey { get; set; }
        public string SettingDescriptionValue { get; set; }
        public bool? Selected { get; set; }
        public string ReasonForFailure { get; set; }
        public string ResolutionKey { get; set; }
        public string ResolutionValue { get; set; }
        public bool? Completed { get; set; }
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append($"SettingId: {SettingId}, ");
            text.Append($"SettingNameKey: {SettingNameKey}, ");
            text.Append($"SettingNameValue: {SettingNameValue}, ");
            text.Append($"SettingDescriptionKey: {SettingDescriptionKey}, ");
            text.Append($"SettingDescriptionValue: {SettingDescriptionValue}, ");
            text.Append($"Selected: {Selected}, ");
            text.Append($"ReasonForFailure: {ReasonForFailure}, ");
            text.Append($"ResolutionKey: {ResolutionKey}, ");
            text.Append($"ResolutionValue: {ResolutionValue}, ");
            text.Append($"Completed: {Completed}");
            return text.ToString();
        }
    }
}
