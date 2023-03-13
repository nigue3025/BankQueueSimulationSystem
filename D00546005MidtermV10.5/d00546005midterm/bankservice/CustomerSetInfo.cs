using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomVariateGenerator;

namespace BankService
{
    public class CustomerSetInfo
    {
        public RandomVariableGenerator ArrivalRvType;
        private bool hasClosedTime = false;

        public bool HasClosedTime
        {
            get { return hasClosedTime; }
            set { hasClosedTime = value; }
        }
     
        private int customerNo = 50;
        private double finalArrivalTime = 1000;

        public double FinalArrivalTime
        {
            get { return finalArrivalTime; }
            set { finalArrivalTime = value; }
        }

        public int CustomerNo
        {
            get { return customerNo; }
            set { customerNo = value; }
        }

     

        public CustomerSetInfo()
        {

        }

       
       

    }
}
