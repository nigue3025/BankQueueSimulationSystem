using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomVariateGenerator;
namespace BankService
{

    /// <summary>
    /// Manage a collection of Bank Tellers
    /// Service Type: Each teller owns a queue. Each queue corresponds to a teller(a teller<=>a queue)
    /// Warning: Service type that sharing a queue with Mutliple-teller DOES NOT IMPLEMENT HERE!!! 
    /// Main function:Given a sets of tellers, assign a teller or queue for a newly arrived customer   
    /// </summary>
    public class Bank
    {
        private List<BankTeller> TheBankTellerList;
        List<Customer> TheCustomerArrivalList;
        public List<Customer> TheCustomerExitList;
        private BankTeller currentSelectedBankTeller;
        enumBankTellersQueueBehavior myServerQueueType;
        private List<FcfsTimedQueue<Customer>> theDistinctQueueList;

        public List<FcfsTimedQueue<Customer>> TheDistinctQueueList
        {
            get {
           
                    
                     List<FcfsTimedQueue<Customer>> tempList=new List<FcfsTimedQueue<Customer>>();
                    tempList.Add(TheBankTellerList[0].ReferredQueue);

                    for (int i = 1; i < TheBankTellerList.Count; i++)
                       for(int j=0;j<tempList.Count;j++)
                       {
                           if (TheBankTellerList[i].ReferredQueue == tempList[j])
                               break;

                           if(j==tempList.Count-1)tempList.Add(TheBankTellerList[i].ReferredQueue);
                       }

                    return tempList;

             
                
               
            
               }
        
        }




        public enumBankTellersQueueBehavior SetMyServerQueueType
        {
  
            set { myServerQueueType = value; }
        }



    

        public BankTeller CurrentSelectedBankTeller
        {
            get { return currentSelectedBankTeller; }
          
        }

        public Bank(List<BankTeller> TheBankTellerList, List<Customer>TheCustomerArrivalList,enumBankTellersQueueBehavior theBankTellerQueueBehavior)
        {
            this.TheBankTellerList = TheBankTellerList;
            this.TheCustomerArrivalList = TheCustomerArrivalList;
            myServerQueueType = theBankTellerQueueBehavior;
            TheCustomerExitList = new List<Customer>();

            for (int i = 0; i < TheBankTellerList.Count; i++)
                TheBankTellerList[i].TheExitCustomerList = TheCustomerExitList;
        
        }


        //for graphic model creation
        public Bank()
        {


        }


        public double SytemTerminationTime
        {
            get
            {
                if (TheCustomerExitList.Count != 0)
                    return TheCustomerExitList[TheCustomerExitList.Count - 1].LeftTime;
                else
                    return this.MinServiceDoneTellerTime;
            }

        }

      
        /// <summary>
        /// Find the index of an idle teller
        /// </summary>
        public int AnIdleTellerIndex
        {
            get
            {
                return getAnIdleTellerIndex();
            }
      
        }

        /// <summary>
        /// If all the teller are busy, return true
        /// </summary>
        public bool IsAllTellerBusy
        {
            get
            {
                for (int i = 0; i < TheBankTellerList.Count; i++)
                    if (TheBankTellerList[i].Status == "idle")
                        return false;
              
                return true;
            }
         
        }

        public int MinServiceDoneTellerTimeIndex
        {
            get
            {
                return getMinTimeTellerIndex();
            }
         
        }
      
        public double MinServiceDoneTellerTime
        {
            get
            {
              
                double tempMin = double.MaxValue;
                for (int i = 0; i < TheBankTellerList.Count; i++)
                    if (TheBankTellerList[i].NextServiceDoneTime < tempMin)
                        tempMin = TheBankTellerList[i].NextServiceDoneTime;
                return tempMin;

            }
          
        }




        private int getAnIdleTellerIndex()
        {
            int theIndex = -1;
            for (int i = 0; i < TheBankTellerList.Count; i++)
                if (TheBankTellerList[i].Status == "idle")
                {
                    theIndex = i;
                    break;
                }
            return theIndex;
          
        }

        /// <summary>
        /// Find the index from the minimum of all the teller based on service done time 
        /// </summary>
        /// <returns></returns>
        private int getMinTimeTellerIndex()
        {
            double tempMin = double.MaxValue;
            

            //return的index為-1時會當掉,提醒程式有誤
            int tempMinIndex = -1;

            if (TheBankTellerList[0].Status == "busy")
            {
                tempMin = TheBankTellerList[0].NextServiceDoneTime;
                tempMinIndex = 0;
            }

            //若所有Min service done time 有兩組, ID比較前面的Teller(server)優先回傳,非隨機選曲的方式
            for(int i=0;i<TheBankTellerList.Count;i++)
                if (TheBankTellerList[i].NextServiceDoneTime < tempMin && TheBankTellerList[i].Status=="busy")
                {
                    tempMin = TheBankTellerList[i].NextServiceDoneTime;
                    tempMinIndex = i;
                }

            return tempMinIndex;
        }


        /// <summary>
        /// Return a index of the teller with the minimum queue length
        /// </summary>
        /// <returns></returns>
        private int getMinQueueLengthTellerIndex()
        {
            int tempMin = TheBankTellerList[0].CurrentQueueLength;
            int tempMinIndex = 0;

            //若min queue length the server 有兩組一上,優先回傳ID 較前面的 server 之 index
            for (int i = 0; i < TheBankTellerList.Count; i++)
                if (TheBankTellerList[i].CurrentQueueLength < tempMin)
                {
                    tempMinIndex = i;
                    tempMin = TheBankTellerList[i].CurrentQueueLength;
                }

            return tempMinIndex;
        }



        /// <summary>
        /// Enqueue(line up) the customer when all the server is busy
        /// </summary>
        /// <param name="aCustomer"></param>
        public void escortACustomer(Customer aCustomer)
        {
            currentTime = aCustomer.ArrivalTime;

          



            //Put the customer into the Smallest size ServerQueue 
            currentSelectedBankTeller = TheBankTellerList[MinQueueLengthServerIndex];
            currentSelectedBankTeller.escortACustomer(aCustomer);

            
        }


        /// <summary>
        /// Make the customer leave the teller
        /// </summary>
        public void expellACustomer()
        {
            
      
            //Expell the customer from the busy teller which has the exactly min service done time
            currentSelectedBankTeller = TheBankTellerList[MinBusyServiceDoneTellerTimeIndex];
            currentSelectedBankTeller.expellACustomer();
            
            //update system current time
            currentTime = TheCustomerExitList[TheCustomerExitList.Count - 1].serviceDoneTime;
            
        }

        /// <summary>
        /// put the customer to the teller(server) directly
        /// </summary>
        /// <param name="aCustomer">the customer to be served</param>
        public void serveACustomer(Customer aCustomer)
        {
            currentTime = aCustomer.ArrivalTime;

            // Tell an idle Teller(Server) to serve the customer arrived
            currentSelectedBankTeller = TheBankTellerList[AnIdleTellerIndex];
            currentSelectedBankTeller.serveACustomer(aCustomer);

        }

        /// <summary>
        /// Make sure the arrival of the customer has the priotiy to enter the System
        /// </summary>
        public bool HasArrivalPriority
        {
            get
            {

                if (TheCustomerArrivalList.Count != 0 && !HasExpellPriority)
                {    
                    return true;
                }
                else
                    return false;

            }

        }

        /// <summary>
        /// If all the server are idle, return true
        /// </summary>
        public bool IsAllTellerIdle
        {
            get
            {
                for (int i = 0; i < TheBankTellerList.Count; i++)
                    if (TheBankTellerList[i].Status=="busy")
                        return false;

                return true;
            }

        }


        public double MinBusyServiceDoneTellerTime
        {
            get
            {
                return getMinBusyServiceDoneTellerTime();
            }

        }
        double getMinBusyServiceDoneTellerTime()
        {

            double tempMin = double.MaxValue;
            for (int i = 0; i < TheBankTellerList.Count; i++)
                if (TheBankTellerList[i].Status=="busy" && TheBankTellerList[i].NextServiceDoneTime < tempMin)
                    tempMin = TheBankTellerList[i].NextServiceDoneTime;
            return tempMin;

        }

        int getMinBusyServiceDoneTellerTimeIndex()
        {
            double tempMin = double.MaxValue;
            int index = 0;
            for (int i = 0; i < TheBankTellerList.Count; i++)
                if (TheBankTellerList[i].Status == "busy" && TheBankTellerList[i].NextServiceDoneTime < tempMin)
                {
                    tempMin = TheBankTellerList[i].NextServiceDoneTime;
                    index = i;
                }
            return index;

        }
        public int MinBusyServiceDoneTellerTimeIndex
        {
            get
            {
                return getMinBusyServiceDoneTellerTimeIndex();
            }
        }


     
      


        /// <summary>
        /// Make sure if the teller(server) has priority to expell the customer
        /// </summary>
        public bool HasExpellPriority
        {

            get
            {

                //若仍有櫃台運作且顧客arrival事件皆已結束=>Expell優先               
                if (IsAllTellerIdle == false)                
                  if (TheCustomerArrivalList.Count == 0)
                    return true;
       


                //若expell發生的時間點早發生於於顧客Arrival時間==>expell優先
                if (TheCustomerArrivalList.Count != 0)
                    if (MinBusyServiceDoneTellerTime <= TheCustomerArrivalList[0].ArrivalTime)
                        return true;


                //上述二條件若無法達成=> expell非優先(has expell Priotiy == false)
                return false;
                

            }
        }



     

        public double CurrentTime
        {
            get
            {
                return currentTime;
            }
          
        }

        private double currentTime;

        public int MinQueueLengthServerIndex
        {
            get
            {
                return getMinQueueLengthTellerIndex();
            }
            
        }
    }
}
