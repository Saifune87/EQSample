using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EQService.ApiSubmissionModel
{
    public sealed class ApiSubmissionRecords
    {
        [XmlAttribute]
        public string TableName { get; set; }
        [XmlAttribute]
        public int FormId { get; set; }
        [XmlAttribute]
        public int SubmissionSequence { get; set; }
        [XmlElement("Record")]
        public List<ApiSubmissionRecord> Records { get; set; }
    }
}
