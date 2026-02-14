using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class CategoriaProfile : Profile
{
    public CategoriaProfile()
    {
        CreateMap<CategoriaRequestDTO, CategoriaEntity>();

        CreateMap<CategoriaEntity, CategoriaResponseDTO>();
    }
}