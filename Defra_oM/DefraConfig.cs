using BH.oM.Adapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapter.Defra
{
    [Description("Define configuration settings for pulling Defra data using the Defra Adapter")]
    public class DefraConfig : ActionConfig
    {
        public virtual int SampleFrequency { get; set; } = 5;
    }
}
