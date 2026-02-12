using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class ComidaProfile : Profile
{
    public ComidaProfile()
    {
        CreateMap<ComidaEntity, ComidaResponseDTO>();

        CreateMap<CategoriaEntity, CategoriaResponseDTO>();
        
        CreateMap<ComidaRequestDTO, ComidaEntity>()
            .ForMember(dest => dest.Categorias, opt => opt.Ignore());
    }
}