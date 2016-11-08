using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using WindART;
using System.Data;
using TurbineDataUtility.Model;


namespace MultiConfigTool_UI
{
    public class SessionColumnConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                SessionColumnType columnType = (SessionColumnType)value;
                return Enum.GetName(typeof(SessionColumnType), columnType);
            }
            else
                return SessionColumnType.Select;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class FileProgressControlVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool thisVal = (bool)value;
            if (thisVal)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class EnumToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SelectionItem<StationSummarySheetType> sheetType = (SelectionItem<StationSummarySheetType>)value;
            if (sheetType != null)
            {
                string returnVal = Enum.GetName(typeof(StationSummarySheetType), sheetType.SelectedItem);
                return returnVal;
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class NullableToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? val = (bool?)value;
            if (val == null || val == false)
                return false;
            else
                return true;
        }

        #endregion
    }
    public class DateTimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            DateTime? selectedDate = value as DateTime?;
            
            if (selectedDate != null)
            {

                string dateTimeFormat = @"dd/mm/yyyy hh:MM:ss";

                return selectedDate.Value.ToString(dateTimeFormat);

            }



            return string.Empty;

        }



        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            bool? inventoryValue = value as bool?;
            if (inventoryValue==true)
                return new SolidColorBrush(Colors.Green);
            else
                return new SolidColorBrush(Colors.Red);

        }

    }
    public class DecimalConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {



            if (value.GetType() == typeof(double))
            {
                double ret = (double)value;
                return Math.Round(ret, 3);

            }
            if (value.GetType() == typeof(int))
            {
                return value;
            }
                
            return value;


            

        }



        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotImplementedException();
        }

    }
    public class DataInventoryConverter : IValueConverter
    {

        

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
    public static class SessionColumnTypeConverter
    {
        static Dictionary<string, SessionColumnType> ColTypeLookUp = new Dictionary<string, SessionColumnType>
            {
                {"WSAvg",SessionColumnType.WSAvg },
                {"WSStd",SessionColumnType.WSStd  },
                {"WSMax",SessionColumnType.WSMax  },
                {"WSMin",SessionColumnType.WSMin },
                {"WDAvg",SessionColumnType.WDAvg  },
                {"WDStd",SessionColumnType.WDStd  },
                {"TPAvg",SessionColumnType.TempAvg  },
                {"TPStd",SessionColumnType.TempStd  },
                {"TPMax",SessionColumnType.TempMax },
                {"TPMin",SessionColumnType.TempMin },
                {"BVAvg",SessionColumnType.BVAvg  },
                {"BPAvg",SessionColumnType.BPAvg },
                {"BPStd",SessionColumnType.BPStd },
                {"BPMax",SessionColumnType.BPMax },
                {"BPMin",SessionColumnType.BPMin }


            };
        public static SessionColumnType GetWindARTColType(string coltype,string mathoperator)
        {
            return ColTypeLookUp[coltype + mathoperator];
        }
    }
    public class BinWidthVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AxisType thisVal = (AxisType)value;
            if (thisVal==AxisType.WD || thisVal==AxisType.WS )
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class RowHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IAxis axis=default(IAxis);
            DataRow dr;
            DataRowView dv;
            int  index=default(int);
            string val=string.Empty;
            ShearGridViewModel sgvm;
            foreach (object o in values)
            {
                if (o.GetType() == typeof(ShearGridViewModel))
                {
                    sgvm = (ShearGridViewModel)o;
                    axis = sgvm.GridCollection.Xaxis;
                }
                if (o.GetType() == typeof(DataRowView))
                {
                    dv = (DataRowView)o;
                    dr = dv.Row;
                    index = dr.Table.Rows.IndexOf(dr);
                }

               
               

            } 
            val = axis.AxisValues[index].ToString ();
            return val;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ColumnHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IAxis axis = default(IAxis);
            DataColumn dc;
            DataRowView dv;
            int index = default(int);
            string val = string.Empty;
            ShearGridViewModel sgvm;
            foreach (object o in values)
            {
                if (o.GetType() == typeof(ShearGridViewModel))
                {
                    sgvm = (ShearGridViewModel)o;
                    axis = sgvm.GridCollection.Yaxis;
                }
                if (o.GetType() == typeof(DataRowView))
                {
                    dv = (DataRowView)o;
                   
                    index = dv.Row.Table.Rows.IndexOf(dv.Row );
                }




            }
            val = axis.AxisValues[index].ToString();
            return val;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
