using AutoMapper;
using PD421_Dashboard_WEB_API.BLL.Dtos.Game;
using PD421_Dashboard_WEB_API.BLL.Dtos.Image;
using PD421_Dashboard_WEB_API.DAL.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.MapperProfiles
{
    public class GameMapperProfile : Profile
    {
        public GameMapperProfile()
        {
            CreateMap<GameImageEntity, GameImageDto>();
            CreateMap<UpdateGameDto, GameEntity>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateGameDto, GameEntity>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate.ToUniversalTime()));

            CreateMap<GameEntity, GameDto>()
              .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.Images.Count > 0 ? src.Images.First(i => i.IsMain) : null))
              .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Where(i => !i.IsMain))); 

        }
    }
}
