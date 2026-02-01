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
            .ForCtorParam("NombreComida", opt => opt.MapFrom(src => src.Comida.Nombre))
            .ForCtorParam("CaloriasProporcionales", opt => opt.MapFrom(src =>
                (int)Math.Round((src.Comida.Calorias * src.Cantidad) / 100)))
            .ForCtorParam("ProteinasProporcionales", opt => opt.MapFrom(src =>
                Math.Round((src.Comida.Proteinas * src.Cantidad) / 100, 2)))
            .ForCtorParam("CarbohidratosProporcionales", opt => opt.MapFrom(src =>
                Math.Round((src.Comida.Carbohidratos * src.Cantidad) / 100, 2)))
            .ForCtorParam("GrasasProporcionales", opt => opt.MapFrom(src =>
                Math.Round((src.Comida.Grasas * src.Cantidad) / 100, 2)));

        CreateMap<DietaEntity, DietaResponseDTO>()
            .ForCtorParam("PacienteNombre", opt => opt.MapFrom(src => 
                $"{src.Paciente.Nombre} {src.Paciente.Apellido}"))
            .ForCtorParam("ComidasDetalle", opt => opt.MapFrom(src => src.DietaComidas))
            .ForCtorParam("TotalCalorias", opt => opt.MapFrom(src =>
                Math.Round(src.DietaComidas.Sum(dc => (dc.Comida.Calorias * dc.Cantidad) / 100), 2)))
            .ForCtorParam("TotalProteinas", opt => opt.MapFrom(src =>
                Math.Round(src.DietaComidas.Sum(dc => (dc.Comida.Proteinas * dc.Cantidad) / 100), 2)))
            .ForCtorParam("TotalCarbohidratos", opt => opt.MapFrom(src =>
                Math.Round(src.DietaComidas.Sum(dc => (dc.Comida.Carbohidratos * dc.Cantidad) / 100), 2)))
            .ForCtorParam("TotalGrasas", opt => opt.MapFrom(src =>
                Math.Round(src.DietaComidas.Sum(dc => (dc.Comida.Grasas * dc.Cantidad) / 100), 2)));

        CreateMap<DietaRequestDTO, DietaEntity>()
            .ForMember(dest => dest.DietaComidas, opt => opt.Ignore()); 
            
        CreateMap<DietaComidaRequestDTO, DietaComidaEntity>();
    }
}