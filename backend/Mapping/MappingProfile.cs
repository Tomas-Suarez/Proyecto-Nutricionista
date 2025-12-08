using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;
using backend.Enum;

namespace backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UsuarioRequestDTO, UsuarioEntity>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol.ToString()))
                .ForMember(dest => dest.Password_Hash, opt => opt.Ignore())
                .ForMember(dest => dest.Avatar_Url, opt => opt.Ignore());

            CreateMap<UsuarioEntity, UsuarioResponseDTO>()
                .ForMember(dest => dest.Id_Usuario, opt => opt.MapFrom(src => src.Id_Usuario))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => System.Enum.Parse<ERol>(src.Rol)));
        }
    }
}