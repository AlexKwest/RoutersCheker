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
    public class TheadManager
    {
        RouterManager[] routers;
        public TheadManager(RouterManager[] routers)
        {
            this.routers = routers;
        }

        public void Run()
        {
            foreach (var router in routers)
            {
                if (HelpClass.isCheckInternetConection(router.RouterAddress) == true)//("http://ya.ru") == true)//
                {
                    Thread threadRouter = new Thread(router.MonitoringRouter);
                    threadRouter.Start();
                }
                else
                {
                    System.Console.WriteLine($"Шлюз {router.RouterAddress} {router.ConfigRouter.LabelRouter}{router.ConfigRouter.LabeGSM} недоступен");
                }
            }
        }
    }
}