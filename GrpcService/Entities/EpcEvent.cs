using System;
using System.Collections.Generic;

namespace EpcDataApp.GrpcService.Entities
{
    public partial class EpcEvent
    {
        public DateTime Time { get; set; }
        public int IdPath { get; set; }
        public int Type { get; set; }
        public int NumberInOrder { get; set; }
        public int IdEpc { get; set; }
    }
}
