using System;
using System.Collections.Generic;

namespace EpcDataApp.GrpcService.Entities
{
    public partial class EventSub
    {
        public DateTime Time { get; set; }
        public int IdPath { get; set; }
        public bool Direction { get; set; }
    }
}
