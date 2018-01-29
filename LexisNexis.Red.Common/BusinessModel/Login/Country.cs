using System.Xml.Serialization;
namespace LexisNexis.Red.Common.BusinessModel
{
    public class Country
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Language { get; set; }
        public string RemoteUrl { get; set; }
        public string CustomerSupportTEL { get; set; }
        public string CustomerSupportEmail { get; set; }
        public string ContentLinkRedirectorUrl { get; set; }
        public string DictionaryUrl { get; set; }
        public ContactUs ContactUs { get; set; }
        public string ServiceCode { get; set; }
    }

    public class ContactUs
    {
        private string sendByDX = "";
        public string Phone { get; set; }
        public string InternationalCallers { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public PostToUs PostToUs { get; set; }
        public string SendByDX
        {
            get { return sendByDX.Trim(); }
            set { sendByDX = value; }
        }
        public string WorkingHours { get; set; }

    }
    public class PostToUs
    {
        private string content = "";
        [XmlAttribute("Title")]
        public string Title { get; set; }
        [XmlElementAttribute("Content")]
        public string Content
        {
            get
            {
                return content.Trim();
            }
            set
            {
                content = value;
            }
        }

    }
}
