using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Segment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Business
{

    /// <summary>
    /// Segment service
    /// </summary>
    public class SegmentUtil
    {
        private LexisNexis.Red.Common.Segment.ISegment segment;
        private static HashSet<String> reserveWords = new HashSet<string>();

        public static HashSet<String> RESERVE_WORDS_FOR_SQLITE
        {
            get { return SegmentUtil.reserveWords; }
            set { SegmentUtil.reserveWords = value; }
        }

        public LexisNexis.Red.Common.Segment.ISegment Segment
        {
            get { return segment; }
            set { segment = value; }
        }

        private static readonly SegmentUtil instance;

        static SegmentUtil()
        {
            instance = new SegmentUtil();
        }

        private SegmentUtil()
        {
            IoCContainer.Instance.RegisterInterface<ISegment, SimpleSegment>();
            Segment = IoCContainer.Instance.ResolveInterface<ISegment>();
        }

        public static SegmentUtil Instance
        {
            get
            {
                return instance;
            }
        }


        public async Task Prepare()
        {
            await Segment.Init();
        }

        /// <summary>
        /// Words Tokenization
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<String> PhraseSegment(string text)
        {
            List<string> result = new List<string>();
            try
            {
                List<String> words = Segment.Analyse(text);
                int groupNumber = words.Count / Constants.MAX_WORD_NUMBER;
                int lastGroupWordNumber = words.Count % Constants.MAX_WORD_NUMBER;

                for(int i = 0; i <= groupNumber; i++)
                {
                    int takeNumber = 0;
                    if (i == groupNumber)
                        takeNumber = lastGroupWordNumber;
                    else
                        takeNumber = Constants.MAX_WORD_NUMBER;

                    string resultText = String.Empty;
                    foreach (String wordInfo in words.Skip(Constants.MAX_WORD_NUMBER * i).Take(takeNumber))
                    {
                        if (!String.IsNullOrEmpty(wordInfo.Trim()) && !RESERVE_WORDS_FOR_SQLITE.Contains(wordInfo.ToLower()))
                        {
                            resultText = String.IsNullOrEmpty(resultText) ? wordInfo : String.Join(Constants.NEAR_SEPERATOR, resultText, wordInfo);
                        }
                    }

                    if (!String.IsNullOrEmpty(resultText.Trim()))
                        result.Add(resultText);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("PhraseSegment" + ex.ToString());
                return result;
            }
        }
    }
}
