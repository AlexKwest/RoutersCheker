using System;

namespace USSD.Model
{
    public class ModelSlot
    {
        private MobileOperator mobileOperator;
        public MobileOperator MobileOperator {
            get { 
                return mobileOperator;
                }
            set 
            {
                switch(value)
                {
                    case MobileOperator.Megafon: CommandUSSD = "*100%23"; break;
                    case MobileOperator.MTC: CommandUSSD = "*100%23"; break;
                    case MobileOperator.Tele2: CommandUSSD = "*105*1%23"; break;
                    default: CommandUSSD = "*"; break;
                }
                mobileOperator = value;
            }
        }
        public string CommandUSSD {get; private set;}
        public SlotNumber SlotNumber {get; private set;}
        
        public ModelSlot()
        {
            
        }

        public ModelSlot(MobileOperator mobileOperator, SlotNumber slotNumber)
        {
            MobileOperator = mobileOperator;
            SlotNumber = slotNumber;
        }
    }
    public enum MobileOperator
    {
        MTC,
        Megafon,
        Tele2, 
        Beeline,
        Empty
    }

    public enum SlotNumber
    {
        line1, line2, line3, line4, line5, line6, line7, line8, line9, line10,
        line11, line12, line13, line14, line15, line16, line17, line18, line19, line20,
        line21, line22, line23, line24, line25, line26, line27, line28, line29, line30,
        line31, line32
    }   
}