using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Models;

namespace backend.Mapping;

public class PesajeProfile : Profile
{
    public PesajeProfile()
    {
        CreateMap<PesajeRequestDTO, PesajeEntity>()
            .ForMember(dest => dest.Fecha_Pesaje, opt => opt.MapFrom(src => 
                src.Fecha_Pesaje ?? DateTime.Now));

        CreateMap<PesajeEntity, PesajeResponseDTO>()
            .ForMember(dest => dest.Imc, opt => opt.MapFrom(src => 
                CalcularImc(src.Peso_Kg, src.Paciente.Altura_Cm)))
            .ForMember(dest => dest.ClasificacionImc, opt => opt.MapFrom(src => 
                ObtenerClasificacion(CalcularImc(src.Peso_Kg, src.Paciente.Altura_Cm))))
            .ForMember(dest => dest.DiferenciaPesoAnterior, opt => opt.Ignore());
    }

    private decimal CalcularImc(decimal peso, decimal alturaCm)
    {
        if (alturaCm <= 0)
        {
          return 0;  
        } 
        
        decimal alturaMetros = alturaCm / 100;
        return Math.Round(peso / (alturaMetros * alturaMetros), 2);
    }

    private EIMC ObtenerClasificacion(decimal imc)
    {
        return imc switch
        {
            < 18.5m => EIMC.Bajo_Peso,
            < 25.0m => EIMC.Peso_Normal,
            < 30.0m => EIMC.SobrePeso,
            _ => EIMC.Obesidad
        };
    }
}