namespace Kaban.API.Controllers
{
    public static class ApiRoutes
    {
        private const string Root = "api";

        public static class Dashboard
        {
            public const string GetUserBoards = Root + "/dashboard/get-user-boards";
            public const string CreateBoard = Root + "/dashboard/create-board";
        }
        
        public static class BoardPage
        {
            public const string GetBoard = Root + "/boardpage/get-board/{boardId}";
            public const string GetList = Root + "/boardpage/get-list/{boardId}/{listId}";
            
            public const string GetBoardDetails = Root + "/boardpage/get-board-details/{boardId}";
            public const string GetCardDetails = Root + "/boardpage/get-card-details/{boardId}/{cardId}";
            
            public const string GetArchivedLists = Root + "/boardpage/get-archived-lists/{boardId}";
            public const string GetArchivedCards = Root + "/boardpage/get-archived-cards/{boardId}";
            
            public const string CreateList = Root + "/boardpage/create-list";
            public const string CreateCard = Root + "/boardpage/create-card";
            public const string CreateCardComment = Root + "/boardpage/create-card-comment";
            
            public const string CopyList = Root + "/boardpage/copy-list";
            
            public const string SetCardCover = Root + "/boardpage/set-card-cover";
            public const string SetBoardWallpaper = Root + "/boardpage/set-board-wallpaper";
            
            public const string UpdateBoardInfo = Root + "/boardpage/update-board-info";
            public const string UpdateList = Root + "/boardpage/update-list";
            public const string UpdateCard = Root + "/boardpage/update-card";
            
            public const string RenumberLists = Root + "/boardpage/renumber-all-lists";
            public const string RenumberCards = Root + "/boardpage/renumber-all-cards";
            
            public const string DeleteBoard = Root + "/boardpage/delete-board/{boardId}";
            public const string DeleteList = Root + "/boardpage/delete-list/{listId}";
            public const string DeleteCard = Root + "/boardpage/delete-card/{cardId}";
            public const string DeleteCardComment = Root + "/boardpage/delete-card-comment/{cardCommentId}";
        }

        public static class Users
        {
            public const string AuthenticateUser = Root + "/users/authenticate-user";
            public const string RegisterUser = Root + "/users/register-user";
        }
    }
}