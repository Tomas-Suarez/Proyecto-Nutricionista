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
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.PatologiaPacientes, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Objetivo, opt => opt.MapFrom(src => src.IdObjetivo));

        CreateMap<PacienteRequestDTO, PacienteEntity>()
            .ForMember(dest => dest.Id_Paciente, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Peso_Inicial, opt => opt.Ignore())
            .ForMember(dest => dest.Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.PatologiaPacientes, opt => opt.Ignore());

        CreateMap<PacienteEntity, PacienteResponseDTO>()
            .ConstructUsing((src, context) => new PacienteResponseDTO(
                src.Id_Paciente,
                src.Id_Usuario,
                src.Nombre,
                src.Apellido,
                src.Dni,
                src.Usuario != null ? src.Usuario.Email : string.Empty,
                src.Usuario?.Avatar_Url,
                src.Telefono,
                src.Genero,
                src.Peso_Inicial,
                src.Altura_Cm,
                src.Estado.ToString(),
                src.Objetivo != null ? context.Mapper.Map<ObjetivoResponseDTO>(src.Objetivo) : null,
                src.PatologiaPacientes.Select(pp =>
                    context.Mapper.Map<PatologiaResponseDTO>(pp.Patologia)
                ).ToList()
            ));
    }
}