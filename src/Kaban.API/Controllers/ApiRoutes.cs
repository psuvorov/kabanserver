namespace Kaban.API.Controllers
{
    public static class ApiRoutes
    {
        private const string Root = "api";

        public static class Dashboards
        {
            public const string GetUserBoards = Root + "/dashboards/get-user-boards";
            public const string GetClosedUserBoards = Root + "/dashboards/get-closed-user-boards";
        }
        
        public static class Boards
        {
            public const string GetBoard = Root + "/boards/get-board/{boardId}";
            public const string GetBoardDetails = Root + "/boards/get-board-details/{boardId}";
            public const string CreateBoard = Root + "/boards/create-board";
            public const string SetBoardWallpaper = Root + "/boards/{boardId}/set-board-wallpaper";
            public const string UpdateBoardInfo = Root + "/boards/{boardId}/update-board-info";
            public const string DeleteBoard = Root + "/boards/{boardId}/delete-board";
        }

        public static class Lists
        {
            public const string GetList = Root + "/lists/{boardId}/get-list/{listId}";
            public const string GetArchivedLists = Root + "/lists/{boardId}/get-archived-lists";
            public const string CreateList = Root + "/lists/{boardId}/create-list";
            public const string CopyList = Root + "/lists/{boardId}/copy-list/{listId}";
            public const string UpdateList = Root + "/lists/{boardId}/update-list";
            public const string ReorderLists = Root + "/lists/{boardId}/reorder-lists";
            public const string DeleteList = Root + "/lists/{boardId}/delete-list/{listId}";
        }

        public static class Cards
        {
            public const string GetCardDetails = Root + "/cards/{boardId}/get-card-details/{cardId}";
            public const string GetArchivedCards = Root + "/cards/{boardId}/get-archived-cards";
            public const string CreateCard = Root + "/cards/{boardId}/create-card";
            public const string CreateCardComment = Root + "/cards/{boardId}/create-card-comment";
            public const string SetCardCover = Root + "/cards/{boardId}/set-card-cover/{cardId}";
            public const string UpdateCard = Root + "/cards/{boardId}/update-card";
            public const string ReorderCards = Root + "/cards/{boardId}/reorder-cards";
            public const string DeleteCard = Root + "/cards/{boardId}/delete-card/{cardId}";
            public const string DeleteCardComment = Root + "/cards/{boardId}/delete-card-comment/{cardCommentId}";
        }

        public static class Users
        {
            public const string AuthenticateUser = Root + "/users/authenticate-user";
            public const string RegisterUser = Root + "/users/register-user";
        }
    }
}