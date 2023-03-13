using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankService
{
    public class ArrivalEvent : DiscreteEvent
    {
        Bank myBankManager;
        Customer myCustomer;
       

        public ArrivalEvent(Customer theCustomer,Bank theBankManager,double EventTime,TimeSortedEventQueue theEventList)
        {
            myCustomer = theCustomer;
            myBankManager = theBankManager;
            this.EventTime = EventTime;
            myEventList = theEventList;
            myColor = theCustomer.myColor;
        }

        public int CustomerID
        {
            get
            {
                return myCustomer.ID;
            }
           
        }

        public override void ProcessEvent()
        {
            
                
            if(!myBankManager.IsAllTellerBusy)
            //當有櫃檯(teller)在idle時
            {
                //給Bank Manager選一個idle的櫃台服務剛進門的客人
                myBankManager.serveACustomer(myCustomer);
                EventTime = myCustomer.serviceDoneTime;
                
                //產生一個該客人服務結束的事件,並插入event List中
                ServiceDoneEvent theServiceDoneEvent = new ServiceDoneEvent(myBankManager.CurrentSelectedBankTeller,EventTime,myEventList);
                myEventList.insert(theServiceDoneEvent);
            }
            else
            //當所有的櫃檯(teller)都在忙
                myBankManager.escortACustomer(myCustomer);
           
            //刪掉event list排頭事件(Customer arrival)
            myEventList.RemoveHeader();
        }
    }
}
