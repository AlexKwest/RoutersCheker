using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Serialization;
using System.Threading.Tasks;
using USSD.Model;
using USSD.Help;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace USSD.Controller
{
    public class RouterManager
    {
        public static string FILE_PATH; //Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        public static int ArgumentTimeOut { get; set; }
        static object locker = new object();
        public ConcreteRouter ConfigRouter { get; set; }
        public ModelXML ussdClass { get; set; }
        private ConcurrentDictionary<MobileOperator, string> postDatas { get; set; }
        public string RouterAddress { get; set; }
        private string Login { get; set; }
        private string Password { get; set; }

        public RouterManager(string routerAddress, string login, string password, ConcreteRouter configRouter)
        {
            postDatas = new ConcurrentDictionary<MobileOperator, string>();
            RouterAddress = routerAddress;
            Login = login;
            Password = password;
            ConfigRouter = configRouter;
        }

        

        public void MonitoringRouter()
        {
            while (true)
            {   
                // lock (locker)
                // {
                AddUssdCommandAllMobileProvider();
                Thread.Sleep(20000);
                if (GetModelXML() == true)
                {
                    Show();
                }
                postDatas.Clear();
                Thread.Sleep(ArgumentTimeOut);
                // }

            }
        }
        private void AddUssdCommandAllMobileProvider()
        {
            PostString(MobileOperator.MTC);
            PostString(MobileOperator.Megafon);
            PostString(MobileOperator.Tele2);
            PostString(MobileOperator.Beeline);

            foreach (var t in postDatas)
            {
                PostUssdCommandRouter(t.Value);
            }
        }
        private void PostString(MobileOperator mobileOperator)
        {
            var postData = "";
            var result = ConfigRouter.ModelSlots
            .Where((t) => t.MobileOperator == mobileOperator);

            if (ConfigRouter.ContainsOperator(mobileOperator))
            {
                foreach (var t in result)
                {
                    postData += $"{t.SlotNumber.ToString()}=1&";
                }
                var resultUSSD = result.Select((t) => t.CommandUSSD).First().ToString();

                postData += $"smskey={HelpClass.UnixTimeNow()}&action=USSD&telnum={resultUSSD}&send=Send";
                postDatas.TryAdd(mobileOperator, postData);
            }
        }

        private bool PostUssdCommandRouter(string postDataResult)
        {
            lock (locker)
            {
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(Login + ":" + Password));

                var request = (HttpWebRequest)WebRequest.Create($"{RouterAddress}/default/en_US/ussd_info.html");
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.Headers.Add("Cache-Control", "max-age=0");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ProtocolVersion = HttpVersion.Version11;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.119 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Referer = $"{RouterAddress}/default/en_US/tools.html?type=ussd";

                var data = Encoding.ASCII.GetBytes(postDataResult);
                request.ContentLength = data.Length;

                try
                {
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                     var response = (HttpWebResponse)request.GetResponse();
                     return true;
                }
                catch(WebException ex)
                {
                    ErrorWebStatus(ex);
                    return false;
                }
            }
        }

        private bool GetModelXML()
        {
            lock (locker)
            {
                    WebRequest request = WebRequest.Create($"{RouterAddress}/default/en_US/send_status.xml?u={Login}&p={Password}");
                    XmlSerializer serializer = new XmlSerializer(typeof(ModelXML));
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                             using (StreamReader reader = new StreamReader(stream))
                             {
                                 ussdClass = (ModelXML)serializer.Deserialize(reader);
                                 
                             }
                        }
                    }
                    return true;
                }
                catch (WebException)
                {
                    return false;
                }
            }
        }

        private void ErrorWebStatus(WebException ex)
        {
            WebExceptionStatus status = ex.Status;

            if (status == WebExceptionStatus.ProtocolError)
            {
                System.Console.WriteLine(new String('-', 20));
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    Console.WriteLine($"Проект {ConfigRouter.LabelRouter}. IP:{RouterAddress}");
                    Console.WriteLine($"Код ошибки:{(int)httpResponse.StatusCode} {httpResponse.StatusCode}");
                System.Console.WriteLine(new String('-', 20));
            }
        }

        public void Show()
        {
            lock (locker)
            {
                var pathDirectory = HelpClass.CreateDirectory(FILE_PATH, Path.Combine($"Шлюз\\{DateTime.Now.ToString("yyyy.MM.dd")}",$"{DateTime.Now.ToString("HH")} час",ConfigRouter.ServerOktell));
                var modelSlot = new ModelSlot();
                using (FileStream ostrm = new FileStream($"{pathDirectory}\\{ConfigRouter.LabelRouter}_{ConfigRouter.LabeGSM}.txt", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(ostrm))
                    {
                        //TextWriter oldOut = Console.Out;

                        #region OutputFormat
                        writer.WriteLine("****************************");
                        writer.WriteLine($"Проект {ConfigRouter.LabelRouter}. IP:{RouterAddress}");
                        writer.WriteLine("****************************");
                        writer.WriteLine($"Время обращения к симкартам {DateTime.Now}");
                        writer.WriteLine("----------------------------------------------------------");
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error1))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id1));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error1)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error2))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id2));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error2)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error3))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id3));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error3)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error4))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id4));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error4)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error5))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id5));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error5)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error6))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id6));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error6)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error7))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id7));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error7)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error8))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id8));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error8)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error9))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id9));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error9)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error10))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id10));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error10)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error11))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id11));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error11)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error12))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id12));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error12)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error13))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id13));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error13)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error14))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id14));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error14)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error15))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id15));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error15)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error16))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id16));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error16)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error17))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id17));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error17)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error18))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id18));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error18)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error19))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id19));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error19)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error20))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id20));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error20)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error21))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id21));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error21)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error22))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id22));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error22)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error23))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id23));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error23)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error24))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id24));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error24)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error25))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id25));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error25)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error26))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id26));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error26)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error27))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id27));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error27)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error28))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id28));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error28)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error29))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id29));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error29)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error30))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id30));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error30)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error31))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id31));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error31)}");
                            writer.WriteLine("-------");
                        }
                        if (!String.IsNullOrWhiteSpace(ussdClass.Error32))
                        {
                            modelSlot = ConfigRouter.GetSimCard(nameof(ussdClass.Id32));
                            writer.WriteLine($"Оператор - {modelSlot.MobileOperator}; Cлот - {modelSlot.SlotNumber}; Время - {HelpClass.DateTimeFromUnixTime(Convert.ToInt32(ussdClass.Id1, 16)).ToString()}");
                            writer.WriteLine($"Вернул - {HelpClass.RegexResponse(ussdClass.Error32)}");
                        }
                        writer.WriteLine("----------------------------------------------------------");
                        #endregion
                    }
                }
                System.Console.WriteLine($"Роутер: <{ConfigRouter.LabelRouter} {ConfigRouter.LabeGSM}> IP: {RouterAddress}. Время: {DateTime.Now}. Файл результатов: {pathDirectory}.");
            }
        }
    }
}