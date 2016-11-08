using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class AlphaGridColumn:CalculateGridColumnAlgorithm 
    {
        double _upperht;
        double _lowerht;
        double _upperavg;
        double _loweravg;

        public AlphaGridColumn(double upperavg, double loweravg, double upperht, double lowerht)
        {
            _upperht = upperht;
            _lowerht = lowerht;
            _upperavg = upperavg;
            _loweravg = loweravg;
        }
        public override object CreateValue()
        {
            if (_upperavg == 0 || _loweravg == 0)
                return 0;
            else
            return (object)(Math.Log(_upperavg / _loweravg) / Math.Log(_upperht / _lowerht));
        }
    }
}
