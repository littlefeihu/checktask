using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Business
{
    public class NavigationManager
    {
        private List<BrowserRecord> records;
        private int currenRecordIndex;
        private static NavigationManager instance;
        private NavigationManager()
        {
            currenRecordIndex = -1;
            records = new List<BrowserRecord>();
        }

        static NavigationManager()
        {
            instance = new NavigationManager();
        }

        public static NavigationManager Instance
        {
            get
            {
                return instance;
            }
        }
        public void Clear()
        {
            if (records != null)
            {
                records.Clear();
            }
            currenRecordIndex = -1;
        }
        /// <summary>
        /// indicate whether the current record  can back
        /// </summary>
        [JsonIgnore]
        public bool CanBack { get { return currenRecordIndex > 0; } }
        /// <summary>
        /// indicate whether the current record can forth
        /// </summary>
        [JsonIgnore]
        public bool CanForth { get { return currenRecordIndex < records.Count - 1; } }

        /// <summary>
        /// create new browser record
        /// </summary>
        /// <param name="record"></param>
        public void AddRecord(BrowserRecord record)
        {
            //Avoid add duplicate records
            var existSameRecord = (records != null && records.Count > 0)
                                && (records[currenRecordIndex].GetType() == record.GetType())
                                && (records[currenRecordIndex].Equals(record));
            if (existSameRecord)
            {
                return;
            }
            bool isLastIndex = (currenRecordIndex == records.Count - 1);
            if (isLastIndex)
            {
                currenRecordIndex++;
                records.Add(record);
            }
            else if (currenRecordIndex < records.Count - 1)
            {
                currenRecordIndex++;
                var backCount = records.Count - currenRecordIndex;
                records.RemoveRange(currenRecordIndex, backCount);
                records.Add(record);
            }
        }

        /// <summary>
        /// get Next browser record
        /// </summary>
        /// <returns></returns>
        public BrowserRecord Forth()
        {
            currenRecordIndex++;
            return records[currenRecordIndex];
        }
        /// <summary>
        /// get previours  browser record
        /// </summary>
        /// <returns></returns>
        public BrowserRecord Back()
        {
            currenRecordIndex--;
            return records[currenRecordIndex];
        }
        /// <summary>
        /// get current browser record
        /// </summary>
        [JsonIgnore]
        public BrowserRecord CurrentRecord
        {
            get
            {
                if (records.Count == 0)
                {
                    return null;
                }
                return records[currenRecordIndex];
            }
        }
        [JsonProperty("Records")]
        public List<BrowserRecord> Records
        {
            get
            {
                return records;
            }
        }
        [JsonProperty("CurrentIndex")]
        public int CurrentIndex
        {
            get
            {
                return currenRecordIndex;
            }
            set
            {
                currenRecordIndex = value;
            }
        }


        public void CalculateCurrentIndex(int booKId, int firstTOCId)
        {
            try
            {
                var currentRecordID = CurrentRecord.RecordID;
                Dictionary<Guid, List<BrowserRecord>> dic = new Dictionary<Guid, List<BrowserRecord>>();
                for (int i = 0; i < records.Count; i++)
                {
                    if (records[i].BookID == booKId)
                    {
                        if (i != 0 && records[i - 1].BookID == booKId)
                        {
                            dic.LastOrDefault().Value.Add(records[i]);
                        }
                        else
                        {
                            dic.Add(records[i].RecordID, new List<BrowserRecord> { records[i] });
                        }
                    }
                }
                foreach (var item in dic)
                {
                    var index = records.FindIndex(o => o.RecordID == item.Key);
                    var record = item.Value.FirstOrDefault(o => o.RecordID == currentRecordID);
                    if (record != null)
                    {
                        currenRecordIndex = index;
                    }
                    records.Insert(index, new ContentBrowserRecord(booKId, firstTOCId, 0));
                    index = index + 1;
                    records.RemoveRange(index, item.Value.Count);
                }
                var indexFlag = records.FindIndex(o => o.RecordID == currentRecordID);
                if (indexFlag != -1)
                {
                    currenRecordIndex = indexFlag;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("CalculateCurrentIndex error" + ex.ToString());
            }
        }

        /// <summary>
        /// build navigation manager from string data
        /// </summary>
        /// <param name="recordsData"></param>
        public void RestoreRecords(string recordsData)
        {
            var recordData = JsonConvert.DeserializeObject<RecordData>(
                      recordsData,
                      new JsonSerializerSettings
                      {
                          TypeNameHandling = TypeNameHandling.Auto,
                          Binder = TypeNameSerializationBinder.Instance
                      });
            this.currenRecordIndex = recordData.CurrentIndex;
            this.records = recordData.Records;
        }
        /// <summary>
        /// Serialize  navigation manager to string data
        /// </summary>
        /// <returns></returns>
        public string SerializeRecords()
        {
            return JsonConvert.SerializeObject(
                    instance,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Binder = TypeNameSerializationBinder.Instance
                    });
        }
        public class RecordData
        {
            public int CurrentIndex { get; set; }
            public List<BrowserRecord> Records
            {
                get;
                set;
            }
        }
    }
}
