using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{

    public class UpwardRequestsInput
    {
        [DataMember]
        public List<UpwardRequests> UpwardRequests { get; set; }
    }

    [DataContract]
    public class UpwardRequests
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public int DlId { get; set; }
        [DataMember]
        public int DlCurrentVersion { get; set; }
        [DataMember]
        public string DeviceID { get; set; }
        [DataMember]
        public List<UpwardSyncRequestEntity> UpwardSyncRequests { get; set; }
    }
    [DataContract]
    public class UpwardSyncRequestEntity
    {
        [DataMember]
        public String AnnotationID { get; set; }
        [DataMember]
        public String Annotation { get; set; }
        [DataMember]
        public String Status { get; set; }
    }
}
