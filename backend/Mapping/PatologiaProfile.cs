using AutoMapper;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Models;

namespace backend.Mapping;

public class PatologiaProfile : Profile
{
    public PatologiaProfile()
    {
        CreateMap<PatologiaEntity, PatologiaResponseDTO>();

        CreateMap<PatologiaRequestDTO, PatologiaEntity>();
    }
}