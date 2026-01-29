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
            .ForMember(dest => dest.CaloriasProporcionales, opt => opt.MapFrom(src =>
                (int)Math.Round((src.Comida.Calorias * src.Cantidad) / 100)))
            .ForMember(dest => dest.ProteinasProporcionales, opt => opt.MapFrom(src =>
                (src.Comida.Proteinas * src.Cantidad) / 100))
            .ForMember(dest => dest.CarbohidratosProporcionales, opt => opt.MapFrom(src =>
                (src.Comida.Carbohidratos * src.Cantidad) / 100))
            .ForMember(dest => dest.GrasasProporcionales, opt => opt.MapFrom(src =>
                (src.Comida.Grasas * src.Cantidad) / 100));

        CreateMap<DietaEntity, DietaResponseDTO>()
            .ForMember(dest => dest.PacienteNombre, opt => opt.MapFrom(src =>
                $"{src.Paciente.Nombre} {src.Paciente.Apellido}"))
            .ForMember(dest => dest.ComidasDetalle, opt => opt.MapFrom(src => src.DietaComidas))
            .ForMember(dest => dest.TotalCalorias, opt => opt.MapFrom(src =>
                src.DietaComidas.Sum(dc => (dc.Comida.Calorias * dc.Cantidad) / 100)))
            .ForMember(dest => dest.TotalProteinas, opt => opt.MapFrom(src =>
                src.DietaComidas.Sum(dc => (dc.Comida.Proteinas * dc.Cantidad) / 100)));

        CreateMap<DietaRequestDTO, DietaEntity>();
        CreateMap<DietaComidaRequestDTO, DietaComidaEntity>();
    }
}
