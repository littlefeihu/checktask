using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.DomainService.AnnotationSync;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using LexisNexis.Red.Common.Services;

namespace LexisNexis.Red.CommonTests.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String DictionarySQLitePassword
        {
            get { return "$bu97aIcLRmc9#Ak1&*IVr#^eTh9rFloB@YiMRtf7%l@56RLL8"; }
        }

        public String DictionarySQLitePath
        {
            get { return @"Data Source=C:\DictionaryRed\dictionary.data;Password=" + DictionarySQLitePassword + ";"; }
        }

        public MainWindow()
        {
            InitializeComponent();
            //string dd = null;
            //FileStream fs = new FileStream(@"C:\Allen.Xie\Job\Projects\Red\LexisNexis.Red.CommonTests.WPF\mc_04_iv10001000.xml", FileMode.Open);
            //using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("iso-8859-1")))
            //{

            //    dd = sr.ReadToEnd();
            //    System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
            //    System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;
            //    // Convert it to iso-8859-1 bytes
            //    byte[] isoBytes = iso_8859_1.GetBytes(dd);
            //    // Convert the bytes to utf-8:
            //    byte[] utf8Bytes = System.Text.Encoding.Convert(iso_8859_1, utf_8, isoBytes);
            //    dd = utf_8.GetString(utf8Bytes);
            //}


            //Console.WriteLine(dd);



        }
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationTokenSource source1 = new CancellationTokenSource();
        CancellationTokenSource source2 = new CancellationTokenSource();



        int bookId = 46;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 3;
            Logger.Log("ds");
            //var idnexContent = await PublicationContentUtil.Instance.GetContentFromIndex(bookId, new Index { FileName = "AB1.xml", Title = "AB1" });
            var loginResult = await LoginUtil.Instance.ValidateUserLogin("admin", "1111", "AUNZ");

            IoCContainer.Instance.Resolve<IDeliveryService>().UploadRepair(new Common.Entity.UploadRepairRequest
            {
                username = "",
                userid = "",
                faultDesc = ""
            });


            return;
            //  var rows = AnnCategoryTagUtil.Instance.AddTag("tag", "#FAD788");
            Console.WriteLine(loginResult);
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            if (tags.Count > 0)
                AnnCategoryTagUtil.Instance.DeleteTag(tags[0].TagId);
            Console.WriteLine(tags.Count);

            //var loginResult = await LoginUtil.Instance.ValidateUserLogin("nikitta@lexisred.com", "123465", "AU");
            LoginUtil.Instance.GetLastIsAliveLoginUser();

            //var loginResult = await LoginUtil.Instance.ValidateUserLogin("allen@red.com", "1234", "HK");
            PublicationUtil.Instance.RecenthistoryChanged = () =>
            {
                var recenthistorys = PublicationContentUtil.Instance.GetRecentHistory();
                if (recenthistorys != null)
                    Console.WriteLine(recenthistorys.Count);
            };
            var dd = await PublicationUtil.Instance.GetPublicationOnline();
            Console.WriteLine(dd.RequestStatus);
            //  var tocid = await PublicationContentUtil.Instance.GetTOCIDByDocId(6, "ACLL_CREG_SCH.SGM_REPRO-TX_1");
            // Console.WriteLine(tocid);
            //var dd = GlobalAccess.Instance.CurrentUserInfo;
            List<Task> taskList = new List<Task>();

            var downLoadResult = PublicationUtil.Instance.DownloadPublicationByBookId(bookId, source.Token, (x, y) =>
            {
                Lable1.Content = x;
            }, false);

            taskList.Add(downLoadResult);
            //var downLoadResult1 = PublicationUtil.Instance.DownloadPublicationByBookId(36, source1.Token, (x, y) =>
            //{
            //    Lable2.Content = x;
            //}, false);
            //taskList.Add(downLoadResult1);

            //var downLoadResult2 = PublicationUtil.Instance.DownloadPublicationByBookId(41, source2.Token, (x, y) =>
            //{
            //    Lable3.Content = x;
            //}, false);
            //taskList.Add(downLoadResult2);
            await Task.WhenAll(taskList);
            // Console.WriteLine("ds" + downLoadResult1.Result.DownloadStatus + downLoadResult2.Result.DownloadStatus);
            Console.WriteLine("dsa");
            //var ispbo = await PageSearchUtil.Instance.IsPBO(bookId);
            //if (ispbo)
            //{
            //    var pageSearchItems = await PageSearchUtil.Instance.SeachByPageNum(bookId, 1);
            //    if (pageSearchItems != null)
            //    {
            //        var pageItems = await PageSearchUtil.Instance.GetPagesByTOCID(bookId, pageSearchItems.FirstOrDefault().TOCID);
            //        Console.WriteLine(pageItems.Count);
            //    }
            //}

            Stopwatch watch = new Stopwatch();
            watch.Start();
            var tocNode = await PublicationUtil.Instance.GetDlBookTOC(bookId);
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
            watch.Start();
            var tocs = await PublicationContentUtil.Instance.GetTOCListByBookId(bookId);
            watch.Stop();
            Console.WriteLine(watch.Elapsed);

            var targetTOC = PublicationContentUtil.Instance.GetTOCByTOCId(2565, tocNode);
            var list = await PublicationContentUtil.Instance.GetContentFromTOC(bookId, tocNode.GetFirstPage());
            var recentHistorys = PublicationContentUtil.Instance.GetRecentHistory();
            var tocID = await PublicationContentUtil.Instance.GetTOCIDByDocId(bookId, recentHistorys[0].DOCID);

            //string list = "";
            //FileStream fs = new FileStream(@"C:\Allen.Xie\Job\Projects\Red\LexisNexis.Red.CommonTests.WPF\mc_04_iv10001003.htm", FileMode.Open);
            //using (StreamReader sr = new StreamReader(fs))
            //{
            //    list = sr.ReadToEnd();
            //}
            StringBuilder s = new StringBuilder();
            s.Append("<html><head> <meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\"></head><body>");

            s.Append(list);
            s.Append("</body></html>");

            // ConvertExtendedASCII("aaaaaaaa中文")

            MessageBox.Show(s.ToString());
        }
        private static string ConvertExtendedASCII(string HTML)
        {
            string retVal = "";
            char[] s = HTML.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var downLoadResult2 = await PublicationUtil.Instance.DownloadPublicationByBookId(bookId, source2.Token, (x, y) =>
            {
                Lable3.Content = x;
            }, false);
            var recentHistorys = PublicationContentUtil.Instance.GetRecentHistory();
            var tocID = await PublicationContentUtil.Instance.GetTOCIDByDocId(bookId, recentHistorys[0].DOCID);
            var tocNode = await PublicationUtil.Instance.GetDlBookTOC(bookId);
            var targetTOC = PublicationContentUtil.Instance.GetTOCByTOCId(tocID, tocNode);
            var list = await PublicationContentUtil.Instance.GetContentFromTOC(bookId, targetTOC);
            MessageBox.Show(list);
            //source.Cancel();
            //try
            //{
            //    using (SQLiteConnection connection = new SQLiteConnection(DictionarySQLitePath))
            //    {
            //        try
            //        {
            //            connection.Open();

            //            //Fetch term keyword
            //            SQLiteCommand command = new SQLiteCommand(connection);
            //            command.CommandText = "SELECT zterm FROM zterm WHERE z_pk = @SearchTermId";
            //            command.Parameters.Add(new SQLiteParameter("@SearchTermId", "orders"));

            //            SQLiteDataReader sqReader = command.ExecuteReader();

            //            try
            //            {
            //                while (sqReader.Read())
            //                {
            //                }

            //            }
            //            finally
            //            {
            //                connection.Close();

            //                sqReader.Dispose();
            //                command.Dispose();
            //                connection.Dispose();

            //                GC.Collect();
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            throw;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

        }

        private async void Button_Download_DictionaryClick(object sender, RoutedEventArgs e)
        {
            var dd = await PublicationUtil.Instance.GetPublicationOnline();
        }

        private async void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            var loginResult = await LoginUtil.Instance.ValidateUserLogin("allen@lexisred.com", "1234", "AU");
            LoginUtil.Instance.GetLastIsAliveLoginUser();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            source1.Cancel();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            source2.Cancel();
        }
    }
}
