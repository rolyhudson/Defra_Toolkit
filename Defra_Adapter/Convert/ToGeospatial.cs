using BH.Engine.Reflection;
using BH.oM.Adapter.Defra;
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
        public static IGeospatial ToGeospatial(object response)
        {
            if (response is CustomObject)
            {
                //custom object with samples
                CustomObject r = response as CustomObject;
                if (r.CustomData.ContainsKey("samples"))
                {
                    return ToMultiPoint(response);
                }
                
            }
            Compute.RecordError("Response was not in the expected format.");
            return null;
        }
    }
}
