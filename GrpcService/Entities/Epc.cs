using System;
using System.Collections.Generic;

namespace EpcDataApp.GrpcService.Entities
{
    public partial class Epc
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public int Type { get; set; }
    }
}
