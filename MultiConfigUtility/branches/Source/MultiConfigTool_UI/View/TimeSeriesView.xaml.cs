using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visifire.Charts;
using TurbineDataUtility.Model;

namespace MultiConfigTool_UI
{
    /// <summary>
    /// Interaction logic for TimeSeriesView.xaml
    /// </summary>
    public partial class TimeSeriesView : UserControl
    {
        public TimeSeriesView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(TimeSeriesView_Loaded);           
        }

        void TimeSeriesView_Loaded(object sender, RoutedEventArgs e)
        {
            TagGroup a=(TagGroup)this.DataContext;
            Chart TimeSeriesSample = new Chart();
            TimeSeriesSample.Titles.Add(new Title() { Text = a.Key });
            TimeSeriesSample.Height = 400;
            TimeSeriesSample.Width=900;
            TimeSeriesSample.Style = (Style)FindResource("TimeSeriesStyle");

            
            
            
            
            TimeSeriesSample.AxesX.Add(new Axis() { IntervalType = IntervalTypes.Minutes, Interval=30, 
                ValueFormatString ="MM/dd/yyyy hh:mm"});

            AxisLabels xaxislabel = new AxisLabels();
            xaxislabel.Angle = -90;
            TimeSeriesSample.AxesX[0].AxisLabels = xaxislabel;
            TimeSeriesSample.AxesY.Add(new Axis() { AxisMinimum = 0 });
            

            TimeSeriesSample.ScrollingEnabled = true;
            TimeSeriesSample.ZoomingEnabled = false;

            foreach (Tag tg in a)
            {
                
                DataSeries ds = new DataSeries();
                ds.SelectionEnabled = true;

                ds.MarkerScale = .5;
                ds.XValueType = ChartValueTypes.DateTime;
                ds.LegendText = tg.TagName;

                ds.XValueFormatString = "MM/dd/yyyy hh:mm";

                ds.RenderAs = RenderAs.QuickLine ;

                if (!TimeSeriesSample.Series.Contains(ds))
                    TimeSeriesSample.Series.Add(ds);


                foreach (KeyValuePair<DateTime, double> kv in tg.Data)
                {
                    DataPoint dp = new DataPoint() { XValue = kv.Key, YValue = kv.Value };
                    if (!TimeSeriesSample.Series[a.IndexOf(tg)].DataPoints.Contains(dp))
                        TimeSeriesSample.Series[a.IndexOf(tg)].DataPoints.Add(dp);

                }
            }
            LayoutRoot.Children.Add(TimeSeriesSample);


            

        }


       
        

        

       
    }
}
