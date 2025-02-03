using AutoMapper;
using FlightDetailApi.DTO;
using FlightDetailApi.Models;

namespace FlightDetailApi.MappingDTO
{
    public class FlightMapper : Profile
    {
        public FlightMapper()
        {
            // Map between InternationalFlightDetails and FlightOutputDTO
            CreateMap<InternationalFlightDetails, FlightOutputDTO>()
                .ReverseMap(); // Enable reverse mapping

            // Map between DomesticFlightDetails and FlightOutputDTO
            CreateMap<DomesticFlightDetails, FlightOutputDTO>()
                .ReverseMap(); // Enable reverse mapping

            // Map between FlightInputDTO and InternationalFlightDetails
            CreateMap<FlightInputDTO, InternationalFlightDetails>().ReverseMap(); // Ignore unmapped fields

            // Map between FlightInputDTO and DomesticFlightDetails
            CreateMap<FlightInputDTO, DomesticFlightDetails>().ReverseMap();

            CreateMap<UserLoginDTO,UserRegistrationModel>().ReverseMap();

            CreateMap<Object,InternationalFlightDetails>().ReverseMap();
            CreateMap<Object,DomesticFlightDetails>().ReverseMap();

            // Map List<InternationalFlightDetails> to List<FlightOutputDTO>
            CreateMap<List<InternationalFlightDetails>, List<FlightOutputDTO>>()
                .ConvertUsing((source, destination, context) =>
                    source.Select(flight => context.Mapper.Map<FlightOutputDTO>(flight)).ToList());

            // CreateMap<AdminModel,AdminLoginDTO>().ReverseMap();
            // Map List<DomesticFlightDetails> to List<FlightOutputDTO>
            CreateMap<List<DomesticFlightDetails>, List<FlightOutputDTO>>()
                .ConvertUsing((source, destination, context) =>
                    source.Select(flight => context.Mapper.Map<FlightOutputDTO>(flight)).ToList());
        }
    }
}
