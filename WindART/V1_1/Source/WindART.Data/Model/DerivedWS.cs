using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;
using System.Reflection;


namespace WindART
{
    public class DerivedWS : AbstractDeriveWS
    {
        public override List<double> DeriveNewWS(ISessionColumnCollection collection, AbstractAlpha alpha,
            double[,] sourceWS, DataTable data, double derivedHt)
        {
            SessionColumnCollection colclxn = (SessionColumnCollection)collection;
            int rowcount = data.Rows.Count;
            int xAxisColIndex = default(int);
            int yAxisColIndex = default(int);

            List<double> newDerivedWS = new List<double>(rowcount);
            //dynamically create list for xAxis source data

            if (alpha.Xaxis != null && alpha.Yaxis == null)
            {
                AssignColIndex(alpha.Xaxis, (SessionColumnCollection)collection);

                Type t = Type.GetType(collection.Columns[alpha.Xaxis.SessionColIndex].ColumnType.ToString());

                MethodInfo mi = typeof(WindART.Utils).GetMethod("ExtractDataTableColumn", new Type[] { typeof(int), typeof(DataTable) });
                MethodInfo mi1 = mi.MakeGenericMethod(new Type[] { t });
                dynamic xAxisSource = mi1.Invoke(null, new object[] { alpha.Xaxis.SessionColIndex, data });

                //only 1 axis
                for (int i = 0; i <= rowcount; i++)
                {
                    double thisAlpha = alpha.Alpha[alpha.Xaxis.GetAxisValue(xAxisSource[i]), 0];
                    newDerivedWS[i] = sourceWS[1, i] * Math.Pow((derivedHt / sourceWS[0, i]), thisAlpha);

                }

                return newDerivedWS;

            }
            if (alpha.Xaxis != null && alpha.Yaxis != null)
            {
                AssignColIndex(alpha.Xaxis, (SessionColumnCollection)collection);
                AssignColIndex(alpha.Yaxis, (SessionColumnCollection)collection);

                MethodInfo miX = typeof(WindART.Utils).GetMethod("ExtractDataTableColumn", new Type[] { typeof(int), typeof(DataTable) });
                //Console.WriteLine(miX.ToString());
                //x
                //Console.WriteLine(" idx passed in...");
                //Console.WriteLine(alpha.Xaxis.SessionColIndex);

                Type tX = Type.GetType(data.Columns[alpha.Xaxis.SessionColIndex].DataType.ToString());
                //Console.WriteLine(tX.ToString ());

                MethodInfo miX1 = miX.MakeGenericMethod(new Type[] { tX });
                //Console.WriteLine("just before invoke..." + miX1.ToString ());
                dynamic xAxisSource = miX1.Invoke(null, new object[] { alpha.Xaxis.SessionColIndex, data });
                //Console.WriteLine(" xsource " + xAxisSource.Count);
                //y
                Type tY = Type.GetType(data.Columns[alpha.Yaxis.SessionColIndex].DataType.ToString());
                MethodInfo miY1 = miX.MakeGenericMethod(new Type[] { tY });
                dynamic yAxisSource = miY1.Invoke(null, new object[] { alpha.Yaxis.SessionColIndex, data });
                //Console.WriteLine(" ysource " + yAxisSource.Count);

                Console.WriteLine(alpha.Alpha.GetUpperBound(0) + "   " + alpha.Alpha.GetUpperBound(1));
                //2 axes
                for (int i = 0; i <= rowcount - 1; i++)
                {
                    //Console.WriteLine(xAxisSource[i]);
                    // Console.WriteLine(" wd " + alpha.Xaxis.GetAxisValue(xAxisSource[i]).ToString());
                    // Console.WriteLine("                      hour " + alpha.Yaxis.GetAxisValue(yAxisSource[i]).ToString());
                    int x; int y;
                    if (alpha.Xaxis.AxisType != AxisType.WS)
                        x = (int)alpha.Xaxis.GetAxisValue(xAxisSource[i]);
                    else
                        x = (int)alpha.Xaxis.GetAxisValue(sourceWS[1, i]);

                    if (alpha.Yaxis.AxisType != AxisType.WS)
                        y = (int)alpha.Yaxis.GetAxisValue(yAxisSource[i]);
                    else
                        y = (int)alpha.Yaxis.GetAxisValue(sourceWS[1, i]);

                    if (x < 0 || y < 0)
                    {
                        newDerivedWS.Add(-9999.99);
                    }
                    else
                    {
                        double thisAlpha = default(double);
                        try
                        {
                            thisAlpha = alpha.Alpha[x, y];
                        }
                        catch
                        {
                            //if there is no bin for the ws encountered use a bulk shear
                            thisAlpha = alpha.AllAlphaAvg ;
                        }
                        if (sourceWS[1, i] < 0)
                            newDerivedWS.Add(-9999.99);
                        else
                            newDerivedWS.Add(sourceWS[1, i] * Math.Pow((derivedHt / sourceWS[0, i]), thisAlpha));
                    }

                }

                return newDerivedWS;
            }

            return null;
        }

        public override List<double> DeriveNewWS(ISessionColumnCollection collection,
            AbstractAlpha alpha, double[,] sourceWS, Dictionary<string, List<double>> axisSourceData, double derivedHt)
        {
            throw new NotImplementedException();
        }

        private void AssignColIndex(IAxis axis, SessionColumnCollection collection)
        {
            AxisType type = axis.AxisType;

            switch (type)
            {
                case AxisType.Month:
                    {

                        axis.SessionColIndex = collection.DateIndex;
                        break;

                    }
                case AxisType.Hour:
                    {

                        axis.SessionColIndex = collection.DateIndex;
                        break;

                    }
                case AxisType.WD:
                    {

                        axis.SessionColIndex = collection.WDComp(collection.DataSetStart);
                        break;
                    }
                case AxisType.WS:
                    {


                        axis.SessionColIndex = collection.UpperWSComp(collection.DataSetStart);
                        break;
                    }
                default:
                    axis.SessionColIndex = 0;
                    break;
            }

        }
    }
}

