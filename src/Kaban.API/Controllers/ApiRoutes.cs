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
        
        public static class Boards
        {
            public const string GetBoard = Root + "/boards/get-board/{boardId}";
            public const string GetBoardDetails = Root + "/boards/get-board-details/{boardId}";
            public const string SetBoardWallpaper = Root + "/boards/set-board-wallpaper";
            public const string UpdateBoardInfo = Root + "/boards/update-board-info";
            public const string DeleteBoard = Root + "/boards/delete-board/{boardId}";
        }

        public static class Lists
        {
            public const string GetList = Root + "/lists/get-list/{boardId}/{listId}";
            public const string GetArchivedLists = Root + "/lists/get-archived-lists/{boardId}";
            public const string CreateList = Root + "/lists/create-list";
            public const string CopyList = Root + "/lists/copy-list";
            public const string UpdateList = Root + "/lists/update-list";
            public const string ReorderLists = Root + "/lists/reorder-lists";
            public const string DeleteList = Root + "/lists/delete-list/{listId}";
        }

        public static class Cards
        {
            public const string GetCardDetails = Root + "/cards/get-card-details/{boardId}/{cardId}";
            public const string GetArchivedCards = Root + "/cards/get-archived-cards/{boardId}";
            public const string CreateCard = Root + "/cards/create-card";
            public const string CreateCardComment = Root + "/cards/create-card-comment";
            public const string SetCardCover = Root + "/cards/set-card-cover";
            public const string UpdateCard = Root + "/cards/update-card";
            public const string ReorderCards = Root + "/cards/reorder-cards";
            public const string DeleteCard = Root + "/cards/delete-card/{cardId}";
            public const string DeleteCardComment = Root + "/cards/delete-card-comment/{cardCommentId}";
        }

        public static class Users
        {
            public const string AuthenticateUser = Root + "/users/authenticate-user";
            public const string RegisterUser = Root + "/users/register-user";
        }
    }
}