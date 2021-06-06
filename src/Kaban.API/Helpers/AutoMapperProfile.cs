using AutoMapper;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Requests.CardComments;
using Kaban.API.Controllers.Requests.Cards;
using Kaban.API.Controllers.Requests.Lists;
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
            CreateMap<RegisterUserRequest, User>();
            CreateMap<User, AuthSuccessResponse>();

            CreateMap<Board, BoardResponse>();
            CreateMap<BoardResponse, Board>();
            CreateMap<Board, BoardShortInfoResponse>();
            CreateMap<BoardShortInfoResponse, Board>();
            CreateMap<CreateBoardRequest, Board>();
            
            CreateMap<List, ListResponse>();
            CreateMap<ListResponse, List>();
            CreateMap<CreateListRequest, List>();
            CreateMap<List, ArchivedListResponse>();

            CreateMap<CoverImageOrientation, CoverImageOrientationDto>();
            CreateMap<CoverImageOrientationDto, CoverImageOrientation>();
            CreateMap<Card, CardResponse>();
            CreateMap<CardResponse, Card>();
            CreateMap<CreateCardRequest, Card>();
            CreateMap<Card, CardDetailsResponse>()
                .ForMember(dest => dest.ListName,
                    opt => opt.MapFrom(src => src.List.Name));
            CreateMap<Card, ArchivedCardResponse>()
                .ForMember(dest => dest.ListName,
                    opt => opt.MapFrom(src => src.List.Name));
            
            // CreateMap<CardComment, CardCommentDto>()
            //     .ForMember(dest => dest.Username, 
            //         opt => opt.MapFrom((src => src.User.Username)));
            CreateMap<CardCommentResponse, CardComment>();
            CreateMap<CreateCardCommentRequest, CardComment>();


        }
    }
}