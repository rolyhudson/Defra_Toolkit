using BH.oM.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geospatial;

namespace BH.oM.Adapter.Defra
{
    public class GetSamplesRequest : BHoMObject, IRequest
    {
        public virtual BoundingBox BoundingBox { get; set; } = new BoundingBox();
    }
}
