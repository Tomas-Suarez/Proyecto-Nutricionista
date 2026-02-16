using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class DietaProfile : Profile
{
    public DietaProfile()
    {

        CreateMap<DietaComidaEntity, DietaComidaResponseDTO>()
            .ForMember(dest => dest.NombreComida, opt => opt.MapFrom(src => src.Comida.Nombre))
            .ForMember(dest => dest.Momento, opt => opt.MapFrom(src => src.Momento.ToString()));


        CreateMap<DietaEntity, DietaResponseDTO>()
            .ForMember(dest => dest.PacienteNombre, opt => opt.MapFrom(src => 
                $"{src.Paciente.Nombre} {src.Paciente.Apellido}"))
            .ForMember(dest => dest.Comidas, opt => opt.MapFrom(src => src.DietaComidas));

        CreateMap<DietaRequestDTO, DietaEntity>()
            .ForMember(dest => dest.DietaComidas, opt => opt.Ignore()); 
            
        CreateMap<DietaComidaRequestDTO, DietaComidaEntity>();
    }
}