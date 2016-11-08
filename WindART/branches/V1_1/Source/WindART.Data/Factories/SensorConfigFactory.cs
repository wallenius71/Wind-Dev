using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class SensorConfigFactory:ISensorConfigFactory 
    {
        

        
        public ISensorConfig CreateConfigType(SessionColumnType _coltype)
        {
            try
            {
                switch (_coltype)
                {
                    case SessionColumnType.WSAvg :
                        {
                            WindSpeedConfig result = new WindSpeedConfig();
                            return result;
                            
                        }
                    default:
                        {
                            SensorConfig result = new SensorConfig();
                            return result;
                            
                        }
                
                }   
                
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        
    }
}
