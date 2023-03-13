using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomVariateGenerator;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
namespace BankService
{
    public class BankTeller :GraphicalElement
    {
        public double NextServiceDoneTime;
        //private double OldEventTime=0.0;
        private FcfsTimedQueue<Customer> myQueue;

      
        //private int RandomServiceTimeGenerator;
        public string Status="idle";
        private double cumulatedBusyTime=0;
        private double busyTimeStart;
        //Color myColor;

        RandomVariableGenerator myServiceTime;
        private double ServiceDoneTime;
        private Customer theUnderServiceCustomer;
        public List<Customer> TheExitCustomerList;
        //public int[] theBankTellerIndex;

        int customerServed = 0;
        private int ID;
        private string serverName;
        static int serverSerialNo = 0;

        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        public int TellerID
        {
            get { return ID; }
            set { ID = value; }
        }
        public int QueueID;
        public Series ganttChartSeries=null;


        public FcfsTimedQueue<Customer> setReferenceQueue
        {
 
            set { myQueue = value; }
        }
        

        


       /// <summary>
       /// 
       /// </summary>
       /// <param name="theServiceTime">input the random variate generator for Service Time</param>
        /// <param name="theQueue">input the refered queue for the teller(server)</param>
        public BankTeller(RandomVariableGenerator theServiceTime,FcfsTimedQueue<Customer> theQueue )
        {
            myQueue = theQueue;
            myServiceTime = theServiceTime;
            serverName = "Server" + serverSerialNo.ToString();
            serverSerialNo++;
            ShowTellerInfo = new TellerInfo();
            ShowTellerInfo.theTeller = this;
        }

        public bool IsQueueRefered
        {
          get
          {
              if (myQueue != null)
                  return true;
              else return false;
           }
        }


        public BankTeller(RandomVariableGenerator theServiceTime,Rectangle rc) : base(rc)
        {
            myServiceTime = theServiceTime;
            serverName = "Server" + serverSerialNo.ToString();
            serverSerialNo++;

            ShowTellerInfo = new TellerInfo();
            ShowTellerInfo.theTeller = this;

            endPoint =new Point( rc.Left,rc.Top);

        }
       

        public int CustomerServed
        {
            get
            {
                return customerServed;
            }
          
        }

        public void resetBankTeller()
        {
            customerServed = 0;
            cumulatedBusyTime = 0;

            if (ReferredQueue != null)
            {
                ReferredQueue.Clear();
                
            }
           
            Status = "idle";
            
            
        }
       

        public double BusyTime
        {
            get
            {
                return cumulatedBusyTime;
            }
       
        }



        public RandomVariableGenerator RevisibleServiceTimeData
        {
            get
            {

                return myServiceTime;
            }
            set
            {
                myServiceTime = value;

            }

        }


        public int CurrentQueueLength
        {
            get
            {
                return myQueue.CurrentSize;
            }
          
        }

        public int MaxQueueLength
        {
            get
            {
                return myQueue.MaxLength;
            }
         
        }

        public FcfsTimedQueue<Customer> ReferredQueue
        {
            get
            {
                return myQueue;
            }
         
        }
       
        /// <summary>
        /// Put the customer from the queue to the server 
        /// </summary>
        public void serveACustomer()
        {


            theUnderServiceCustomer = myQueue.removeHead();
            updateTheQueuedCustomerGraphicalData();
            //updateTheQueudCustomerStringList();

            theUnderServiceCustomer.enterTellerTime = NextServiceDoneTime;
           
            double generatedServiceTime = myServiceTime.getASampleData();
           
            //Prevent from generating negative service time value
            while(generatedServiceTime<0)
                generatedServiceTime = myServiceTime.getASampleData();


        
            ServiceDoneTime = NextServiceDoneTime+ generatedServiceTime;
            NextServiceDoneTime = ServiceDoneTime;

            theUnderServiceCustomer.serviceDoneTime = ServiceDoneTime;
            Status = "busy";
           // changeTellerToBusyState_G();
            customerServed++;
                

            
        }

        public override void drawElement(Graphics g)
        {

           // base.drawElement(g);
            g.DrawString(this.serverName.ToString(), new Font(new FontFamily("Arial"), 10, new FontStyle()), new SolidBrush(Color.Black), bound.X + 2, bound.Y - 13);


            if (Status == "busy")
            {
               // g.FillRectangle(new SolidBrush(Color.Red), bound);
                g.FillRectangle(new SolidBrush(myColor), bound);
                Pen thePen = new Pen(new SolidBrush(Color.Red));
                thePen.Width = 5;
                g.DrawRectangle(thePen, bound);
                g.DrawString("C"+theUnderServiceCustomer.ID.ToString()+"\n"+NextServiceDoneTime.ToString("0.000"), new Font(new FontFamily("Arial"), 12, new FontStyle()), new SolidBrush(theUnderServiceCustomer.myColor), bound.X+8, bound.Y+10);

            }
            else
                g.FillRectangle(new SolidBrush(myColor), bound);
        }

        /// <summary>
        /// put the customer to the teller(server) directly
        /// </summary>
        /// <param name="aCustomer">the customer to be served</param>
        public void serveACustomer(Customer aCustomer)
        {
            theUnderServiceCustomer = aCustomer;
            theUnderServiceCustomer.enterTellerTime = theUnderServiceCustomer.ArrivalTime;

            double generatedServiceTime=myServiceTime.getASampleData();
            //Prevent from generating negative service time value
            while (generatedServiceTime < 0)
                generatedServiceTime = myServiceTime.getASampleData();


            ServiceDoneTime = aCustomer.ArrivalTime + generatedServiceTime;
            NextServiceDoneTime = ServiceDoneTime;
           
            
            theUnderServiceCustomer.serviceDoneTime = ServiceDoneTime;
            Status = "busy";
            customerServed++;
          //  changeTellerToBusyState_G();
            busyTimeStart = theUnderServiceCustomer.ArrivalTime;

            updateGanttChart();
        }
        
        /// <summary>
        /// Make the customer leave the teller
        /// </summary>
        public void expellACustomer()
        {

        
            TheExitCustomerList.Add(theUnderServiceCustomer);
            
            //If the refered queue is not empty, keep serving the customer inside the queue
            //If the refered queue is empty, mark up the server as idle status. Update the cumulated busy time
            if (myQueue.CurrentSize != 0)
                serveACustomer();
            else
            {
            
                Status = "idle";
                updateBusyTime();
                updateGanttChart();
            }

            
        }


        void updateBusyTime()
        {       
          cumulatedBusyTime+=  theUnderServiceCustomer.serviceDoneTime - busyTimeStart;
        }


        /// <summary>
        /// Enqueue(line up) the customer when the server is busy
        /// </summary>
        /// <param name="aCustomer"></param>
        public void escortACustomer(Customer aCustomer)
        {
            myQueue.addTail(aCustomer);
            updateTheQueuedCustomerGraphicalData();
            //updateTheQueudCustomerStringList();
        }

        /// <summary>
        /// generate the bank teller List data
        /// </summary>
        /// <param name="serverNo">Total number of the server</param>
        /// <param name="theServiceTime">Input R.V. of the service time</param>
        /// <param name="theGanttSeries">Input the Series for drawing gantt Series</param>
        /// <param name="theQueueSizeStepLine">Input the Series for drawing the queueVarationSize Series </param>
        /// <param name="theServerQueueBehavior">enum type of  (1)multiline queue or (2) all servers share with one queue</param>
        /// <returns></returns>
        public static List<BankTeller> generateBankTellerList(int serverNo,List<RandomVariableGenerator> theServiceTimeList,Series[] theGanttSeries,Series[] theQueueSizeStepLine, enumBankTellersQueueBehavior theServerQueueBehavior)
        {
            List<BankTeller> TheBankTellers=new List<BankTeller>();
            FcfsTimedQueue<Customer> singleQueue=new FcfsTimedQueue<Customer>();

            int TheQueueID = 0;
       
            for (int i = 0; i < serverNo; i++)
            {
                BankTeller aBankTeller;
                
                if (theServerQueueBehavior == enumBankTellersQueueBehavior.MultiLineQueue)
                {
                    //若屬於multiLineQueue形式=>N個Server有N個獨立參照的Queue
                    aBankTeller = new BankTeller(theServiceTimeList[i], new FcfsTimedQueue<Customer>());
                    TheQueueID = i;
                }
                else
                    //若屬於SharedWithOneQueue=>N個server共同參照1個Queue
                    aBankTeller = new BankTeller(theServiceTimeList[i], singleQueue);

               
                
                aBankTeller.ID = i;
                aBankTeller.QueueID = TheQueueID;                               
                aBankTeller.ganttChartSeries = theGanttSeries[i];
                aBankTeller.ganttChartSeries.LegendText = "Server" + i.ToString();

                theQueueSizeStepLine[TheQueueID].LegendText = "QueueNo:" + TheQueueID.ToString();

                TheBankTellers.Add(aBankTeller);
             
            }

            initializeGanntChartSeries(TheBankTellers, theGanttSeries);
           
            return TheBankTellers;
        }

        public static void initializeGanntChartSeries(List<BankTeller> tList,Series[] theGanttSeries)
        {
            for (int i = 0; i < tList.Count; i++)
            {

                tList[i].ID = i;
                tList[i].QueueID = i;
                tList[i].ganttChartSeries = theGanttSeries[i];
               
                tList[i].ganttChartSeries.Color = tList[i].myColor;
                
               
                tList[i].ganttChartSeries.Name = tList[i].serverName;
                tList[i].ganttChartSeries.LegendText = tList[i].serverName;
            }

        }

        /*
        public static void gatherBankTellerList(List<BankTeller> tList,Series[] theGanttSeries,Series[] theQueueSizeStepLine, enumBankTellersQueueBehavior theServerQueueBehavior)
        {

            int TheQueueID = 0;        

            if (theServerQueueBehavior == enumBankTellersQueueBehavior.SharedWithOneQueue)
            {
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].ID = i;
                    tList[i].QueueID = TheQueueID;
                    tList[i].ganttChartSeries = theGanttSeries[i];
                    tList[i].ganttChartSeries.LegendText = "Server" + tList[i].ID.ToString();
                }
            }
            else
            for (int i = 0; i <tList.Count; i++)
            {
                
                 
                tList[i].ID = i;
                tList[i].QueueID = TheQueueID;
                tList[i].ganttChartSeries = theGanttSeries[i];
                tList[i].ganttChartSeries.LegendText = "Server" + tList[i].ID.ToString();



            }



        }*/

      


        /*
        public static BankTeller genearteABankTeller(int serverNo, RandomVariableGenerator theServiceTime, Series[] theGanttSeries, Series[] theQueueSizeStepLine, enumBankTellersQueueBehavior theServerQueueBehavior)
        {
            BankTeller aBankTeller;
            aBankTeller = new BankTeller(theServiceTime,);
           
                return aBankTeller;
        }*/
       
        



        // for graphical model creation
        //for multiple queue
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputBankTeller">assigned bank teller</param>
        /// <param name="inputQueue">assinged queue</param>
        public static void linkQueueToBankTeller(BankTeller inputBankTeller, FcfsTimedQueue<Customer> inputQueue)
        {
            inputQueue.ServerColor = inputBankTeller.myColor;
            inputBankTeller.setReferenceQueue = inputQueue;
            
        }

        




        private void updateGanttChart()
        {
            if (Status == "busy")
            {
                //剛從idle轉換成busy狀態=>畫idle gantt chart
                ganttChartSeries.Points.AddXY(busyTimeStart, ID);
                ganttChartSeries.Points[ganttChartSeries.Points.Count - 1].Color = Color.Black;
            }
            else
            {
                //剛從busy轉換成idle狀態=>畫busy ganttChart
                ganttChartSeries.Points.AddXY(theUnderServiceCustomer.serviceDoneTime,ID);  
            }

        }

        TellerInfo showTellerInfo;//=new TellerInfo();

        public TellerInfo ShowTellerInfo
        {
            get {
               
          
                showTellerInfo.TheCustomer = theUnderServiceCustomer;         
                return showTellerInfo; 
           
            
               }
            set { showTellerInfo = value; }
        }


        public void updateTheQueuedCustomerGraphicalData()
        {
            updateTheQueuedCustomerColorList();
            updateTheQueudCustomerStringList();
        }

     

        public void updateTheQueudCustomerStringList()
        {
            List<String> theStringList = new List<string>();

            for (int i = 0; i < myQueue.CurrentSize; i++)
            {
             Customer c=  myQueue.ElementAt(i);
             theStringList.Add("C" + c.ID.ToString());
            }


            myQueue.stringTextList = theStringList;
            
            //return theStringList;
         

        }


        public void updateTheQueuedCustomerColorList()
        {
            List<Color> theColorList = new List<Color>();

            for (int i = 0; i < myQueue.CurrentSize; i++)
            {
                Customer c = myQueue.ElementAt(i);
                               
                theColorList.Add(c.myColor);
            }
            myQueue.QueuedColorList = theColorList;
        }



    public  class TellerInfo
    {
       
          public BankTeller theTeller;

        public Customer TheCustomer;

        public int CustomerID
        {
            get {
                if (TheCustomer != null)
                    return theTeller.theUnderServiceCustomer.ID;
                else
                    return -1;
            }
          //  set { theCustomer = value; }
        }
        public int TellerID
        {
            get { return theTeller.TellerID; }
      
        }
        public string TellerName
        {
            get
            {
                return theTeller.serverName;
            }
            set
            {

               theTeller.serverName = value;
            }
        }


          /*
        public int CustomerServed
        {
            get{return theTeller.CustomerServed;}

        }*/

        


   

       // public get


        

    }

    }

  
}
