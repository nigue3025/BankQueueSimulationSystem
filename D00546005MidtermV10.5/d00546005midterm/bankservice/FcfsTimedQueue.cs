using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BankService
{
    public  class  FcfsTimedQueue <T> :GraphicalElement
    {
        Queue<T> myQueue;
        public List<string> stringTextList;
        public List<Color> QueuedColorList;
        public int ID;
        static int serialNoCount=0;
        private string name;
        double startTime = 0;
        double endTime = 0;

        public string Name
        {
            get { 
                return name; 
            }
            set { name = value; }
        }


        int sofarMaxLength =0;
        public Color ServerColor;
        double timeAverageLength = 0;


        public double TimeAverageSize
        {
          
            get
            {                
                    return timeAverageLength;
            }
            set
            {
                timeAverageLength = value;
            }

        }

        public FcfsTimedQueue()
        {
            myQueue = new Queue<T>();
           
       
        }

       

        public FcfsTimedQueue(Rectangle rc):base(rc)
        {
            myQueue = new Queue<T>();

            name = "Queue" + serialNoCount.ToString();
            serialNoCount++;
          
        }

        public T ElementAt(int index)
        {
            T[] arrayQueue=myQueue.ToArray();

            return arrayQueue[index];
        }


        public int CurrentSize
        {
            get
            {
                return myQueue.Count;
            }
      
        }

        void updateMaxLength()
        {
            if(myQueue.Count>1)
            if (myQueue.Count > sofarMaxLength)
                sofarMaxLength = myQueue.Count;

        }

     

        public int MaxLength
        {
            get
            {
                return sofarMaxLength;
            }
           
        }

        /*
        public T removeHead( double time)
        {
            startTime=endTime;
            endTime = time;

            T t = myQueue.Dequeue();
           
            return t;
        }

         */
        
        public T removeHead()
        {

             T t=myQueue.Dequeue();

            
            //ArrayQueue=myQueue.ToArray();
            return t;
        }

        public void addTail(T obj)
        {

            myQueue.Enqueue(obj);
           // ArrayQueue= myQueue.ToArray();
            updateMaxLength();
        }

        public void Clear()
        {
            sofarMaxLength = 0;
            myQueue.Clear();
        }

       


        public override void drawElement(Graphics g)
        {
            base.drawElement(g);

            g.DrawRectangle(new Pen(new SolidBrush(ServerColor), 6), bound);

            string QueueInfo="";// = this.CurrentSize.ToString()+"\n";
            string nextLine = "\n";
            g.DrawString(this.CurrentSize.ToString(), new Font(new FontFamily("Arial"), 15, new FontStyle()), new SolidBrush(Color.Black), bound.X, bound.Y);

            g.DrawString(this.name.ToString(), new Font(new FontFamily("Arial"), 10, new FontStyle()), new SolidBrush(Color.Black), bound.X+2, bound.Y-13);

            if (this.CurrentSize !=0)
            {
                
                for (int i = 0; i < stringTextList.Count; i++)
                {
                    QueueInfo +=nextLine+ stringTextList[i] ;
                    g.DrawString(QueueInfo, new Font(new FontFamily("Arial"), 15, new FontStyle()), new SolidBrush(QueuedColorList[i]), bound.X, bound.Y);
                    nextLine += "\n";
                    QueueInfo = "";
                }


                
            }
        }


    }
}
