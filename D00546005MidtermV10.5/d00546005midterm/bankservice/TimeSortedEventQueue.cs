using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankService
{
    public class TimeSortedEventQueue
    {
        List<DiscreteEvent> myEventList;

        public double HeadEventTime
        {
            get
            {

                return myEventList[0].EventTime;
            }

        }

        public int Count
        {
            get
            {
                return myEventList.Count;
            }
        }

        public TimeSortedEventQueue()
        {
            myEventList = new List<DiscreteEvent>();

        }
        public TimeSortedEventQueue(List<DiscreteEvent> theList)
        {
            myEventList = theList;
        }

        public int CurrentSize
        {
            get
            {
                return myEventList.Count;
            }
       
        }

        public DiscreteEvent getEvent(int index)
        {

            return myEventList[index];
        }


        public bool IsWronglySorted
        {
            get
            {
                for (int i = 0; i < myEventList.Count-1; i++)
                    if (myEventList[i].EventTime > myEventList[i + 1].EventTime)
                        return true;

                return false;
            }


        }

        public void processHeaderEvent()
        {
            myEventList[0].ProcessEvent();

        }

        public void insert(DiscreteEvent theEvent)
        {

            bool isBreak = false;

            if (myEventList.Count == 0)
                //Insert when the list is empty
                myEventList.Add(theEvent);
            else
            {

                if (theEvent.EventTime < myEventList[0].EventTime)
                    myEventList.Insert(0, theEvent);
                else
                {
                    for (int i = 0; i < myEventList.Count; i++)
                        if (theEvent.EventTime >= myEventList[i].EventTime)
                            if (i + 1 < myEventList.Count)
                                if (theEvent.EventTime < myEventList[i + 1].EventTime)
                                {
                                    myEventList.Insert((i + 1), theEvent);
                                    isBreak = true;
                                    break;
                                }
                    if (!isBreak)
                        myEventList.Add(theEvent);
                }
            }

        }


        public DiscreteEvent RemoveHeader()
        {
            DiscreteEvent theHeader = myEventList[0];
            myEventList.RemoveAt(0);

            return theHeader;
        }


    }
}
