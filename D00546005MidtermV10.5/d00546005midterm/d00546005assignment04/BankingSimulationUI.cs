using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RandomVariateGenerator;
using BankService;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace BankService
{
    public partial class MultiQueueBankTellerSimulationUI : Form
    {
        //List<SimulationConfigure> ConfigureList; 
        Series[] QueueSizeVariationStepLines;
        Series[] pieCharts;
        Series[] GanttChartSeries;
        Series EventListSeries;
        BankSimulationEngine theBankSimulation;
        SimulationConfigure currentConfigure;
       // List<ToolStripButton> allToolStripButton;
      //  List<Rectangle> TellerGraphics;

        String[,] academicRunRecordedData;


        //Variable for graphcial model construction
        List<BankTeller> theBankTellerList;
        List<FcfsTimedQueue<Customer>> theQueueList;
        List<GraphicalElement> theRelationList;
        Point startPoint,endPoint;
        enumBankTellersQueueBehavior currentQueueBehavior=enumBankTellersQueueBehavior.MultiLineQueue;
        CustomerSetInfo createdCustomerSetInfo;
        GraphicalElement Entrance;
        bool isArrivalSelected = false;

        BankTeller currentSelectedTeller;
        FcfsTimedQueue<Customer> currentSelectedQueue;
        public MultiQueueBankTellerSimulationUI()
        {
            InitializeComponent();
           // TellerGraphics = new List<Rectangle>();


            
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer,
              true);

            MethodInfo mm = typeof(Panel).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            mm.Invoke(splitContainer10.Panel1, new object[] {ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer,
                true });
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 用來儲存simulation setting的structure
        /// </summary>
        public struct SimulationConfigure
        {
            public int CustomerNo;
            public String ConfigureName;
            public int ServerNo;
            public int QueueNo;
            public RandomVariableGenerator ServiceTimeRV;
            public RandomVariableGenerator ArrivalTimeRV;
            public Series[] QueueStepLines;
            public Series[] pieCharts;
            public Series[] GanttChartSeires;
            public enumBankTellersQueueBehavior myQueueBehavior;
            public List<RandomVariableGenerator> ServiceTimeRvList;

        }

        

        private void btnInitializeConfigure_Click(object sender, EventArgs e)
        {
            try
            {

                currentConfigure.CustomerNo = int.Parse(textBoxCustomerNo.Text);
                currentConfigure.ServerNo = int.Parse(textBoxServerNo.Text);
                currentConfigure.QueueNo = currentConfigure.ServerNo;
              
                pieCharts = new Series[currentConfigure.ServerNo];
                GanttChartSeries = new Series[currentConfigure.ServerNo];



               // currentConfigure.QueueStepLines = QueueSizeVariationStepLines;
                currentConfigure.pieCharts = pieCharts;
                currentConfigure.GanttChartSeires = GanttChartSeries;



                InitializeQueueSizeVarationStepLine();
                initializeBusyRatioPiechart();
                InitializeGanttChart();
                InitializeEventListSeriesPoint(chartEventList);

                theBankSimulation = new BankSimulationEngine(currentConfigure.CustomerNo, currentConfigure.ServerNo, currentConfigure.ArrivalTimeRV, currentConfigure.ServiceTimeRvList, currentConfigure.QueueStepLines, currentConfigure.pieCharts, currentConfigure.GanttChartSeires, EventListSeries, currentConfigure.myQueueBehavior,createdCustomerSetInfo);
                //theBank = new BankSimulationEngine(currentConfigure.CustomerNo, currentConfigure.ServerNo, currentConfigure.ArrivalTimeRV, currentConfigure.ServiceTimeRV, currentConfigure.QueueStepLines, currentConfigure.pieCharts, currentConfigure.GanttChartSeires, EventListSeries,currentConfigure.myQueueBehavior);

          }
            catch
            {
                MessageBox.Show("Inappropriate Settings");

            }
      
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
          
        
          

        }

        void stepLineChartPostSettings()
        {
            
          
        }

        RandomVariableGenerator RvTypeSelection(ComboBox ReferedComboBox)
        {
            RandomVariableGenerator SelectedRvType=new RandomVariableGenerator();
            switch (ReferedComboBox.SelectedIndex)
            {
                case(0):
                    //Normal
                    SelectedRvType = new NormalRVGenerator(1, 1, EnumNormalMethod.BoxMuller);
                break;
                case(1):
                    //Exponential
                SelectedRvType = new ExponentialRVGenerator(1, EnumExponentialMethod.LogUniform);
                break;
                case(2):
                    //gamma

                SelectedRvType = new GammaRVGenerator(1, 1, EnumGammaMethod.KunduAndGuptaMethod);
                break;
                case(3):
                    //Triangular
                SelectedRvType = new TriangularRVGenerator(2, 0, 1);

                break;
                
                case(4):
                    //Weibull
                SelectedRvType = new WeibullRVGenerator(1, 1);
                break;




            }

            return SelectedRvType;
        }


        void InitializeEventListSeriesPoint(Chart theChart)
        {
            EventListSeries = new Series();       
            EventListSeries.ChartType = SeriesChartType.Point;          
            EventListSeries.MarkerSize = 8;
            EventListSeries.IsVisibleInLegend = false;

            theChart.Series.Clear();
            theChart.Series.Add(EventListSeries);

        }

        void InitializeQueueSizeVarationStepLine()
        {
            int totalQueueNo=0;


            if (currentConfigure.myQueueBehavior == enumBankTellersQueueBehavior.MultiLineQueue)
                totalQueueNo = currentConfigure.QueueNo;
            else
                totalQueueNo = 1;

            currentConfigure.QueueStepLines = new Series[totalQueueNo];

            chartQueueSizeVariation.Series.Clear();

           
            for (int i = 0; i < totalQueueNo; i++)
            {
               
                currentConfigure.QueueStepLines[i] = new Series();
                currentConfigure.QueueStepLines[i].ChartType = SeriesChartType.StepLine;
                currentConfigure.QueueStepLines[i].BorderWidth = 3;        
                chartQueueSizeVariation.Series.Add(currentConfigure.QueueStepLines[i]);
            }
        }

        void initializeBusyRatioPiechart()
        {
            //Initialize the pie Chart settings

            chartPieChart.Series.Clear();
            chartPieChart.ChartAreas.Clear();
            chartPieChart.Titles.Clear();

            

            for (int i = 0; i < currentConfigure.pieCharts.Length; i++)
            {
                currentConfigure.pieCharts[i] = new Series("Server" + i.ToString());
                currentConfigure.pieCharts[i].ChartType = SeriesChartType.Pie;
                
                chartPieChart.ChartAreas.Add(new ChartArea(i.ToString()));
                chartPieChart.Series.Add(currentConfigure.pieCharts[i]);
                chartPieChart.Titles.Add(i.ToString());

                chartPieChart.Series[i].ChartArea = i.ToString();
                chartPieChart.Series[i].IsValueShownAsLabel=true;
                chartPieChart.Series[i].IsVisibleInLegend = false;
                

               
                chartPieChart.Titles[i].Text = "server" + i.ToString();
                chartPieChart.Titles[i].DockedToChartArea = i.ToString();
                chartPieChart.Titles[i].Alignment = ContentAlignment.TopLeft;
            }

        }

        void postSetCharts()
        {


        }


        void InitializeGanttChart()
        {
            chartGanttChart.Series.Clear();
            

            for(int i=0;i<currentConfigure.ServerNo;i++)
            {
                currentConfigure.GanttChartSeires[i] = new Series();
                currentConfigure.GanttChartSeires[i].ChartType = SeriesChartType.StepLine;
             
                currentConfigure.GanttChartSeires[i].BorderWidth = 8;
                //currentConfigure.GanttChartSeires[i].IsVisibleInLegend = false;
                chartGanttChart.Series.Add(currentConfigure.GanttChartSeires[i]);

            }

        }

        //Run Single event time Simulation
        private void buttonRunOneTIme_Click(object sender, EventArgs e)
        {


            if (theBankSimulation.TheEventList.CurrentSize == 0)
            {
                MessageBox.Show("No more event to run");
                outputCustomerData();
                outputServerData();
            }
            else
            {
                //event list element不為空時才執行
                theBankSimulation.EventListSeries = EventListSeries;
                theBankSimulation.executeOneSimulation();
                postSettingEventListChart(chartEventList);
                outputCustomerData();
                outputServerData();
            }

            resizeQueueVaraionChart();
            resizeGanttChart();
        }


        void postSettingEventListChart(Chart theChart)
        {
            theChart.Series.Clear();
            theChart.Series.Add(theBankSimulation.EventListSeries);

        }

        //Complete simulation (Run all event)
        private void buttonRunAll_Click(object sender, EventArgs e)
        {    

            if (theBankSimulation.TheEventList.CurrentSize > 0)
                //event list element不為空時才執行
                theBankSimulation.executeAllSimulation();
            else
                MessageBox.Show("No more event to run");

            


            outputCustomerData();
            outputServerData();
            outputOverAllData();

            resizeQueueVaraionChart();
            resizeGanttChart();
        }


        
        void resizeQueueVaraionChart()
        {
        //重設chart area避免step line出界或太小看不清楚    
            chartQueueSizeVariation.ChartAreas.Clear();
            chartQueueSizeVariation.ChartAreas.Add("1");

            for (int i = 0; i < chartQueueSizeVariation.Series.Count; i++)
                chartQueueSizeVariation.Series[i].ChartArea = "1";


        }

        void resizeGanttChart()
        {
            chartGanttChart.ChartAreas.Clear();
            chartGanttChart.ChartAreas.Add("1");

            for (int i = 0; i < chartGanttChart.Series.Count; i++)
                chartGanttChart.Series[i].ChartArea = "1";


        }


        string  outputCustomerData()
        {
            List<Customer> theExitCustomer= theBankSimulation.TheCustomerExitList;
            richTextBoxCustomer.Text = "";
            string customerString="ID\tArvlTime\tWt.Time\tExtTime\t (ordered by ExitTime)\n";
           
            
            for (int i = 0; i < theExitCustomer.Count; i++)
            {
                customerString += theExitCustomer[i].ID.ToString()+"\t";
                customerString += theExitCustomer[i].ArrivalTime.ToString("0.0000") + "\t";
                customerString += theExitCustomer[i].WaitingTime.ToString("0.0000") + "\t";
                customerString += theExitCustomer[i].LeftTime.ToString("0.0000") + "\n";
            }

            richTextBoxCustomer.Text = customerString;
            return customerString;
        }


        string outputServerData()
        {

            richTextBoxTeller.Text = "";
            string tellerString = "ID\tServed\tQname\tMaxQSz\ttAvg.Sz\tBzTime\tIdleTime\n";
            for (int i = 0; i < theBankSimulation.TheBankTellers.Count; i++)
            {
                tellerString += theBankSimulation.TheBankTellers[i].TellerID.ToString() + "\t";
                tellerString += theBankSimulation.TheBankTellers[i].CustomerServed.ToString() + "\t";
                tellerString += theBankSimulation.TheBankTellers[i].ReferredQueue.Name.ToString() + "\t";
                tellerString += theBankSimulation.TheBankTellers[i].MaxQueueLength.ToString() + "\t";
                tellerString += theBankSimulation.TheBankTellers[i].ReferredQueue.TimeAverageSize.ToString("0.00") + "\t";
                tellerString += theBankSimulation.TheBankTellers[i].BusyTime.ToString("0.00") + "\t";
                tellerString += (theBankSimulation.theBank.SytemTerminationTime - theBankSimulation.TheBankTellers[i].BusyTime).ToString("0.00") + "\n";
               
            }

            richTextBoxTeller.Text = tellerString;
            return tellerString;
        }

        String outputOverAllData()
        {
            richTextBoxOverAll.Text = "";
            string overAllString="";
            overAllString = overAllString+ "CustomerAverageWatingTime: " + theBankSimulation.CustomerAverageWatingTime.ToString("0.000") +"\n";
            overAllString =overAllString+ "ServerAverageBusyTime: " + theBankSimulation.ServerAverageBusyTime.ToString("0.000") + "\n";
            richTextBoxOverAll.Text = overAllString;
            return overAllString;


        }

     
        void postSettingQueueSizeVarationChart()
        {
            chartQueueSizeVariation.Series.Clear();
            
            for(int i=0;i<QueueSizeVariationStepLines.Length;i++)
            chartQueueSizeVariation.Series.Add(QueueSizeVariationStepLines[i]);

        }

       


        private void comboBoxInterarrival_SelectedIndexChanged(object sender, EventArgs e)
        {
            createdCustomerSetInfo = new CustomerSetInfo();
            currentConfigure.ArrivalTimeRV = RvTypeSelection(comboBoxInterarrival);
            propertyGridArrivalTime.SelectedObject = currentConfigure.ArrivalTimeRV;
            createdCustomerSetInfo.ArrivalRvType = currentConfigure.ArrivalTimeRV;
        }

        private void comboBoxServiceTime_SelectedIndexChanged(object sender, EventArgs e)
        {

            currentConfigure.ServiceTimeRvList[comboBoxServerNo.SelectedIndex] = RvTypeSelection(comboBoxServiceTime);
            propertyGridServiceTime.SelectedObject = currentConfigure.ServiceTimeRvList[comboBoxServerNo.SelectedIndex];
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentConfigure.myQueueBehavior = selectQueueBehavior(comboBox1);
        }

        enumBankTellersQueueBehavior selectQueueBehavior(ComboBox inputComboBox)
        {

            switch (inputComboBox.SelectedIndex)
            {
                case(0):
                    return enumBankTellersQueueBehavior.MultiLineQueue;

                case(1):
                    return enumBankTellersQueueBehavior.SharedWithOneQueue;
                


            }

            return enumBankTellersQueueBehavior.MultiLineQueue;
        }

        private void btnConfirmBankNo_Click(object sender, EventArgs e)
        {
            comboBoxServerNo.Items.Clear();
            currentConfigure.ServiceTimeRvList = new List<RandomVariableGenerator>();
            currentConfigure.ServiceTimeRvList.Clear();
            for (int i = 0; i < int.Parse(textBoxServerNo.Text); i++)
            {
                comboBoxServerNo.Items.Add("server" + i.ToString());

                currentConfigure.ServiceTimeRvList.Add(new ExponentialRVGenerator(1,EnumExponentialMethod.LogUniform));
            }
        }

        private void comboBoxServerNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGridServiceTime.SelectedObject = currentConfigure.ServiceTimeRvList[comboBoxServerNo.SelectedIndex];
        }

        private void propertyGridServiceTime_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolSender_Click(object sender, EventArgs e)
        {
             if (sender == tsBtnPointer)
            {

                tsBtnPointer.Checked = true;
                tsBtnArrival.Checked=tsBtnRefer.Checked = tsBtnTeller.Checked = tsBtnQueue.Checked = !tsBtnPointer.Checked;

            }
            else if (sender == tsBtnTeller)
            {
                tsBtnTeller.Checked = true;
                tsBtnArrival.Checked=tsBtnRefer.Checked = tsBtnPointer.Checked = tsBtnQueue.Checked = !tsBtnTeller.Checked;


            }
            else if(sender==tsBtnQueue)
            {

                tsBtnQueue.Checked = true;
                tsBtnArrival.Checked=tsBtnRefer.Checked = tsBtnPointer.Checked = tsBtnTeller.Checked = !tsBtnQueue.Checked;


            }
            else if (sender == tsBtnRefer)
            {
                tsBtnRefer.Checked = true;
                tsBtnArrival.Checked = tsBtnPointer.Checked = tsBtnTeller.Checked = tsBtnQueue.Checked = !tsBtnRefer.Checked;

            }
            else if (sender == tsBtnArrival)
            {
                tsBtnArrival.Checked = true;
                tsBtnPointer.Checked = tsBtnTeller.Checked = tsBtnQueue.Checked = tsBtnRefer.Checked=!tsBtnArrival.Checked;

            }
            
            
        }

        private void splitContainer10_Panel1_MouseUp(object sender, MouseEventArgs e)
        {
          //  Point thePosition = splitContainer10.Panel1.PointToScreen(e.Location);

            Point thePosition = e.Location;
            endPoint = e.Location;
            
            //generate teller
            if(tsBtnTeller.Checked)
            {
               
             //   ControlPaint.DrawReversibleFrame(theRec, Color.Green, FrameStyle.Thick);
                Rectangle theRec = new Rectangle(thePosition.X, thePosition.Y, 50, 50);
                if (theBankTellerList == null)
                    theBankTellerList = new List<BankTeller>();



                BankTeller theBankTeller = new BankTeller(new ExponentialRVGenerator(1, EnumExponentialMethod.LogUniform), theRec);
                theBankTeller.startPoint = startPoint;
                theBankTeller.endPoint = endPoint;
                
                theBankTellerList.Add(theBankTeller);
                checkClick(e);
              
                splitContainer10.Panel1.Refresh();

            }
            else if (tsBtnPointer.Checked)
            {

                checkClick(e);
            }
            else if (tsBtnQueue.Checked)
            {
                Rectangle theRec = new Rectangle(thePosition.X, thePosition.Y, 50, 100);

                if (theQueueList == null)
                    theQueueList = new List<FcfsTimedQueue<Customer>>();

             
                FcfsTimedQueue<Customer> theQueue = new FcfsTimedQueue<Customer>(theRec);
                theQueue.startPoint = startPoint;
                theQueue.endPoint = endPoint;

                if (currentQueueBehavior == enumBankTellersQueueBehavior.SharedWithOneQueue && theQueueList.Count == 0)

                    theQueueList.Add(theQueue);
                    

                else if (currentQueueBehavior == enumBankTellersQueueBehavior.MultiLineQueue)
  
                    theQueueList.Add(theQueue);


                propertyGridSelectedElement2.SelectedObject = theQueueList[theQueueList.Count-1];

                splitContainer10.Panel1.Refresh();

            }
            else if (tsBtnRefer.Checked)
            {
                if (checkSelectedBankTeller(e)!=null)
                {
                    currentSelectedTeller = checkSelectedBankTeller(e);

                   
                    if (theRelationList == null)
                        theRelationList = new List<GraphicalElement>();

                    if (!currentSelectedTeller.IsQueueRefered && !currentSelectedQueue.hasRelation)
                    {
                       
                        Rectangle rc = getRect(startPoint, e.Location);
                        GraphicalElement relation = new GraphicalElement(rc);
                        relation.startPoint = startPoint;
                        relation.endPoint = e.Location;


                        theRelationList.Add(relation);


                        BankTeller.linkQueueToBankTeller(currentSelectedTeller, currentSelectedQueue);
                    }

                }

            }
            else if (tsBtnArrival.Checked)
            {
                createdCustomerSetInfo = new CustomerSetInfo();
                
                createdCustomerSetInfo.ArrivalRvType = new ExponentialRVGenerator(1, EnumExponentialMethod.LogUniform);
             
                
                Rectangle theRec = new Rectangle(e.Location.X, e.Location.Y, 50,50);
                Entrance = new GraphicalElement(theRec);

            }

            splitContainer10.Panel1.Refresh();
        }

        void checkClick(MouseEventArgs e)
        {
            isArrivalSelected = false;

            //check IsQueueClicked
            if(theQueueList!=null)
                foreach(FcfsTimedQueue<Customer> fq in theQueueList)
                    switch (fq.CheckIsClicked(e.Location))
                    {
                        case enumClickType.inside:
                            propertyGridSelectedElement.SelectedObject = fq;
                            propertyGridSelectedElement2.SelectedObject = null;

                            break;

                    }


            if (theBankTellerList != null)
                foreach (BankTeller bt in theBankTellerList)
                    switch (bt.CheckIsClicked(e.Location))
                    {
                        case enumClickType.inside:
                            propertyGridSelectedElement.SelectedObject = bt.RevisibleServiceTimeData;
                            propertyGridSelectedElement2.SelectedObject = bt.ShowTellerInfo;
                            currentConfigure.ServiceTimeRV = bt.RevisibleServiceTimeData;
                            currentSelectedTeller = bt;
                            break;

                    }


            if (Entrance != null)
            {
                switch(Entrance.CheckIsClicked(e.Location))
                {
                    case enumClickType.inside:
                        propertyGridSelectedElement.SelectedObject = createdCustomerSetInfo.ArrivalRvType;
                        propertyGridSelectedElement2.SelectedObject = createdCustomerSetInfo;
                isArrivalSelected = true;
                break;
                }
            }

        }

        FcfsTimedQueue<Customer> checkSelectedQueue(MouseEventArgs e)
        {
            if (theQueueList != null)
                foreach (FcfsTimedQueue<Customer> fq in theQueueList)
                    switch (fq.CheckIsClicked(e.Location))
                    {
                        case enumClickType.inside:
                            propertyGridSelectedElement.SelectedObject = fq;
                            //currentConfigure.ServiceTimeRV = fq.RevisibleServiceTimeData;
                            //currentSelectedTeller = fq;
                            return fq;
                            break;

                    }


            return null;
        }

        BankTeller checkSelectedBankTeller(MouseEventArgs e)
        {
            if (theBankTellerList != null)
                foreach (BankTeller bt in theBankTellerList)
                    switch (bt.CheckIsClicked(e.Location))
                    {
                        case enumClickType.inside:
                            propertyGridSelectedElement.SelectedObject = bt.RevisibleServiceTimeData;
                            currentConfigure.ServiceTimeRV = bt.RevisibleServiceTimeData;
                            currentSelectedTeller = bt;
                            return bt;
                            break;


                    }

            return null;
        }

        void drawRectangle()
        {
           

        }

        private void splitContainer10_Panel1_Paint(object sender, PaintEventArgs e)
        {

            if(theRelationList!=null)
                foreach (GraphicalElement ge in theRelationList)
                {
                    ge.drawLine(e.Graphics);
                }

            if (theQueueList!=null)
            foreach (FcfsTimedQueue<Customer> fq in theQueueList)
            {
                fq.drawElement(e.Graphics);       
            }

            if (theBankTellerList!=null)
            foreach (BankTeller bt in theBankTellerList)
            {
                bt.drawElement(e.Graphics);
            }

            if (Entrance != null)
            {
                Entrance.drawCircle(e.Graphics);
              
            }

        }

        private void comboBoxRvSelectionG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isArrivalSelected)
            {
                createdCustomerSetInfo.ArrivalRvType = RvTypeSelection(comboBoxRvSelectionG);
                propertyGridSelectedElement2.SelectedObject = createdCustomerSetInfo;
                propertyGridSelectedElement.SelectedObject = createdCustomerSetInfo.ArrivalRvType;
            }
            else
            {
              
                currentSelectedTeller.RevisibleServiceTimeData = RvTypeSelection(comboBoxRvSelectionG);
                propertyGridSelectedElement.SelectedObject = currentSelectedTeller.RevisibleServiceTimeData;
            }
        }

        private void splitContainer10_Panel1_MouseDown(object sender, MouseEventArgs e)
        {

            startPoint = e.Location;

            if (tsBtnRefer.Checked)
            {
                if (theBankTellerList != null )
                {
                    /*
                    if (checkSelectedBankTeller(e) != null)
                        currentSelectedTeller = checkSelectedBankTeller(e);
                    */

                    if (checkSelectedQueue(e) != null)
                        currentSelectedQueue = checkSelectedQueue(e);
                    

                    startPoint = e.Location;

                }

            }

        }



        Rectangle getRect(Point p1, Point p2)
        {
            Rectangle rec = Rectangle.Empty;
            if (p1.X < p2.X)
            {
                rec.X = p1.X;
                rec.Width = p2.X - p1.X;
            }
            else
            {
                rec.X = p2.X;
                rec.Width = p1.X - p2.X;

            }
            if (p1.Y < p2.Y)
            {
                rec.Y = p1.Y;
                rec.Height = p2.Y - p1.Y;
            }
            else
            {
                rec.Y = p2.Y;
                rec.Height = p1.Y - p2.Y;
            }
            return rec;
        }

        private void radioButtonMultiLine_CheckedChanged(object sender, EventArgs e)
        {
            /*
            clearWholeModel();

            
            if (radioButtonMultiLine.Checked == true)
                currentQueueBehavior = enumBankTellersQueueBehavior.MultiLineQueue;
            else
                currentQueueBehavior = enumBankTellersQueueBehavior.SharedWithOneQueue;
            */
        

        }


        void clearWholeModel()
        {

            try
            {

                theBankTellerList.Clear();
                theQueueList.Clear();
                createdCustomerSetInfo = null;
                if(theRelationList!=null)
                theRelationList.Clear();
                Entrance = null;
            }
            catch
            {

            }
         

            splitContainer10.Panel1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void buttonResetG_Click(object sender, EventArgs e)
        {
            
            try
            {
            

                if (theBankSimulation != null)
                {
                    theBankSimulation = null;
                    foreach (BankTeller bt in theBankTellerList)
                        bt.resetBankTeller();
                }
                splitContainer10.Panel1.Refresh();


                if (createdCustomerSetInfo == null)
                    MessageBox.Show("Customer Arrival Data (Entrance) required!!!");
                
                currentConfigure.CustomerNo = createdCustomerSetInfo.CustomerNo;
                
                currentConfigure.ServerNo = theBankTellerList.Count;
                currentConfigure.QueueNo= theQueueList.Count;
                currentConfigure.ArrivalTimeRV = createdCustomerSetInfo.ArrivalRvType;
                currentConfigure.myQueueBehavior = currentQueueBehavior;

                pieCharts = new Series[currentConfigure.ServerNo];
                GanttChartSeries = new Series[currentConfigure.ServerNo];



                // currentConfigure.QueueStepLines = QueueSizeVariationStepLines;
                currentConfigure.pieCharts = pieCharts;
                currentConfigure.GanttChartSeires = GanttChartSeries;



                InitializeQueueSizeVarationStepLine();
                initializeBusyRatioPiechart();
                InitializeGanttChart();
                InitializeEventListSeriesPoint(chartEventListG);

      
                theBankSimulation = new BankSimulationEngine(currentConfigure.CustomerNo, currentConfigure.ArrivalTimeRV, theBankTellerList, currentConfigure.QueueStepLines, currentConfigure.pieCharts, currentConfigure.GanttChartSeires, EventListSeries, currentConfigure.myQueueBehavior, createdCustomerSetInfo);
              

            
            }
            catch
            {
                MessageBox.Show("Inappropirate Settings");

            }
        
        }

   

        private void buttonRunOneExecutionG_Click(object sender, EventArgs e)
        {
            if (theBankSimulation.TheEventList.CurrentSize == 0)
            {
                MessageBox.Show("No more event to run");
                outputCustomerData();
                outputServerData();
            }
            else
            {
                //event list element不為空時才執行
                theBankSimulation.EventListSeries = EventListSeries;
                theBankSimulation.executeOneSimulation();
                postSettingEventListChart(chartEventListG);
                outputCustomerData();
                outputServerData();


                splitContainer10.Panel1.Refresh();
              //  splitContainer10_Panel1_Paint(null, null);
            }


           
            resizeQueueVaraionChart();
            resizeGanttChart();

       
        }

        public void refreshSimulation()
        {

        }

        public void refreshGraphicalModel()
        {
            
        }


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void propertyGridSelectedElement2_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxHasClosedTime_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHasClosedTime.Checked == true)
                createdCustomerSetInfo.HasClosedTime = true;
            else
                createdCustomerSetInfo.HasClosedTime = false;
        }

        private void textBoxClosedTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                createdCustomerSetInfo.FinalArrivalTime = double.Parse(textBoxClosedTime.Text);
            }
            catch
            {
                textBoxClosedTime.Text = "1000";

            }
        }

        private void radioButtonOneLine_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonClearGraphModel_Click(object sender, EventArgs e)
        {
            clearWholeModel();
            propertyGridSelectedElement.SelectedObject = null;
            propertyGridSelectedElement2.SelectedObject = null;
            
        }

        private void buttonAcademicRun_Click(object sender, EventArgs e)
        {
            academicRunRecordedData = new String[3, int.Parse(textBoxItertationNo.Text)];
            listBoxAcademicRun.Items.Clear();
            for (int i = 0; i < int.Parse(textBoxItertationNo.Text); i++)
            {
                buttonResetG_Click(null, null);
                acadamicRun(i);

               


                resizeQueueVaraionChart();
                resizeGanttChart();
               
            }
        }


        void acadamicRun(int i)
        {
            if (theBankSimulation.TheEventList.CurrentSize > 0)
                theBankSimulation.executeAllSimulation();
            else
                MessageBox.Show("No more event to run");

            academicRunRecordedData[0, i] = outputCustomerData();
            academicRunRecordedData[1, i] = outputServerData();
            academicRunRecordedData[2, i] = outputOverAllData();
            outputOverAllData();
            listBoxAcademicRun.Items.Add("iterationNo:" + i.ToString());

        }


        private void listBoxAcademicRun_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBoxCustomer.Text = academicRunRecordedData[0, listBoxAcademicRun.SelectedIndex];
            richTextBoxTeller.Text = academicRunRecordedData[1, listBoxAcademicRun.SelectedIndex];
            richTextBoxOverAll.Text = academicRunRecordedData[2, listBoxAcademicRun.SelectedIndex];
        }

        private void buttonRunAllExecutionG_Click(object sender, EventArgs e)
        {

            if (theBankSimulation == null) 
            buttonResetG_Click(null, null);
         
            if (checkBoxStartAnimation.Checked)
            {
                timer1.Interval = 100;
                timer1.Start();


                //  timer1.Stop();
            }
            else
            {
                theBankSimulation.executeAllSimulation();
                timer1.Stop();

                 splitContainer10.Panel1.Refresh();

            outputCustomerData();
            outputServerData();
            outputOverAllData();

            resizeQueueVaraionChart();
            resizeGanttChart();
            }
            poSetPieChart();
            

      
        }

        private void saveResultToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveResult saveR=new SaveResult();

            saveFileDialog1.Filter = "txt|*.txt";
            if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            else
            {
                String FileString = saveFileDialog1.FileName;
                SaveResult.saveSingleRunData(FileString, theBankSimulation);
             
            }
        }

        void poSetPieChart()
        {
            for (int i = 0; i < chartPieChart.Titles.Count; i++)
            {
                chartPieChart.Titles[i].Text = theBankSimulation.TheBankTellers[i].ServerName;

            }

        }

        private void richTextBoxCustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "MD file|*.MD";
            saveFileDialog1.FilterIndex = 1; 
            if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            else
            {
                String FileString = saveFileDialog1.FileName;
                saveModel(FileString);

            }


           




        }

        void saveModel(string fs)
        {
            StreamWriter swm = new StreamWriter(fs);

            swm.WriteLine("Queue");
            foreach (FcfsTimedQueue<Customer> fq in theQueueList)
            {
                swm.WriteLine(fq.Name.ToString());

                swm.WriteLine(fq.endPoint.X.ToString());
                swm.WriteLine(fq.endPoint.Y.ToString());
                wrtColor(swm,fq.myColor);
            }

            


            swm.WriteLine("BankTeller");
            foreach (BankTeller bt in theBankTellerList)
            {
             
                swm.WriteLine(bt.ServerName.ToString());

                wrtRvString(swm, bt.RevisibleServiceTimeData);
             
              
              //  swm.WriteLine("ReferenceQueue");
                swm.WriteLine(bt.ReferredQueue.Name.ToString());

            
                swm.WriteLine(bt.endPoint.X.ToString());
                swm.WriteLine(bt.endPoint.Y.ToString());
                //swm.WriteLine(bt.RevisibleServiceTimeData.distributionType);
                wrtColor(swm, bt.myColor);
                
            }

           

            swm.WriteLine("QueueServerRelationLine");
            for (int i = 0; i < theRelationList.Count; i++)
            {
                swm.WriteLine(theRelationList[i].startPoint.X.ToString());
                swm.WriteLine(theRelationList[i].startPoint.Y.ToString());
                swm.WriteLine(theRelationList[i].endPoint.X.ToString());
                swm.WriteLine(theRelationList[i].endPoint.Y.ToString());

            }

            swm.WriteLine("CustomerArrival");
            swm.WriteLine(createdCustomerSetInfo.CustomerNo.ToString());
            wrtRvString(swm, createdCustomerSetInfo.ArrivalRvType);
            swm.WriteLine(createdCustomerSetInfo.HasClosedTime.ToString());
            swm.WriteLine(createdCustomerSetInfo.FinalArrivalTime.ToString());

            swm.WriteLine(Entrance.endPoint.X.ToString());
           swm.WriteLine(Entrance.endPoint.Y.ToString());
           
         

   

            swm.Close();

            
        }
        
        void wrtRvString(StreamWriter sw,RandomVariableGenerator theRV) 
        {
           // sw.WriteLine(


                switch (theRV.distributionType)
                {
                    case EnumRvType.Exponential:
                        sw.WriteLine(EnumRvType.Exponential.ToString());

                        ExponentialRVGenerator ERV;
                        ERV = (ExponentialRVGenerator) theRV;
                        sw.WriteLine(ERV.Beta.ToString());
                        

                    break;
                    case EnumRvType.Gamma:
                        sw.WriteLine(EnumRvType.Gamma.ToString());                      

                        GammaRVGenerator GRV;
                        GRV = (GammaRVGenerator)theRV;
                        sw.WriteLine(GRV.Alpha.ToString());
                        sw.WriteLine(GRV.Beta.ToString());


                    break;
                    case EnumRvType.Normal:
                        sw.WriteLine(EnumRvType.Normal.ToString());
                        NormalRVGenerator NRV;
                        NRV = (NormalRVGenerator)theRV;
                        sw.WriteLine(NRV.Mu.ToString());
                        sw.WriteLine(NRV.Sigma.ToString());


                    break;
                    case EnumRvType.Triangular:
                    sw.WriteLine(EnumRvType.Triangular.ToString());
                    TriangularRVGenerator TRV = (TriangularRVGenerator)theRV;
                    sw.WriteLine(TRV.Min.ToString());
                    sw.WriteLine(TRV.Max.ToString());
                    sw.WriteLine(TRV.Peak.ToString());


                    break;

                    case EnumRvType.Weibull:
                    sw.WriteLine(EnumRvType.Weibull.ToString());

                    WeibullRVGenerator WRV = (WeibullRVGenerator)theRV;
                    sw.WriteLine(WRV.Alpha.ToString());
                    sw.WriteLine(WRV.Beta.ToString());


                    break;              

                }


        }

        void wrtColor(StreamWriter sw,Color cl)
        {
            sw.WriteLine(cl.A);
            sw.WriteLine(cl.R);
            sw.WriteLine(cl.G);
            sw.WriteLine(cl.B);
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (theBankSimulation.TheEventList.CurrentSize>0)
            buttonRunOneExecutionG_Click(null, null);
            else
                timer1.Stop();
        }

        private void checkBoxStartAnimation_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxStartAnimation.Checked)
                timer1.Stop();
         

        }

        private void loadModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "文字檔|*.MD";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                
            else
            {
                String FileString = openFileDialog1.FileName;
                try
                {
                    loadModel(FileString);

                }
                catch
                {
                    
                }
            }
        }

        void loadModel(string fs)
        {
            
            /*
            currentConfigure.CustomerNo, 
            currentConfigure.ArrivalTimeRV, 
            theBankTellerList, 
            currentConfigure.QueueStepLines, 
            currentConfigure.pieCharts, 
            currentConfigure.GanttChartSeires, 
            EventListSeries, 
            currentConfigure.myQueueBehavior, 
            createdCustomerSetInfo
            */
             
            theQueueList = new List<FcfsTimedQueue<Customer>>();
            theBankTellerList = new List<BankTeller>();
            theRelationList = new List<GraphicalElement>();

       
            StreamReader sr = new StreamReader(fs);
            String theString = sr.ReadLine();

            if (theString == "Queue")
                theString = sr.ReadLine();
            else
                MessageBox.Show("error on loading modle");
            
            while (theString != "BankTeller")
            {
             
              //  theRec = new Rectangle(nq.endPoint.X, nq.endPoint.Y, 50, 100);
                string qName = theString;
                int x = int.Parse(sr.ReadLine());
                int y = int.Parse( sr.ReadLine());
                Rectangle theRec = new Rectangle(x, y,50,100);
                FcfsTimedQueue<Customer> nq = new FcfsTimedQueue<Customer>(theRec);
                nq.Name = qName;

                nq.myColor = Color.FromArgb(int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()));

                theQueueList.Add(nq);
                theString = sr.ReadLine();
            }

            theString = sr.ReadLine();
                while (theString != "QueueServerRelationLine")
                {
                    Rectangle theRec;// = new Rectangle(thePosition.X, thePosition.Y, 50, 50);
                    string tellerName = theString;
                    RandomVariableGenerator theRv = judgeRvGeneration(sr, sr.ReadLine());
                    //int xPoint, yPoint;

                    String theReferedQueueName = sr.ReadLine();

                   // xPoint = int.Parse(sr.ReadLine());
                   // yPoint = int.Parse(sr.ReadLine());

                    theRec = new Rectangle(int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()), 50, 50);

                    BankTeller bt = new BankTeller(theRv, theRec);

                    bt.myColor = Color.FromArgb(int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()));
                    BankTeller.linkQueueToBankTeller(bt, findQueueFromString(theReferedQueueName, theQueueList));



                    theBankTellerList.Add(bt);
                    theString = sr.ReadLine();
             
                }

                theString = sr.ReadLine();
                while (theString != "CustomerArrival")
                {
                    
                    Point start,end;
                  
                    start=new Point(int.Parse(theString),int.Parse(sr.ReadLine()));
                    end= new Point(int.Parse(sr.ReadLine()),int.Parse(sr.ReadLine()));

                    GraphicalElement ge = new GraphicalElement(getRect(start, end));
                    ge.startPoint = start;
                    ge.endPoint = end;

                    theRelationList.Add(ge);

                    theString = sr.ReadLine();
                }

                createdCustomerSetInfo = new CustomerSetInfo();
                    createdCustomerSetInfo.CustomerNo = int.Parse(sr.ReadLine());
                    createdCustomerSetInfo.ArrivalRvType = judgeRvGeneration(sr, sr.ReadLine());
                    createdCustomerSetInfo.HasClosedTime = bool.Parse(sr.ReadLine());
                   createdCustomerSetInfo.FinalArrivalTime = double.Parse(sr.ReadLine());

                    Rectangle theRec2=new Rectangle(int.Parse(sr.ReadLine()),int.Parse(sr.ReadLine()),50,50);

                    Entrance= new GraphicalElement(theRec2);



                    sr.Close();
                splitContainer10.Panel1.Refresh();
        }


        RandomVariableGenerator judgeRvGeneration(StreamReader sr, String theRvType)
        {
            RandomVariableGenerator rv = null;
         
            switch (theRvType)
            {
                case "Exponential":
                    double beta = double.Parse(sr.ReadLine());
                    rv = new ExponentialRVGenerator(beta, EnumExponentialMethod.LogUniform);                   
                    break;
                case "Normal":
                    double mu = double.Parse(sr.ReadLine());
                    double stdev = double.Parse(sr.ReadLine());
                    rv = new NormalRVGenerator(mu, stdev, EnumNormalMethod.BoxMuller);
                    break;
                case "Gamma":
                    double alpha = double.Parse(sr.ReadLine());
                    double beta1 = double.Parse(sr.ReadLine());
                    rv = new GammaRVGenerator(alpha, beta1, EnumGammaMethod.AhrensTwoPartMethod);
                    break;

                case "Triangular":
                    double min = double.Parse(sr.ReadLine());
                    double max = double.Parse(sr.ReadLine());
                    double peak = double.Parse(sr.ReadLine());

                    rv = new TriangularRVGenerator(max, min, peak);
                    break;

                case "Weibull":
                    double alphaW = double.Parse(sr.ReadLine());
                    double betaW = double.Parse(sr.ReadLine());

                    rv = new WeibullRVGenerator(alphaW, betaW);
                    break;

            }


            return rv;
        }


        FcfsTimedQueue<Customer> findQueueFromString(string sn,List< FcfsTimedQueue<Customer>> lfs)
        {
            FcfsTimedQueue<Customer> theQ=null;
            for (int i = 0; i < lfs.Count; i++)
                if (sn == lfs[i].Name)
                {
                    theQ = lfs[i];
                    break;
                }


            return theQ;
        }

        private void saveAcadamicRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "txt|*.txt";
            if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            else
            {
               
                String FileString = saveFileDialog1.FileName;
                SaveResult.saveAcadamicRunData(FileString, academicRunRecordedData);

            }

        }

      
       

    }
}
