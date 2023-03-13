using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BankService
{
    class SaveResult
    {
        String myStringCustomer,myStringTeller,myStringOverall;

        public SaveResult(String Cusotmer,String Teller, String Overall)
        {
            myStringCustomer = Cusotmer;
            myStringTeller = Teller;
            myStringOverall = Overall;
        }


        public SaveResult()
        {

        }
   
        public static void saveAcadamicRunData(String fs, string[,] acadamicRunRecord)
        {
            //嘗試以string 陣列為output的寫入存檔方式

            StreamWriter sw = new StreamWriter(fs);

            for(int k=0;k<acadamicRunRecord.Length/3;k++)
            {
               
             char[] seperator={'\t','\n'};

             sw.WriteLine("Academic Run No " + k.ToString());

            //Write customer Data
            string[] st = acadamicRunRecord[0, k].Split(seperator);
                string aString = "";
                for (int i = 0; i < 5; i++)
                {
                    aString += st[i].ToString()+"  ";
                }
                sw.WriteLine(aString);

                aString = "";
                for (int i = 5; i < st.Length; i++)
                {
                    if (i % 4 == 1 && i != 5)
                    {
                        sw.WriteLine(aString);
                        aString = st[i].ToString()  +"\t";
                    }
                    else
                       aString += st[i] + "\t";

                }

           //Write server Data
           st = acadamicRunRecord[1, k].Split(seperator);
           aString = "";

           for (int i = 0; i < 7; i++)
           {
               aString += st[i].ToString()+"\t";
           }
           sw.WriteLine(aString);
           aString = "";
           for (int i = 7; i < st.Length; i++)
           {

               if ((i - 6) % 7 == 1 && i != 7)
               {
                   sw.WriteLine(aString);
                   aString = st[i].ToString() + "\t";
               }
               else
               {
                   aString += st[i] + "\t";
               }


           }


           st = acadamicRunRecord[2, k].Split(seperator);
           aString = "";

           sw.WriteLine(st[0]);
           sw.WriteLine(st[1]);

        }




            sw.Close();
        }


       

     


        public static void saveSingleRunData(String fs,BankSimulationEngine inputBank)
        {
            //嘗試直接從simulation class的output資料作為寫入存檔方式
            StreamWriter sw = new StreamWriter(fs);
          

            sw.WriteLine("Cust.ID\tArrivalTime\tWaitingTime\tExitTime\t (ordered by ExitTime)");
            for(int i=0;i<inputBank.TheCustomerExitList.Count;i++)
            {
             sw.WriteLine(  inputBank.TheCustomerExitList[i].ID.ToString()+"\t"+inputBank.TheCustomerExitList[i].ArrivalTime.ToString("0.00")+"\t\t"
                   +inputBank.TheCustomerExitList[i].WaitingTime.ToString("0.00") + "\t\t"
                   +inputBank.TheCustomerExitList[i].LeftTime.ToString("0.00"));
          
            }

            sw.WriteLine("");
            sw.WriteLine("Serv.ID\tServed\tQueueName\tMaxQueueSize\tTimeAvgQSz\tBusyTime\tIdleTime");
            for(int i=0;i<inputBank.TheBankTellers.Count;i++)
            {
                sw.WriteLine(
                     inputBank.TheBankTellers[i].TellerID.ToString() + "\t"+ inputBank.TheBankTellers[i].CustomerServed.ToString() + "\t"
                + inputBank.TheBankTellers[i].ReferredQueue.Name.ToString() + "\t\t"
                 + inputBank.TheBankTellers[i].MaxQueueLength + "\t\t"
                 +inputBank.TheBankTellers[i].ReferredQueue.TimeAverageSize.ToString("0.00")+"\t\t"
                 + inputBank.TheBankTellers[i].BusyTime.ToString("0.00") + "\t\t"
              + (inputBank.theBank. SytemTerminationTime - inputBank.TheBankTellers[i].BusyTime).ToString("0.00")  );

            }
            sw.WriteLine("");
            sw.WriteLine("overAllData");
            sw.WriteLine("AverageCustomerWaitingTime:"+inputBank.CustomerAverageWatingTime.ToString("0.000"));
            sw.WriteLine("AverageTellerBusyTime:"+ inputBank.ServerAverageBusyTime.ToString("0.000"));


            sw.Close();
        }

      


    }
}
