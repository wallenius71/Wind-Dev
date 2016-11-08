using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class MergeWS:AbstractMergeWS 
    {
        
        public override double[,] Merge(Dictionary<int,Dictionary<double,List<Double>>> sourceWS)
        {
            Console.WriteLine("WindART " + sourceWS .Count );


            int elements = sourceWS.First().Value.First().Value.Count;
            Console.WriteLine(elements);

            int heights = sourceWS.Count();

            //array to hold merged data 
            double[,] result=new double[2,elements];

            bool elementSet = false;


            for (int i = 0; i < elements; i++)
            {

                for (int j = 1; j <= heights; j++)
                {
                    //Console.WriteLine(i + ", " + j);
                    double thisWS = sourceWS[j].First ().Value [i];
                   // Console.WriteLine(j + ", " + i + " ws:" + thisWS + " HT: " + sourceWS[j].First().Key);
                    if (thisWS >= 0)
                    {
                        //Console.WriteLine("                HT assigned:" + sourceWS[j].First().Key);
                       // Console.WriteLine("");
                        result[0, i] = sourceWS[j].First().Key ;
                        result[1, i] = thisWS;
                        elementSet = true;
                       break;//value has been assigned move on 
                    }

                 }
                
                if (!elementSet)
                {
                    //Console.WriteLine("                    HT assigned:" + -9999);
                    //Console.WriteLine("");
                    result[0, i] = -9999.99;
                    result[1, i] = -9999.99;

                }
            
                    elementSet = false;
               
            }

           
            return result;
        }
    }
}
