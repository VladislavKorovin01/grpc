using System;
using System.Collections.Generic;

namespace EpcDataApp.GrpcService.Entities
{
    public partial class EventDeparture
    {
        public DateTime Time { get; set; }
        public int IdPath { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainIndex { get; set; } = null!;
    }
}
