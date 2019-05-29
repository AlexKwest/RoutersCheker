using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Net.NetworkInformation;

namespace USSD.Help 
{
    public static class HelpClass
    {
        public static string UnixTimeNow()
        {
            int unixTime = (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            return Convert.ToString(unixTime, 16);
        }

        public static DateTime DateTimeFromUnixTime(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static string RegexResponse(string usssAnswer)
        {
            return usssAnswer;
            // Regex regex = new Regex("([-]?[Минус]?[0-9]+)[,.]?");
            // return regex.Match(usssAnswer).Groups[1].ToString();
        }


        public static string CreateDirectory(string directoryMain, string directorySub)
        {
            string pathDefault = directoryMain;
            string subpath = directorySub;
            DirectoryInfo dirInfo = new DirectoryInfo(pathDefault);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            return dirInfo.CreateSubdirectory(subpath).FullName;  
        }

        public static void ClearConsole(object timer)
        {
            while(true)
            {
                Thread.Sleep((int)timer*2 - 6000);
                Console.Clear();
            }

        }

        public static string LoadPathConfig(string config)
        {
            config = Path.Combine(Environment.CurrentDirectory, config);
            string pathDefault = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string path = "";

            using (var st = new FileStream(config, FileMode.OpenOrCreate))
            {
                using (var sr = new StreamReader(st, Encoding.UTF8))
                {
                    path = sr.ReadLine();
                }
            }
            //TODO: Добавить проверку на валидность path

            if (string.IsNullOrWhiteSpace(path))
            {
                using (var st = new FileStream(config, FileMode.Open))
                {
                    using (var sw = new StreamWriter(st, Encoding.UTF8))
                    {
                        sw.Write(pathDefault);
                    }
                    return pathDefault;
                }
            }
            return path;
        }

        public static bool isCheckInternetConection(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;
            nameOrAddress = nameOrAddress.Substring(7);

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            return pingable;
        }      
    }
}