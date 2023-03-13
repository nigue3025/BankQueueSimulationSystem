using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomVariateGenerator;

namespace BankService
{
    public class Customer:GraphicalElement
    {
        private double arrivalTime;
      
        public double ArrivalTime
        {
            get { return arrivalTime; }
     
        }
        


        public int ID;
        public double enterTellerTime=0.0;

        public double WaitingTime
        {
            get
            {
                return (enterTellerTime-arrivalTime);

            }

        }

       


        public Customer()
        {
      
         
        }

        public Customer(Random ColorRnd):base (ColorRnd)
        {
            //staticRnd = ColorRnd;

        }


        public double LeftTime
        {
            get
            {
                return serviceDoneTime;
            }
          
        }
        /// <summary>
        /// Generate the customer List
        /// </summary>
        /// <param name="customerNo">total customer number</param>
        /// <param name="ArrivalTimeRV">R.V. of the customer arrival time</param>
        /// <returns></returns>
        public static List<Customer> generateCustomerList(int customerNo,RandomVariableGenerator ArrivalTimeRV,CustomerSetInfo theCustomersInfo)
        {
            List<Customer> theCustomerList = new List<Customer>();
            double cumulatedTime=0;
            Random colorRnd = new Random() ;

            for (int i = 0; i < customerNo; i++)
            {

                Customer theCust = new Customer(colorRnd);
              
              theCustomerList.Add(theCust);
              
              theCustomerList[i].arrivalTime=cumulatedTime;
              theCustomerList[i].ID = i;
               
              double generatedValue = ArrivalTimeRV.getASampleData();
              
              while(generatedValue<0)
                  generatedValue = ArrivalTimeRV.getASampleData();

              cumulatedTime += generatedValue;       
       
                 if (theCustomersInfo.HasClosedTime == true && cumulatedTime>theCustomersInfo.FinalArrivalTime )
                     break;

            }

            return theCustomerList;
        }

        public static ArrivalEvent generateCustomerArrivalEvent(Customer theCustomer,Bank theBankManager,TimeSortedEventQueue theEventList)
        {

            return new ArrivalEvent(theCustomer, theBankManager, theCustomer.ArrivalTime,theEventList);

        }



       

    
        public double serviceDoneTime;
    }
}
