using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackBone
{
    public class UDDefinitions
    {

        public string ud1 { get; set; }
        public string ud2 { get; set; }
        public string ud3 { get; set; }
        public string ud4 { get; set; }
        public string ud5 { get; set; }

     


    }

    public struct LicenseInfo
    {
        public int MaxNoAccts;

        public int MaxNoDmains;

        public int MaxNoAffs;

        public int MaxNoUsers;

        public int MaxTransVolume;

        public DateTime ExpiryDate;

        public string ModulesToDeactivate;

        public bool IsInactive;
    }
}
