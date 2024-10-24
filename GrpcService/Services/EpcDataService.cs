using EpcDataApp.GrpcService;
using EpcDataEnums;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace EpcDataApp.GrpcService.Services
{
    public class EpcDataService : EpcData.EpcDataBase
    {
        private readonly TestDbContext _db;
        private readonly ILogger<EpcDataService> _logger;
        public EpcDataService(TestDbContext db, ILogger<EpcDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public override Task<WagonListResponse> GetWagons(WagonRequest request, ServerCallContext context)
        {
            try
            {
                DateTime timeStart = request.TimeStart.ToDateTime();
                DateTime timeEnd = request.TimeEnd.ToDateTime();
                request.Validate();

                IEnumerable<WagonResponse> res = (from epcEvent in _db.EpcEvents
                                                  join epc in _db.Epcs on epcEvent.IdEpc equals epc.Id
                                                  where epcEvent.Time > timeStart
                                                  where epcEvent.Time < timeEnd
                                                  join eventArrival in _db.EventArrivals on epcEvent.Time equals eventArrival.Time into setArrivals
                                                  from arrival in setArrivals.DefaultIfEmpty()
                                                  join evetDeparture in _db.EventDepartures on epcEvent.Time equals evetDeparture.Time into setDepartures
                                                  from departure in setDepartures.DefaultIfEmpty()
                                                  where epc.Type == 1
                                                  where epc.Number != "00000000"
                                                  orderby epc.Number
                                                  select new WagonResponse()
                                                  {
                                                      Number = epc.Number,
                                                      TimeArrival = arrival.Time.ToString(),
                                                      TimeDeparture = departure.Time.ToString()
                                                  }).ToList().Where(w => w.TimeArrival != null || w.TimeDeparture != null).ToList();

                var response = new WagonListResponse();
                response.Wagons.AddRange(res);

                return Task.FromResult(response);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "���������� ������ �������!"));
            }
        }
        public override Task<PathListResponse> GetPathListCrossMoveEpc(PathRequest request, ServerCallContext context)
        {
            try
            {
                string NumberEpc = request.NumberEpc;

                var wagon = _db.Epcs.AsNoTracking().FirstOrDefault(epc => epc.Number == NumberEpc);
                if (wagon == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "����� � �������� ������� �� ������!"));
                }

                var res = (from epcEvent in _db.EpcEvents
                           join eventAdd in _db.EventAdds on epcEvent.Time equals eventAdd.Time
                           join epc in _db.Epcs on epcEvent.IdEpc equals epc.Id into epcs
                           from resEpc in epcs.DefaultIfEmpty()
                           where resEpc.Type == 1
                           where resEpc.Number == NumberEpc
                           join path in _db.Paths on eventAdd.IdPath equals path.Id into paths
                           from resPath in paths.DefaultIfEmpty()
                           join park in _db.Parks on resPath.IdPark equals park.Id into parks
                           from resPark in parks.DefaultIfEmpty()
                           select new PathResponse()
                           {
                               IdPath = resPath.Id,
                               AsuNumberPath = resPath.AsuNumber,
                               IdPark = resPark.Id,
                               AsuNumberPark = resPark.AsuNumber,
                               NamePark = resPark.Name,
                               TypePark = ((TypePark)resPark.Type).ToString(),
                               DirectionPark = ((DirectionPark)resPark.Direction).ToString()
                           }).ToList();

                var response = new PathListResponse();
                response.Paths.AddRange(res);

                return Task.FromResult(response);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "���������� ������ �������"));
            }

        }
    }
}