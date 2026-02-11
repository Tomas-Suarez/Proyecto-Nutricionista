using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class ObjetivoProfile : Profile
{
    public ObjetivoProfile()
    {
        CreateMap<ObjetivoEntity, ObjetivoResponseDTO>();

        CreateMap<ObjetivoRequestDTO, ObjetivoEntity>();
    }
}