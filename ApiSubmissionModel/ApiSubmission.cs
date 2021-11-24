using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EQService.ApiSubmissionModel
{
    /// <summary>
    /// model for Xml root 
    /// </summary>
    [XmlRoot("ApiSubmission")]
    public sealed class ApiSubmission
    {
        [XmlElement("Records")]
        public List<ApiSubmissionRecords> Records { get; set; }

        [XmlAttribute("CustId")]
        public string CustomerID { get; set; }
    }
}
