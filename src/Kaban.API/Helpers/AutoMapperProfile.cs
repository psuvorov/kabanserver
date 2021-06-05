using AutoMapper;
using Kaban.API.Controllers.Requests.Users;
using Kaban.API.Controllers.Responses.Boards;
using Kaban.API.Controllers.Responses.CardComments;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.API.Controllers.Responses.Lists;
using Kaban.API.Controllers.Responses.Users;
using Kaban.Domain.Models;
using CoverImageOrientation = Kaban.Domain.Enums.CoverImageOrientation;

namespace Kaban.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserRequest, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, AuthSuccessResponse>();

            CreateMap<Board, BoardDto>();
            CreateMap<BoardDto, Board>();
            CreateMap<Board, BoardShortInfoDto>();
            CreateMap<BoardShortInfoDto, Board>();
            CreateMap<CreateBoardDto, Board>();
            
            CreateMap<List, ListDto>();
            CreateMap<ListDto, List>();
            CreateMap<CreateListDto, List>();
            CreateMap<List, ArchivedListDto>();

            CreateMap<CoverImageOrientation, CoverImageOrientationDto>();
            CreateMap<CoverImageOrientationDto, CoverImageOrientation>();
            CreateMap<Card, CardDto>();
            CreateMap<CardDto, Card>();
            CreateMap<CreateCardDto, Card>();
            CreateMap<Card, CardDetailsDto>()
                .ForMember(dest => dest.ListName,
                    opt => opt.MapFrom(src => src.List.Name));
            CreateMap<Card, ArchivedCardDto>()
                .ForMember(dest => dest.ListName,
                    opt => opt.MapFrom(src => src.List.Name));
            
            // CreateMap<CardComment, CardCommentDto>()
            //     .ForMember(dest => dest.Username, 
            //         opt => opt.MapFrom((src => src.User.Username)));
            CreateMap<CardCommentDto, CardComment>();
            CreateMap<CreateCardCommentDto, CardComment>();


        }
    }
}