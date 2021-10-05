using BH.oM.Adapter.Defra;
using BH.oM.Adapters.HTTP;
using BH.oM.Data.Collections;
using Geosp = BH.oM.Geospatial;
using Geom = BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Geospatial;

namespace BH.Engine.Adapter.Defra
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static List<GetRequest> GetRequests(Geosp.BoundingBox boundingBox, DefraConfig config)
        {
            //define in planar space the number of queries in x and y
            Geom.BoundingBox box = boundingBox.ToUTM();
            Domain xDom = box.BoundingBoxDomain("X");
            Domain yDom = box.BoundingBoxDomain("Y");
            double xRange = yDom.Max - yDom.Min;
            double yRange = yDom.Max - yDom.Min;

            //max grid is 32 * 32 samples approx 1000 samples
            int xBoxes = (int)Math.Ceiling(xRange / (32 * config.SampleFrequency));
            int yBoxes = (int)Math.Ceiling(yRange / (32 * config.SampleFrequency));

            //convert this to one query per grid cell
            Domain lonDom = boundingBox.Domain("Longitude");
            Domain latDom = boundingBox.Domain("Latitude");
            double lonRange = lonDom.Max - lonDom.Min;
            double latRange = latDom.Max - latDom.Min;
            //increments in longitude (x) and latitude (y)
            double xinc = lonRange / xBoxes;
            double yinc = latRange / yBoxes;

            List<GetRequest> requests = new List<GetRequest>();
            for (int x = 0; x <= xBoxes; x++)
            {
                double lonMin = x * xinc + lonDom.Min;
                double lonMax = (x + 1) * xinc + lonDom.Min;
                for (int y = 0; y <= yBoxes; y++)
                {
                    double latMin = y * yinc + latDom.Min;
                    double latMax = (y + 1) * yinc + latDom.Min;
                    requests.Add(new GetRequest()
                    {
                        BaseUrl = m_GetSamplesBaseURL,
                        Parameters = Parameters( Envelope(lonMin, latMin, lonMax, latMax) )
                    });
                }
            }
            return requests;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string Envelope(double lonMin, double latMin, double lonMax, double latMax)
        {
            //envelope in the form
            //{"xmin" : -109.55, "ymin" : 25.76, "xmax" : -86.39, "ymax" : 49.94, "spatialReference" : {"wkid" : 4326}}
            string envelope = $"{{" +
                $"\"xmin\" : {lonMin} , \"ymin\" : { latMin }," +
                $" \"xmax\" :{lonMax}, \"ymax\" : {latMax}, " +
                $"\"spatialReference\" : {{\"wkid\" : 4326}}}}";
            return envelope;
        }

        /***************************************************/

        private static Domain BoundingBoxDomain(this Geom.BoundingBox boundingBox, string axis)
        {
            switch (axis)
            {
                case "X":
                    return new Domain(boundingBox.Min.X, boundingBox.Max.X);
                case "Y":
                    return new Domain(boundingBox.Min.Y, boundingBox.Max.Y);
                case "Z":
                    return new Domain(boundingBox.Min.Z, boundingBox.Max.Z);
                default:
                    return new Domain(0,0);
            }
        }

        

        private static Dictionary<string,object> Parameters(string envelope)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("geometryType", "esriGeometryEnvelope");
            parameters.Add("returnFirstValueOnly",true);
            parameters.Add("f", "json");
            parameters.Add("geometry", envelope);
            parameters.Add("interpolation", "+RSP_BilinearInterpolation");
            return parameters;
        }

        private static string m_GetSamplesBaseURL = "https://environment.data.gov.uk/image/rest/services/SURVEY/LIDAR_Composite_1m_DTM_2020_TSR/ImageServer/getSamples?";
        private static string m_GetSamplesParameters = "&geometryType=esriGeometryEnvelope&sampleDistance=&mosaicRule=&pixelSize=&returnFirstValueOnly=true&interpolation=+RSP_BilinearInterpolation&outFields=&f=pjson";
    }
}
