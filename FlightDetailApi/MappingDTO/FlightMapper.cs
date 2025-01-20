using AutoMapper;
using FlightDetailsApi.DTO;
using FlightDetailsApi.Models;

namespace FlightDetailsApi.MappingDTO
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
            CreateMap<FlightInputDTO, InternationalFlightDetails>()
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore()); // Ignore unmapped fields

            // Map between FlightInputDTO and DomesticFlightDetails
            CreateMap<FlightInputDTO, DomesticFlightDetails>();

            // Map List<InternationalFlightDetails> to List<FlightOutputDTO>
            CreateMap<List<InternationalFlightDetails>, List<FlightOutputDTO>>()
                .ConvertUsing((source, destination, context) =>
                    source.Select(flight => context.Mapper.Map<FlightOutputDTO>(flight)).ToList());

            // Map List<DomesticFlightDetails> to List<FlightOutputDTO>
            CreateMap<List<DomesticFlightDetails>, List<FlightOutputDTO>>()
                .ConvertUsing((source, destination, context) =>
                    source.Select(flight => context.Mapper.Map<FlightOutputDTO>(flight)).ToList());
        }
    }
}
