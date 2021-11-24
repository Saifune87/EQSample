using System;
using System.Xml.Serialization;

namespace EQService.ApiSubmissionModel
{
    public sealed class ApiSubmissionRecordField
    {
        [XmlAttribute]
        public string FieldName { get; set; }

        [XmlIgnore]
        public object Value { get; set; }
        [XmlText]
        public string ValueString
        {
            get
            {
                return Value.ToString();
            }
            set { }
        }
    }
}
