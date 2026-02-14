using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;
using System.Linq;

namespace backend.Mapping;

public class PacienteProfile : Profile
{
    public PacienteProfile()
    {
        CreateMap<RegistroPacienteDTO, PacienteEntity>()
            .ForMember(dest => dest.Id_Paciente, opt => opt.Ignore())
            .ForMember(dest => dest.Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Objetivo, opt => opt.Ignore())
            .ForMember(dest => dest.PatologiaPacientes, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Objetivo, opt => opt.MapFrom(src => src.IdObjetivo))

            .ForMember(dest => dest.TokenAcceso, opt => opt.Ignore())
            .ForMember(dest => dest.CodigoAcceso, opt => opt.Ignore());


        CreateMap<PacienteRequestDTO, PacienteEntity>()
            .ForMember(dest => dest.Id_Paciente, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Peso_Inicial, opt => opt.Ignore())
            .ForMember(dest => dest.Nutricionista, opt => opt.Ignore())
            .ForMember(dest => dest.Objetivo, opt => opt.Ignore())
            .ForMember(dest => dest.PatologiaPacientes, opt => opt.Ignore())
            .ForMember(dest => dest.TokenAcceso, opt => opt.Ignore())
            .ForMember(dest => dest.CodigoAcceso, opt => opt.Ignore());


        CreateMap<PacienteEntity, PacienteResponseDTO>()
            .ConstructUsing((src, context) => new PacienteResponseDTO(
                src.Id_Paciente,
                src.Nombre,
                src.Apellido,
                src.Dni,
                src.Email ?? string.Empty,
                src.Telefono,
                src.Genero,
                src.Peso_Inicial,
                src.Altura_Cm,
                src.Estado.ToString(),
                src.TokenAcceso,
                src.CodigoAcceso,
                src.Objetivo != null ? context.Mapper.Map<ObjetivoResponseDTO>(src.Objetivo) : null,
                src.PatologiaPacientes.Select(pp =>
                    context.Mapper.Map<PatologiaResponseDTO>(pp.Patologia)
                ).ToList()
            ));
    }
}