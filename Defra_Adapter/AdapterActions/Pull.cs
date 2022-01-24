/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapter;
using BH.oM.Data.Requests;
using BH.oM.Adapters.HTTP;
using BH.oM.Adapter.Defra;
using BH.Engine.Adapter.Defra;
using BH.oM.Geospatial;

namespace BH.Adapter.Defra
{
    public partial class DefraAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (!(actionConfig is DefraConfig))
                actionConfig = new DefraConfig();
           
            //only one request defined and handled currently
            if (request is GetSamplesRequest)
            {
                Response defraResponse = new Response();
                MultiPoint dtm = new MultiPoint();
                foreach (object response in Pull(request as GetSamplesRequest, actionConfig as DefraConfig))
                {
                    IGeospatial geospatial = Convert.ToGeospatial(response);
                    if(geospatial is MultiPoint)
                    {
                        dtm.Points.AddRange((geospatial as MultiPoint).Points);
                    }
                }
                defraResponse.FeatureCollection.Features.Add(new Feature() { Geometry = dtm });

                return new List<object> { defraResponse };
            }
            Engine.Base.Compute.RecordError("This type of request is not supported.");
            return new List<object>();
        }
            
        /***************************************************/

        public IEnumerable<object> Pull(GetSamplesRequest request, DefraConfig config)
        {
            List<GetRequest> getRequests = BH.Engine.Adapter.Defra.Create.GetRequests(request.BoundingBox, config);
            List<object> responses = new List<object>();
            foreach (GetRequest g in getRequests)
                responses.Add(m_HTTPAdapter.Pull(g).First());
            return responses;
        }
    }
}
