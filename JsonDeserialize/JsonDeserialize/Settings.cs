using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDeserialize
{
    public class Settings
    {
        public string GroupId { get; set; }
        public string GroupNameValue { get; set; }
        public string GroupNameKey { get; set; }
        public string GroupDescriptionValue { get; set; }
        public string GroupDescriptionKey { get; set; }
        public bool? Selected { get; set; }
        public List<SubSettings> SubSettings { get; set; }
        public bool? Completed { get; set; }
        public string ReasonForFailure { get; set; }
        public string ResolutionKey { get; set; }
        public string ResolutionValue { get; set; }
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append($"GroupId: {GroupId}, ");
            text.Append($"GroupNameValue: {GroupNameValue}, ");
            text.Append($"GroupNameKey: {GroupNameKey}, ");
            text.Append($"GroupDescriptionValue: {GroupDescriptionValue}, ");
            text.Append($"GroupDescriptionKey: {GroupDescriptionKey}, ");
            text.Append($"Selected: {Selected}, ");
            text.Append($"SubSettings: {SubSettings?.GetString() ?? "null" }, ");
            text.Append($"Completed: {Completed}, ");
            text.Append($"ReasonForFailure: {ReasonForFailure}, ");
            text.Append($"ResolutionKey: {ResolutionKey}, ");
            text.Append($"ResolutionValue: {ResolutionValue}");
            return text.ToString();
        }
    }
}
