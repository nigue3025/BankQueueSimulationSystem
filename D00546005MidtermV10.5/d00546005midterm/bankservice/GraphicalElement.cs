using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BankService
{
    public class GraphicalElement
    {
        
        public Rectangle bound = Rectangle.Empty;
        public Color myColor;
        Color OriginalColor;
        Random rnd = new Random();
        //public Random staticRnd;
        public Point startPoint;
        public Point endPoint;
        public bool hasRelation=false;
        public Font panelFont;


        public GraphicalElement()
        {
            
        }
        
        public GraphicalElement(Random staticRnd)
        {

            myColor = Color.FromArgb(255, staticRnd.Next(256), staticRnd.Next(256), staticRnd.Next(256));
            endPoint = new Point(bound.Left, bound.Top);
        }

        //draw line only
        public  GraphicalElement(Rectangle rec)
        {
            bound = rec;

           // startPoint = new Point(rec.X, rec.Y);
           // endPoint=new Point(rec.

            myColor = Color.FromArgb(255, rnd.Next(256),rnd.Next(256), rnd.Next(256));
            OriginalColor = myColor;
        }

      

      
       
        public virtual enumClickType CheckIsClicked(Point pt)
        {
            if (bound.Contains(pt)) return enumClickType.inside;

            return enumClickType.none;
        }

       
        
      


        public virtual void drawElement(Graphics g)
        {
            g.FillRectangle(new SolidBrush(myColor), bound);
            endPoint = new Point(bound.Left, bound.Top);
          // g.FillRectangle(new SolidBrush(Color.Green),g);
        }

        public virtual void drawLine(Graphics g)
        {
            g.DrawLine(new Pen(new SolidBrush(Color.Black), 2), startPoint,endPoint);
           

            hasRelation = true;
        }

        public virtual void drawCircle(Graphics g)
        {
         
            g.FillEllipse(new SolidBrush(Color.SeaGreen),bound);

            g.DrawString("Entrance", new Font(new FontFamily("Arial"), 8, new FontStyle()), new SolidBrush(Color.Black), bound.X+5, bound.Y+15);

            endPoint.Y = bound.Top;
            endPoint.X = bound.Left;
        }

        public virtual void drawLine()
        {
            

        }

    }
}
