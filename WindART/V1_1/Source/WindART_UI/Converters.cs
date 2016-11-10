using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data ;
using System.Windows.Controls ;
using System.Windows;
using WindART;

namespace WindART_UI
{
    public class SessionColumnConverter:IValueConverter 
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
            SelectionItem<StationSummarySheetType > sheetType = (SelectionItem<StationSummarySheetType >) value;
            if (sheetType != null)
            {
                string returnVal = Enum.GetName(typeof(StationSummarySheetType), sheetType.SelectedItem );
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
 
}
