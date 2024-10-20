using System;
using System.Collections.Generic;

namespace EpcDataApp.GrpcService.Entities
{
    public partial class Path
    {
        public int Id { get; set; }
        public string AsuNumber { get; set; } = null!;
        public int IdPark { get; set; }

        public virtual Park IdParkNavigation { get; set; } = null!;
    }
}
