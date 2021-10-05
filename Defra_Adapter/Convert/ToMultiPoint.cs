using BH.oM.Base;
using BH.oM.Geospatial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Defra
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static MultiPoint ToMultiPoint(object response)
        {
            MultiPoint dtm = new MultiPoint();
            double x = 0;
            double y = 0;
            double altitude = 0;
            CustomObject r = response as CustomObject;
            if (r.CustomData.ContainsKey("samples"))
            {
                object samples = r.CustomData["samples"];
                if (samples is IList)
                {
                    foreach (CustomObject sample in samples as IList)
                    {
                        if (sample.CustomData.ContainsKey("location"))
                        {
                            CustomObject location = sample.CustomData["location"] as CustomObject;
                            x = System.Convert.ToDouble(location.CustomData["x"]);
                            y = System.Convert.ToDouble(location.CustomData["y"]);
                        }
                        if (sample.CustomData.ContainsKey("value"))
                        {
                            altitude = System.Convert.ToDouble(sample.CustomData["value"]);
                        }
                        dtm.Points.Add(new Point() { Longitude = x, Latitude = y, Altitude = altitude });
                    }
                }
            }
                
            return dtm;
        }
    }
}
