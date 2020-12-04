using AutoMapper;
using KabanServer.Dto.Boards;
using KabanServer.Dto.CardComments;
using KabanServer.Dto.Cards;
using KabanServer.Dto.Lists;
using KabanServer.Dto.Users;
using KabanServer.Entities;

namespace KabanServer.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, AuthenticatedUserDto>();

            CreateMap<Board, BoardDto>();
            CreateMap<BoardDto, Board>();
            CreateMap<Board, BoardShortInfoDto>();
            CreateMap<BoardShortInfoDto, Board>();
            CreateMap<CreateBoardDto, Board>();
            
            CreateMap<List, ListDto>();
            CreateMap<ListDto, List>();
            CreateMap<CreateListDto, List>();
            CreateMap<List, ArchivedListDto>();

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