using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;


namespace MultiConfigTool_UI
{
    public static class ItemSelectedBehavior
    {
        public static DependencyProperty ItemSelectedBehaviorProperty = DependencyProperty.RegisterAttached(
            "ItemSelected",
            typeof(RelayCommand ),
            typeof(ItemSelectedBehavior),
            new UIPropertyMetadata(ItemSelectedBehavior.ItemSelectedChanged));

        public static void SetItemSelected(DependencyObject target, ICommand value)
        {
            target.SetValue(ItemSelectedBehavior.ItemSelectedBehaviorProperty, value);
        }

        private static void ItemSelectedChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Selector element = target as Selector;

            if (element != null)
            {
                // If we're putting in a new command and there wasn't one already
                // hook the event
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    element.SelectionChanged += element_SelectionChanged;
                }
                // If we're clearing the command and it wasn't already null
                // unhook the event
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    element.SelectionChanged -= element_SelectionChanged;
                }
            }
        }
        private static void element_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DependencyObject  element = (DependencyObject)e.OriginalSource ;
            if (element != null)
            {
                ICommand command = (ICommand)element.GetValue(ItemSelectedBehavior.ItemSelectedBehaviorProperty);
                command.Execute(element);
            }
        }
            
    }
            


        


    
}
