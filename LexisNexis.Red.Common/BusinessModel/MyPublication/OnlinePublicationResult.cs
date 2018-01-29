using System.Collections.Generic;
namespace LexisNexis.Red.Common.BusinessModel
{
    public class OnlinePublicationResult
    {
        public RequestStatusEnum RequestStatus { get; set; }

        public List<Publication> Publications { get; set; }
    }
}
