using AutoMapper;
using BookToFlyMVC.DTO;
using BookToFlyMVC.Models;

namespace BookToFlyMVC.MappingDTO{
    public class FlightDTOMapping: Profile
    {
        public FlightDTOMapping()
        {
            CreateMap<FlightDetailsDTO,InternationalFlightModel>().ReverseMap();
            CreateMap<FlightDetailsDTO,DomesticFlightModel>().ReverseMap();
            CreateMap<FlightSearchDTO,FlightSearchModel>().ReverseMap();
        }
    }
}