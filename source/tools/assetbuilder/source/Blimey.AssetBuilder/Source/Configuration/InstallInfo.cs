using System;

namespace Blimey.AssetBuilder.Configuration
{
    public class InstallInfo
    {
        public Int32 InstallTime { get; set; }
        
        public DateTime InstallDateTime
        { 
            get { return DateTimeHelper.FromUnixTime(InstallTime); }
        }
    }
}

