using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EQService.ApiSubmissionModel
{
    public sealed class ApiSubmissionRecord
    {
        [XmlElement("Field")]
        public List<ApiSubmissionRecordField> Fields { get; set; }
    }
}
