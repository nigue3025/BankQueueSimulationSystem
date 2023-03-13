using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankService
{

    public class ServiceDoneEvent : DiscreteEvent
    {
        BankTeller myBankTeller;

        public ServiceDoneEvent(BankTeller theBankTeller,double theEventTime,TimeSortedEventQueue theEventlist)
        {
            myBankTeller = theBankTeller;
            EventTime = theEventTime;
            myEventList = theEventlist;
        }

        public int TellerID
        {
            get
            {
                return myBankTeller.TellerID;
            }
           
        }


        public override void ProcessEvent()
        {
            
            
            //趕走這個櫃台已經被服務完的客人
            //若queue內還有客人,該程式碼會自動服務queue內排頭的客人,Teller Status=busy
            //若queue內沒客人了,Teller status=idle
            myBankTeller.expellACustomer();
            
            
            if (myBankTeller.Status=="busy")
            //若Server內還有客人
            {                
               
                EventTime = myBankTeller.NextServiceDoneTime;
                //產生一個新的Service Down事件插入event List中
                myEventList.insert(new ServiceDoneEvent(myBankTeller,EventTime,myEventList));    
                
            }
           

            //把Event list排頭 Service Down事件刪掉
           myEventList.RemoveHeader();
        }
    }
}
