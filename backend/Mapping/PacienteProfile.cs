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
            .ForMember(dest => dest.Id_Objetivo, opt => opt.MapFrom(src => src.IdObjetivo))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<PacienteRequestDTO, PacienteEntity>()
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<PacienteEntity, PacienteResponseDTO>()
            .ConstructUsing((src, context) => 
            {
                decimal pesoActual = src.Peso_Inicial;
                
                if (src.Pesajes != null && src.Pesajes.Any())
                {
                    pesoActual = src.Pesajes.OrderByDescending(p => p.Fecha_Pesaje).First().Peso_Kg;
                }

                decimal imc = 0;
                if (src.Altura_Cm > 0)
                {
                    decimal alturaMetros = src.Altura_Cm / 100m;
                    imc = pesoActual / (alturaMetros * alturaMetros);
                    imc = Math.Round(imc, 1);
                }

                var objetivoDto = src.Objetivo != null 
                    ? context.Mapper.Map<ObjetivoResponseDTO>(src.Objetivo) 
                    : null;

                var patologiasDto = src.PatologiaPacientes != null
                    ? src.PatologiaPacientes.Select(pp => context.Mapper.Map<PatologiaResponseDTO>(pp.Patologia)).ToList()
                    : new List<PatologiaResponseDTO>();

                return new PacienteResponseDTO(
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
                    pesoActual,
                    imc,
                    objetivoDto,
                    patologiasDto
                );
            });
    }
}