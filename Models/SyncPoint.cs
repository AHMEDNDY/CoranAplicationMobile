using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoranWarshSynchroniser.Models
{
    public class SyncPoint
    {
        public TimeSpan Time { get; set; }
        public int Page { get; set; }
        public string? ElementId { get; set; } // zone du PDF
        } 
       //var syncPoints = new List<SyncPoint>
       //{ new SyncPoint { Time = TimeSpan.FromSeconds(0), Page = 1, ElementId="ayah1" },
       //  new SyncPoint { Time = TimeSpan.FromSeconds(12), Page = 1, ElementId="ayah2" },
       //    new SyncPoint { Time = TimeSpan.FromSeconds(25), Page = 1, ElementId="ayah3" }
       //};

}
