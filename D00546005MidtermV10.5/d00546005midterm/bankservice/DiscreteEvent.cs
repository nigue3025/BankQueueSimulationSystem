using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankService
{
    public class DiscreteEvent :GraphicalElement
    {
        public double EventTime;
        public TimeSortedEventQueue myEventList;
        public virtual void ProcessEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}
