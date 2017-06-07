using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2
{
    public static class CachedKeyEntry
    {

        public static Guid YearDurationID { get { return Guid.ParseExact("{BA74E352-0AD5-424B-BF31-738BA5666649}", "B"); } }
        public static Guid HalftYearDurationID { get { return Guid.ParseExact("{24847114-90E4-483D-B290-97781C3FA0C2}", "B"); } }
        public static Guid SeasonDurationID { get { return Guid.ParseExact("{BD18C4F4-6552-4986-AB4E-BA2DFFDED2B3}", "B"); } }
        public static Guid MonthDurationID { get { return Guid.ParseExact("{D48AA438-AD71-4419-A2A2-A1C390F6C097}", "B"); } }



    }
}