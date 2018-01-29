using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LexisNexis.Red.Common.Segment.Dict
{
    class DictionaryLoader
    {
        public static Framework.Lock Lock = new Framework.Lock();

        private string _DictionaryDir = String.Empty;

        public string DictionaryDir
        {
            get
            {
                return _DictionaryDir;
            }
        }

        private DateTime _MainDictLastTime = DateTime.Now;
        private DateTime _ChsSingleLastTime = DateTime.Now;
        private DateTime _ChsName1LastTime = DateTime.Now;
        private DateTime _ChsName2LastTime = DateTime.Now;
        private DateTime _StopWordLastTime = DateTime.Now;
        private DateTime _SynonymLastTime = DateTime.Now;
        private DateTime _WildcardLastTime = DateTime.Now;

        //private Thread _Thread;

        private DateTime GetLastTime(string fileName)
        {
            //return System.IO.File.GetLastWriteTime(DictionaryDir + fileName);
            return DateTime.Now;
        }

        public DictionaryLoader(string dictDir)
        {
           // _DictionaryDir = Framework.Path.AppendDivision(dictDir, '\\');
            //_MainDictLastTime = GetLastTime("Dict.dct");
            //_ChsSingleLastTime = GetLastTime(Dict.ChsName.ChsSingleNameFileName);
            //_ChsName1LastTime = GetLastTime(Dict.ChsName.ChsDoubleName1FileName);
            //_ChsName2LastTime = GetLastTime(Dict.ChsName.ChsDoubleName2FileName);
            //_StopWordLastTime = GetLastTime("Stopword.txt");
            //_SynonymLastTime = GetLastTime(Dict.Synonym.SynonymFileName);
            //_WildcardLastTime = GetLastTime(Dict.Wildcard.WildcardFileName);
            //MonitorDictionary();
            //_Thread = new Thread(MonitorDictionary);
            //_Thread.IsBackground = true;
            //_Thread.Start();
        }

        private bool MainDictChanged()
        {
            try
            {
                return _MainDictLastTime != GetLastTime("Dict.dct");
            }
            catch
            {
                return false;
            }
        }

        private bool ChsNameChanged()
        {
            try
            {
                return (_ChsSingleLastTime != GetLastTime(Dict.ChsName.ChsSingleNameFileName) ||
                    _ChsName1LastTime != GetLastTime(Dict.ChsName.ChsDoubleName1FileName) ||
                    _ChsName2LastTime != GetLastTime(Dict.ChsName.ChsDoubleName2FileName));
            }
            catch
            {
                return false;
            }
        }

        private bool StopWordChanged()
        {
            try
            {
                return _StopWordLastTime != GetLastTime("Stopword.txt");
            }
            catch
            {
                return false;
            }
        }

        private bool SynonymChanged()
        {
            try
            {
                return _SynonymLastTime != GetLastTime(Dict.Synonym.SynonymFileName);
            }
            catch
            {
                return false;
            }
        }

        private bool WildcardChanged()
        {
            try
            {
                return _WildcardLastTime != GetLastTime(Dict.Wildcard.WildcardFileName);
            }
            catch
            {
                return false;
            }

        }


        private void MonitorDictionary()
        {
           
        }
    }
}
