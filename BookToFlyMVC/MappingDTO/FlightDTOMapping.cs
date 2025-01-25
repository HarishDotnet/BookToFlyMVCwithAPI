using AutoMapper;
using BookToFlyMVC.DTO;
using BookToFlyMVC.Models;
using FlightDetailsApi.Models;
namespace BookToFlyMVC.MappingDTO{
    public class FlightDTOMapping: Profile
    {
        public FlightDTOMapping()
        {
            CreateMap<FlightDetailsDTO,InternationalFlightDetails>().ReverseMap();
            CreateMap<FlightDetailsDTO,DomesticFlightDetails>().ReverseMap();
            CreateMap<FlightSearchDTO,FlightSearchInput>().ReverseMap();
            CreateMap<LoginViewModel,LoginDTO>().ReverseMap();
        }
    }
}