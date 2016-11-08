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
using WindART;

namespace MultiConfigTool_UI
{
    /// <summary>
    /// Interaction logic for ShearGridView.xaml
    /// </summary>
    public partial class ShearGridView : UserControl
    {
        public ShearGridView()
        {
            InitializeComponent();
        }

        private void dg1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGrid dgc = (DataGrid)sender;
            UserControl uc = (UserControl)this.FindName("UC");
            ShearGridViewModel sgvm = (ShearGridViewModel)uc.DataContext;
            IAxis axis = sgvm.GridCollection.Yaxis;
            int index = int.Parse(e.PropertyName.ToString ());
            

            // Create a new template column.
            BindableDataTemplateColumn templateColumn = new BindableDataTemplateColumn();
            templateColumn.ColumnName = e.PropertyName;
            templateColumn.CellTemplate= (DataTemplate)Resources["DecimalConvert"];
            templateColumn.Header = axis.AxisValues[index];
            // ...
            // Replace the auto-generated column with the templateColumn.
            e.Column = templateColumn;

           

        }
    }
}
