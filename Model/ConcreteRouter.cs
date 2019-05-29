using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace USSD.Model
{
    public class ConcreteRouter
    {
        public List<ModelSlot> ModelSlots;
        public string LabelRouter{get;}
        public string LabeGSM{get;}
        public string ServerOktell{get;}

        public ConcreteRouter(string serverOktell, string labelRouter = "", string labelGSM = "")
        {
           ModelSlots = new List<ModelSlot>(32);
           ServerOktell = serverOktell;
           LabelRouter = labelRouter;
           LabeGSM = labelGSM;
        }
        public void AddSlot(ModelSlot modelSlots)
        {
             ModelSlots.Add(modelSlots);
        }

        public void AddSlots(ModelSlot[] modelSlots)
        {
            ModelSlots.AddRange(modelSlots);
        }

        public ModelSlot GetSimCard(string idSlot)
        {
            Regex regex = new Regex("Id([0-9]+)");
            var collections = regex.Match(idSlot).Groups[1];
            return ModelSlots
                            .Where((t) => t.SlotNumber == (SlotNumber)Enum.Parse(typeof(SlotNumber), $"line{collections}"))
                            .Select((s) => new ModelSlot(s.MobileOperator,s.SlotNumber))
                            .First();

            //string result = Regex.IsMatch
        }

        public bool ContainsOperator(MobileOperator mobileOperator)
        {
            var result = ModelSlots.Where((t) => t.MobileOperator == mobileOperator).FirstOrDefault();
            if (result != null)
                return true;
            return false;
        }
    }
}