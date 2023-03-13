using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;





using RandomVariateGenerator;
using System.Windows.Forms.DataVisualization.Charting;


namespace BankService
{


    public class BankSimulationEngine
    {
        public List<BankTeller> TheBankTellers;
        public List<RandomVariableGenerator> TheServiceTimeList;


        public TimeSortedEventQueue TheEventList;
        private List<Customer> TheCustomerArrivalList;
        public List<Customer> TheCustomerExitList;
        public Bank theBank;
        //bool isSimulationTerminated = false;
        int serverNo;
        int customerNo;
        RandomVariableGenerator theCustomerArrivalTime;
        RandomVariableGenerator theServiceTime;
        Series[] thePieChart, theQueueStepLine, theGanttChartSeries;
        enumBankTellersQueueBehavior myBankTellersQueueBehavior;
        public Series EventListSeries=null;
        CustomerSetInfo theCustomerInfo;
        public  FcfsTimedQueue<Customer> singleQueue;

        private List<FcfsTimedQueue<Customer>> theQueueList;


        int iterationCount;

        double HeadEventTime;

        private bool isCreatedByGraphical = false;
        public bool IsCreatedByGraphical
        {
            get { return isCreatedByGraphical; }
            set { isCreatedByGraphical = value; }
        }

      


        public double CustomerAverageWatingTime
        {
            get
            {
                double[] avg =new double[TheCustomerExitList.Count];
                for(int i=0;i<TheCustomerExitList.Count;i++)
                {
                    avg[i] = TheCustomerExitList[i].WaitingTime;
                }
                return avg.Average();
            }

        }


        public double ServerAverageBusyTime
        {
            get
            {
                double[] avg = new double[TheBankTellers.Count];
                for (int i = 0; i < TheBankTellers.Count; i++)
                    avg[i] = TheBankTellers[i].BusyTime;
               
                return avg.Average();

            }


        }


        /// <summary>
        /// Constructor for the banking simulation engine
        /// </summary>
        /// <param name="customerNo">total customer number</param>
        /// <param name="serverNo">total teller(server) number</param>
        /// <param name="customerArrivalTime">R.V. of the customer arrival time</param>
        /// <param name="serviceTime">R.V. of the service time</param>
        public BankSimulationEngine(int customerNo,int serverNo, RandomVariableGenerator customerArrivalTime,RandomVariableGenerator serviceTime)
        {
            //不是目前採用的constructor
            this.serverNo = serverNo;
            theCustomerArrivalTime = customerArrivalTime;
            theServiceTime = serviceTime;
            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = new List<BankTeller>();

            isCreatedByGraphical = false;

        }

        /// <summary>
        /// Constructor for the banking simulation engine
        /// </summary>
        /// <param name="customerNo">total customer number</param>
        /// <param name="serverNo">total teller(server) number</param>
        /// <param name="customerArrivalTime">R.V. of the customer arrival time</param>
        /// <param name="serviceTime">R.V. of the service time</param>
        /// <param name="QueueStepLines">Series for displaying the queueVariationSize</param>

        public BankSimulationEngine(int customerNo, int serverNo, RandomVariableGenerator customerArrivalTime, RandomVariableGenerator serviceTime,Series[] QueueStepLines)
        {
            //不是目前採用的constructor
            this.serverNo = serverNo;
            theCustomerArrivalTime = customerArrivalTime;
            theServiceTime = serviceTime;
            this.customerNo = customerNo;


            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = new List<BankTeller>();
            theQueueStepLine=QueueStepLines;

            isCreatedByGraphical = false;

        }

        /// <summary>
        /// Constructor for the banking simulation engine
        /// </summary>
        /// <param name="customerNo">total customer number</param>
        /// <param name="serverNo">total teller(server) number</param>
        /// <param name="customerArrivalTime">R.V. of the customer arrival time</param>
        /// <param name="serviceTime">R.V. of the service time</param>
        /// <param name="QueueStepLines">Series for displaying the queueVariationSize</param>
        /// <param name="PieCharts">Charts for displaying the teller(server) busy and idle ratio</param>
        public BankSimulationEngine(int customerNo, int serverNo, RandomVariableGenerator customerArrivalTime, RandomVariableGenerator serviceTime, Series[] QueueStepLines,Series[] PieCharts)
        {
            //不是目前採用的constructor
            this.serverNo = serverNo;
            theCustomerArrivalTime = customerArrivalTime;
            theServiceTime = serviceTime;
            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = new List<BankTeller>();
            theQueueStepLine = QueueStepLines;
            thePieChart = PieCharts;

            isCreatedByGraphical = false;
        }

        /// <summary>
        /// Constructor for the banking simulation engine
        /// </summary>
        /// <param name="customerNo">total customer number</param>
        /// <param name="serverNo">total teller(server) number</param>
        /// <param name="customerArrivalTime">R.V. of the customer arrival time</param>
        /// <param name="serviceTime">R.V. of the service time</param>
        /// <param name="QueueStepLines">Series for displaying the queueVariationSize</param>
        /// <param name="PieCharts">Charts for displaying the teller(server) busy and idle ratio</param>
        /// <param name="myGanttChartSeries">Bars for displaying the teller(server) busy and idle time </param>
        public BankSimulationEngine(int customerNo, int serverNo, RandomVariableGenerator customerArrivalTime, RandomVariableGenerator serviceTime, Series[] QueueStepLines, Series[] PieCharts,Series[] myGanttChartSeries)
        {
            //不是目前採用的constructor
            this.serverNo = serverNo;
            theCustomerArrivalTime = customerArrivalTime;
            theServiceTime = serviceTime;
            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = new List<BankTeller>();
            theQueueStepLine = QueueStepLines;
            thePieChart = PieCharts;
            theGanttChartSeries = myGanttChartSeries;

            isCreatedByGraphical = false;

            Initialize();
            InitializeEventList();
        }


        /// <summary>
        /// Constructor for the banking simulation engine
        /// </summary>
        /// <param name="customerNo">total customer number</param>
        /// <param name="serverNo">total teller(server) number</param>
        /// <param name="customerArrivalTime">R.V. of the customer arrival time</param>
        /// <param name="serviceTime">R.V. of the service time</param>
        /// <param name="QueueStepLines">Series for displaying the queueVariationSize</param>
        /// <param name="PieCharts">Charts for displaying the teller(server) busy and idle ratio</param>
        /// <param name="theGanttChartSeries">Bars for displaying the teller(server) busy and idle time </param>
        /// <param name="theEventListSeries"></param>
        public BankSimulationEngine(int customerNo, int serverNo, RandomVariableGenerator customerArrivalTime, RandomVariableGenerator serviceTime, Series[] QueueStepLines, Series[] PieCharts, Series[] GanttChartSeries, Series theEventListSeries, enumBankTellersQueueBehavior theBankTellersQueueBehavior)
        {
            //目前採用的constructor
            this.serverNo = serverNo;
            theCustomerArrivalTime = customerArrivalTime;
            theServiceTime = serviceTime;
            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = new List<BankTeller>();
            theQueueStepLine = QueueStepLines;
            thePieChart = PieCharts;
            theGanttChartSeries = GanttChartSeries;
            EventListSeries = theEventListSeries;
            myBankTellersQueueBehavior = theBankTellersQueueBehavior;
            isCreatedByGraphical = false;
            Initialize();
            InitializeEventList();
        }


        public BankSimulationEngine(int customerNo, int serverNo, RandomVariableGenerator customerArrivalTime, List<RandomVariableGenerator> serviceTimeList, Series[] QueueStepLines, Series[] PieCharts, Series[] GanttChartSeries, Series theEventListSeries, enumBankTellersQueueBehavior theBankTellersQueueBehavior, CustomerSetInfo theCustInfo)
        {
            //目前採用的constructor
            this.serverNo = serverNo;
            theCustomerArrivalTime = customerArrivalTime;
            //theServiceTime = serviceTime;

            TheServiceTimeList = serviceTimeList;
            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = new List<BankTeller>();
            theQueueStepLine = QueueStepLines;
            thePieChart = PieCharts;
            theGanttChartSeries = GanttChartSeries;
            EventListSeries = theEventListSeries;
            myBankTellersQueueBehavior = theBankTellersQueueBehavior;
            this.theCustomerInfo = theCustInfo;

            isCreatedByGraphical = false;
            Initialize();
            InitializeEventList();
        }


        //for graphical Dislay model constructor multiline
        public BankSimulationEngine(int customerNo, RandomVariableGenerator customerArrivalTime, List<BankTeller> InputBankTellerList, Series[] QueueStepLines, Series[] PieCharts, Series[] GanttChartSeries, Series theEventListSeries, enumBankTellersQueueBehavior theBankTellersQueueBehavior, CustomerSetInfo theCustInfo)
        {
            //目前採用的constructor
            this.serverNo = InputBankTellerList.Count;
            theCustomerArrivalTime = customerArrivalTime;
            //theServiceTime = serviceTime;

            //TheServiceTimeList = serviceTimeList;
            TheBankTellers = InputBankTellerList;

            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = InputBankTellerList;
           
            
            theQueueStepLine = QueueStepLines;
            
            thePieChart = PieCharts;
            theGanttChartSeries = GanttChartSeries;
            EventListSeries = theEventListSeries;
            myBankTellersQueueBehavior = theBankTellersQueueBehavior;
            this.theCustomerInfo = theCustInfo;
            isCreatedByGraphical = true;

            Initialize();
            InitializeEventList();
        }


        //for graphical Dislay model constructor single line
        /*
        public BankSimulationEngine(int customerNo, RandomVariableGenerator customerArrivalTime, List<BankTeller> InputBankTellerList, FcfsTimedQueue<Customer> singleQueue, Series[] QueueStepLines, Series[] PieCharts, Series[] GanttChartSeries, Series theEventListSeries, enumBankTellersQueueBehavior theBankTellersQueueBehavior,CustomerSetInfo theCustInfo)
        {
            //目前採用的constructor
            this.serverNo = InputBankTellerList.Count;
            theCustomerArrivalTime = customerArrivalTime;
            //theServiceTime = serviceTime;

            //TheServiceTimeList = serviceTimeList;
            TheBankTellers = InputBankTellerList;
            this.theCustomerInfo = theCustInfo;
            this.singleQueue = singleQueue;
            this.customerNo = customerNo;
            TheCustomerArrivalList = new List<Customer>();
            TheCustomerExitList = new List<Customer>();
            TheBankTellers = InputBankTellerList;
            theQueueStepLine = QueueStepLines;
            thePieChart = PieCharts;
            theGanttChartSeries = GanttChartSeries;
            EventListSeries = theEventListSeries;
            myBankTellersQueueBehavior = theBankTellersQueueBehavior;
           

            isCreatedByGraphical = true;

            Initialize();
            InitializeEventList();
        }*/




        /// <summary>
        /// Generate The bank teller list, custeomr arrival list and the bank manager
        /// </summary>
        public void Initialize()
        {
            //一個bank teller為一個櫃台,可處理queue共享亦可處理queue獨立
            //產生多個bank teller

           
            if (IsCreatedByGraphical == false)
                TheBankTellers = BankTeller.generateBankTellerList(serverNo, TheServiceTimeList, theGanttChartSeries, theQueueStepLine, myBankTellersQueueBehavior);


            BankTeller.initializeGanntChartSeries(TheBankTellers, theGanttChartSeries);
            

            //產生多個客人及客人到達時間
            TheCustomerArrivalList = Customer.generateCustomerList(customerNo, theCustomerArrivalTime,theCustomerInfo);
           
            //負責整合管理多個Bank Teller的物件(類別)
            //theBank = new Bank(TheBankTellers, TheCustomerArrivalList,enumBankTellersQueueBehavior.SharedWithOneQueue); 

            theBank = new Bank(TheBankTellers, TheCustomerArrivalList,myBankTellersQueueBehavior); 
           
            

        }

        public void InitializeEventList()
        {
            TheEventList = new TimeSortedEventQueue();

            for (int i = 0; i < TheCustomerArrivalList.Count; i++)
            {
                TheEventList.insert(Customer.generateCustomerArrivalEvent(TheCustomerArrivalList[i], theBank, TheEventList));
            }

            if (EventListSeries != null) 
                printEventListSeries();
        }

        /// <summary>
        ///  Execute the banking simulation with the Event list method one time
        /// </summary>

        public void executeOneSimulation()
        {
            //一次跑一個事件

            HeadEventTime = TheEventList.HeadEventTime;
            //處理自己所屬類別的事件
            TheEventList.processHeaderEvent();



            printEventListSeries();
            printQueueSizeVariation();            
           // printStackBars();

            TheCustomerExitList = theBank.TheCustomerExitList;
            


            //執行完所有事件時才印出busy ratio pie chart
            if(TheEventList.CurrentSize==0)
                printPieCharts();
        }

       

        /// <summary>
        /// Execute the banking simulation with the Event list method
        /// </summary>
        /// 
        public void executeAllSimulation()
        {
            //一口氣跑完所有的事件


            iterationCount = 0;       
 
            while (TheEventList.CurrentSize != 0)
            //EVENTLIST為空時才結束模擬
            {               
                HeadEventTime = TheEventList.HeadEventTime;              
                
                //處理自己所屬類別的事件
                TheEventList.processHeaderEvent();
                
                printQueueSizeVariation();

                

                //計算回圈跑了幾次(for debug use)
                iterationCount++;
            }

            TheCustomerExitList = theBank.TheCustomerExitList;

            //更新所有圖表
            printEventListSeries();
            printPieCharts();
            

        }

        /// <summary>
        /// Execute the banking simulation  (Not event list method)
        /// </summary>
        /// 

      

        
        void printQueueSizeVariation()
        {
            double oldTime = 0;
            if (theQueueStepLine != null)
            {
                for (int i = 0; i < theQueueStepLine.Length; i++)
                {


                    theQueueStepLine[i].Color = theBank.TheDistinctQueueList[i].myColor;



                    if (theBank.TheDistinctQueueList[i].Name == null)
                    {
                        theBank.TheDistinctQueueList[i].Name = "Queue of " + TheBankTellers[i].ServerName.ToString();
                    }


                    theQueueStepLine[i].LegendText = theBank.TheDistinctQueueList[i].Name;  //TheBankTellers[i].ReferredQueue.Name;
                    theQueueStepLine[i].Points.AddXY(HeadEventTime, theBank.TheDistinctQueueList[i].CurrentSize);

                    //theQueueStepLine[i].Points.AddXY(HeadEventTime, TheBankTellers[i].CurrentQueueLength);

                    oldTime = theBank.CurrentTime;
                }

                computeTimeQueueSize();
            }
        }

        void computeTimeQueueSize()
        {
            for (int i = 0; i < theQueueStepLine.Length; i++)
            {
                if (theQueueStepLine[i].Points.Count > 2)
                {
                    double area = 0;
                    for (int j = 2; j < theQueueStepLine[i].Points.Count; j++)
                        area += (theQueueStepLine[i].Points[j].XValue - theQueueStepLine[i].Points[j - 1].XValue) * theQueueStepLine[i].Points[j].YValues[0];

                    area = area / theQueueStepLine[i].Points[theQueueStepLine[i].Points.Count - 1].XValue;
                    theBank.TheDistinctQueueList[i].TimeAverageSize = area;
                  
                }

            }

        }
     

        void printPieCharts()
        {
            if (thePieChart!=null)
                for (int i = 0; i < thePieChart.Length; i++)
                {
                    // Print Busy Ratio
                    //thePieChart.ti = TheBankTellers[i].ServerName;
                    thePieChart[i].Points.AddY((TheBankTellers[i].BusyTime)/theBank.SytemTerminationTime);
                    thePieChart[i].Points[0].Label ="Busy:"+ (100*(TheBankTellers[i].BusyTime) / theBank.SytemTerminationTime).ToString("0.00") + "%";
                    
                    //Print Idle Ratio
                    thePieChart[i].Points.AddY((theBank.SytemTerminationTime- TheBankTellers[i].BusyTime)/theBank.SytemTerminationTime);
                    thePieChart[i].Points[1].Label = "Idle:" + (100*(theBank.SytemTerminationTime - TheBankTellers[i].BusyTime) / theBank.SytemTerminationTime).ToString("0.00")+"%";
                    //thePieChart[i].tit
                }         

        }

        void printGanttChart()
        {


        }


        /*
        void printStackBars()
        {
            if(theStackedBars!=null)
                for (int i = 0; i < TheBankTellers.Count; i++)
                {
                    //Print total Busy time
                    theStackedBars[0].Points.AddXY(i, TheBankTellers[i].BusyTime);
                    
                    //Print total Idle time
                    theStackedBars[1].Points.AddXY(i,myBankManager.SytemTerminationTime- TheBankTellers[i].BusyTime);
                }


        }*/

        void printEventListSeries()
        {
            DiscreteEvent tempEvent;
          
            
            EventListSeries.Points.Clear();

            for (int i = 0; i < TheEventList.CurrentSize; i++)
            {
                tempEvent = TheEventList.getEvent(i);

                
                if (tempEvent is ArrivalEvent)
                {
                    //Display Arrival Event Point
                   
                    EventListSeries.Points.AddXY(tempEvent.EventTime.ToString("0.0000"), 1);
                    EventListSeries.Points[i].MarkerStyle = MarkerStyle.Circle;
                    EventListSeries.Points[i].MarkerSize = 10;

                   // EventListSeries.Points[i].Color = Color.Green;

                    EventListSeries.Points[i].Color = tempEvent.myColor;

                    EventListSeries.Points[i].MarkerBorderColor = Color.Black;

                    if (customerNo <= 100)
                    {
                        ArrivalEvent tempArrivalEvent = (ArrivalEvent)tempEvent;
                        EventListSeries.Points[i].Label = "C" + tempArrivalEvent.CustomerID;
                    }
                   
                    
                }
                else
                {
                   //Display Service Down Event Point
                    EventListSeries.Points.AddXY(tempEvent.EventTime.ToString("0.0000"), 3);
                    EventListSeries.Points[i].MarkerStyle = MarkerStyle.Star4;
                    EventListSeries.Points[i].MarkerSize = 20;
                    EventListSeries.Points[i].Color = Color.Red;
                    EventListSeries.Points[i].MarkerBorderColor = Color.Black;
                    EventListSeries.Points[i].IsVisibleInLegend = true;


                    ServiceDoneEvent tempServiceDownEvent = (ServiceDoneEvent)tempEvent;
                    EventListSeries.Points[i].Label = "T"+tempServiceDownEvent.TellerID.ToString();

                }
            }

         

        }

      
    }
}
