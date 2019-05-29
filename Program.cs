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
using USSD.Controller;
using System.Threading;
using USSD.Help;
namespace USSD
{
    class Program
    {
        #region ConfigurationRoutingSettingsProperty
        static ModelSlot[] gsm1Example;
        static ModelSlot[] gsm2Example;
        
        
        #endregion
    
        static void Main(string[] args)
        {
           
            RouterManager.FILE_PATH = HelpClass.LoadPathConfig("config.log");
            var timer = 30000;
            InitializeConfigRouter();
            if (args.Length > 0)
            {
                int.TryParse(args[0], out timer);
            }
            RouterManager.ArgumentTimeOut = timer;
            
            #region Ярославль            
                //! Сделать загрузку данный роутеров из файла. Пример josnModel.json
                ConcreteRouter ExampleGSM1 = new ConcreteRouter("Router01","Exm1","GSM1");
                PiterGSM1.AddSlots(gsm1Example);
                ConcreteRouter ExampleGSM2 = new ConcreteRouter("Router02","Exm2","GSM2");
                PiterGSM2.AddSlots(gsm2Example);
                RouterManager router01 = new RouterManager("http://192.168.0.1", "admin", "password", ExampleGSM1);
                RouterManager router02 = new RouterManager("http://192.168.0.2", "admin", "password", ExampleGSM2);   
            #endregion

            RouterManager[] routers = new RouterManager[] {
                router01,
                router02 
            };

            TheadManager theadManager = new TheadManager(routers);
            theadManager.Run();


            // Thread timeClear = new Thread(new ParameterizedThreadStart(HelpClass.ClearConsole));
            // timeClear.Start(timer) ;
        }

         public static void InitializeConfigRouter()
        {
            #region Ярославль
            gsm1Example = new ModelSlot[] {
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line1),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line2),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line3),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line4),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line5),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line6),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line7),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line8),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line9),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line10),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line11),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line12),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line13),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line14),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line15),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line16),

                new ModelSlot(MobileOperator.MTC, SlotNumber.line17),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line18),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line19),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line20),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line21),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line22),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line23),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line24),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line25),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line26),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line27),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line28),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line29),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line30),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line31),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line32),
            };
            gsm2Example = new ModelSlot[] {
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line1),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line2),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line3),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line4),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line5),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line6),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line7),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line8),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line9),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line10),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line11),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line12),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line13),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line14),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line15),
                new ModelSlot(MobileOperator.Megafon, SlotNumber.line16),

                new ModelSlot(MobileOperator.Beeline, SlotNumber.line17),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line18),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line19),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line20),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line21),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line22),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line23),
                new ModelSlot(MobileOperator.Beeline, SlotNumber.line24),

                new ModelSlot(MobileOperator.MTC, SlotNumber.line25),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line26),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line27),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line28),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line29),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line30),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line31),
                new ModelSlot(MobileOperator.MTC, SlotNumber.line32),
            };
            #endregion
        }
    }
}