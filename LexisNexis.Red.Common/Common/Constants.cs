namespace LexisNexis.Red.Common.Common
{
    public static class Constants
    {
        public const string SPACE = " ";
        public const string POST = "POST";
        public const string GET = "Get";
        public const string CONTENT_TYPE_XML = @"application/xml";
        public const string CONTENT_TYPEJSON = @"application/json";
        public const string STATUS_CODE_MESSAGE = "The request did not complete successfully and returned status code ";
        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string ZIP = ".zip";
        public const string XML = ".xml";
        public const string SQLITE = ".sqlite";
        public const string SQLITE_Decrypted = "DigitalLooseLeafDecrypted.sqlite";
        public const string SQLITE_Xpath = "Xpath.sqlite";
        public const string CONTENT_TYPE = "ContentType";
        public const string EMAIL_REGEX = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public const string NET_ERROR = "Unable to connect to the remote server";
        public const string DEVICE_NOT_MATCHED = "Device id is not mapped to the email address provided.";
        public const string EMAIL_NOT_EXIST = "Email address does not exist in the subscription system.";
        public const int VALID_PWD_LENGTH = 3;
        public const string DATABASE_FILE_NAME = "LexisNexisRed.sqlite";
        public const string LOAN_DAY_REMAINING_DUE_TO_EXPIRE = "Due to expire";
        public const string LOAN_DAY_REMAINING = "1 day remaining";
        public const string LOAN_DAYS_REMAINING = "{0} days remaining";
        public const string INITIAL_DATABASE = "LexisNexis.Red.Common.Database.LexisNexisRed.sqlite";
        public const string MAX_REG_EXCEEDED = "You have reached the maximum number of registered devices available for your account. For more information please contact Customer Support on 1800 772 772.";
        public const string REG_BEFORE_DONWLOAD = "Please register device before starting the download process.";
        public const string ERROR_DURING_DOWNLOAD = "Error occurred during DL book download.";
        public const string EMPTY_REQUEST = "Given request should not be empty or null.";
        public const string EREADER_VERSION_EMPTY_NULL = "eReader Version number should not be empty or null.";
        public const string DICTIONARY_FILE_NAME_FULLNAME = "LexisNexis.Red.Common.Segment.Dictionary.txt";
        public const string STOPWORD_FILE_NAME_FULLNAME = "LexisNexis.Red.Common.Segment.Stopword.txt";
        public const string DICTIONARY_FILE_NAME = "Dictionary.txt";
        public const string STOPWORD_FILE_NAME = "Stopword.txt";
        public const string CONTENT_IMAGES_PLACEHOLDER = "##file://inlineobjectlocalpath##";
        /// <summary>
        /// 1M
        /// </summary>
        public const int BUFFER_LENGTH = 1024 * 1024;
        public const string DLFiles = "DLFiles";
        public const string SearchIndex = "SearchIndex";
        public const string SearchIndexFileType = "SearchIndexFileType.zip";
        public const string SEntry = "entry";
        public const string SCipherText = "cipherText";
        public const string Key = "Key";
        public const string IV = "IV";
        public const string TocFileName = "toc.xml";
        public const string UTFEncoding = "UTF-8";
        public const string SISOEncoding = "iso-8859-1";
        public const string LOGGER_NAME = "RedLogger";
        public const string TXT_SUFFIXNAME = ".txt";
        public const string ERROR_MESSAGE = "An existing connection was forcibly closed by the remote host";
        public const string ERROR_MESSAGE1 = "An established connection was aborted by the software in your host machine";
        public const string ANNOTATION_CATEGORY_TAGS_SYNCDATA = "anncategorytagssyncdata";
        public const string ANCESTOR = "ancestor";
        public const string ASSEMBLY_NAME = "LexisNexis.Red.Common";
        public const string Zip = ".zip";
        public const string Xml = ".xml";

        /// <summary>
        /// For MasterFile
        /// </summary>
        public const string AnnotationMasterFile = "MasterFile.xml";
        public const string RosettaXML = "rosettaxmlfilename";
        public const string AnnotationFileName = "annotationfilename";
        public const string AnnotationEntry = "annotationentry";
        public const string STrue = "true";
        public const string SFalse = "false";
        public const string Sissync = "issync";
        public const string SLastSyncDate = "lastsyncdate";
        /// <summary>
        /// For Sync
        /// </summary>
        public const string UploadAnnotations = "UploadAnnotations";
        public const string UploadAnnotationZip = "UploadAnnotation.zip";
        public const string SDeletedAnnotationsXml = "deletedannotations.xml";
        public const string SDeletedAnnotations = "deletedannotations";
        public const string SUpwardSyncRequest = "upwardsyncrequest";
        public const string SUserid = "userid";
        public const string SyncSDlid = "dlid";
        public const string SDlversionid = "dlversionid";
        public const string SDeviceid = "deviceid";
        public const string ISOEncoding = "ISO-8859-1";
        public const string EmptyStreamXml = "emptyStream.xml";
        public const string DLUnderscore = "DL_";
        public const string Status = "status";

        /// <summary>
        /// For Anntoations
        /// </summary>
        public const string Annotation = "annotation";
        public const string SAnnotations = "annotations";
        public const string AnnotationFolder = "Annotations";
        public const string Annotations = "Annotations.xml";
        public const string AnnotationDownZip = "AnnotationDown.zip";
        public const string AnnotationDown = "AnnotationDown";
        public const string AnnotationGuideCardNameTrim = "AnnotationGuideCardNameTrim";
        public const string CUserId = "Userid";
        public const string AnnSUserId = "userid";
        public const string Deviceid = "Deviceid";
        public const string AnnSDeviceId = "deviceid";
        public const string AnnDeviceId = "DeviceId";
        public const string DLBookId = "DlBookId";
        public const string SdlBookId = "dlId";
        public const string DLBookStatus = "DlBookStatus";
        public const string LastAccessDate = "LastAccessDate";
        public const string SdlBookStatus = "status";
        public const string SdlBookMessage = "message";
        public const string CreatedOn = "Createdon";
        public const string SCreatedOn = "createdon";
        public const string NFileName = "Filename";
        public const string SmallFileName = "filename";
        public const string Type = "Type";
        public const string SType = "type";
        public const string SId = "id";
        public const string SRefpt = "refpt";
        public const string SBlockQuote = "blockquote";
        public const string SHrule = "hrule";
        public const string SPage = "page";
        public const string GuideCardName = "Guidecardname";
        public const string SGuideCardName = "guidecardname";
        public const string Note = "Note";
        public const string SNote = "note";
        public const string SHighlight = "highlight";
        public const string Highlight = "Highlight";
        public const string CategoryTagIDs = "CategoryTagIDs";
        public const string DLBookSectionTitle = "DlBookSectionTitle";
        public const string SdlBookTitle = "title";
        public const string AnnotationData = "Annotationdata";
        public const string SAnnotationData = "annotationdata";
        public const string StartLevel = "Startlevel";
        public const string SmallStartLevel = "startlevel";
        public const string EndLevel = "Endlevel";
        public const string SmallEndLevel = "endlevel";
        public const string SmallXPath = "xpath";
        public const string SmallLevelId = "levelId";
        public const string SDeleted = "deleted";
        public const string SOrphaned = "orphaned";
        public const string SUpdated = "updated";
        public const string SCreated = "created";
        public const string SUpdatedOn = "updatedon";
        public const string SOffsetValue = "offsetValue";
        public const string SRequestxml = "request.xml";
        public const string SErrorxml = "error.xml";
        public const string SIsupdated = "isUpdated";
        public const string Yes = "1";
        public const string No = "0";
        public const string DocId = "docid";
        public const string EndDocId = "enddocid";
        public const string StartDocId = "startdocid";
        public const string SOffset = "offset";

        /// <summary>
        /// For Search Segment
        /// </summary>
        public const string OR_SEPERATOR = " OR ";
        public const string AND_SEPERATOR = " AND ";
        public const string NEAR_SEPERATOR = " NEAR/26 ";
        public const string INDEX_DB_PATH = "Index.sqlite";
        public const string DICITIONARY_DB_NAME = "dictionary";
        public const string DICITIONARY_EXTENSION_NAME = ".sqlite";
        public const string GLOBAL_SETTING_MAIL = "All_USERS";
        public const string GLOBAL_SETTING_COUNTRY_CODE = "All_COUNTRY_CODE";
        public const int MAX_WORD_NUMBER = 13;
        public const string NZ_DICTIONARY_VERSION_TEXT = "New Zealand Law Dictionary, 8th edition";
        public const string AU_DICTIONARY_VERSION_TEXT = "LexisNexis Austrailian Legal Dictionary 4ed 2011";
    }
}
