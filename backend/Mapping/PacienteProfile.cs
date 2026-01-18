using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class PacienteProfile : Profile
{
    public PacienteProfile()
    {
        CreateMap<RegistroPacienteDTO, PacienteEntity>()
            .ForMember(dest => dest.Id_Paciente, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

       CreateMap<PacienteRequestDTO, PacienteEntity>()
            .ForMember(dest => dest.Id_Paciente, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Peso_Inicial, opt => opt.Ignore())
            .ForMember(dest => dest.Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

        CreateMap<PacienteEntity, PacienteResponseDTO>()
            .ConstructUsing(src => new PacienteResponseDTO(
                src.Id_Paciente,
                src.Id_Usuario,
                $"{src.Nombre} {src.Apellido}",
                src.Dni,
                src.Usuario != null ? src.Usuario.Email : string.Empty,
                src.Telefono,
                src.Altura_Cm
            ));     
    }
}
