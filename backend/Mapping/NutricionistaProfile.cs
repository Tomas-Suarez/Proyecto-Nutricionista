using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class NutricionistaProfile : Profile
{
    public NutricionistaProfile()
    {
        CreateMap<RegistroNutricionistaDTO, NutricionistaEntity>()
            .ForMember(dest => dest.Id_Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

        CreateMap<NutricionistaRequestDTO, NutricionistaEntity>()
            .ForMember(dest => dest.Id_Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

        CreateMap<NutricionistaEntity, NutricionistaResponseDTO>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"));
    }
}
