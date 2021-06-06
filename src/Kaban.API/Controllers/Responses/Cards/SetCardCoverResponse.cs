namespace Kaban.API.Controllers.Responses.Cards
{
    public class SetCardCoverResponse
    {
        public string CoverImagePath { get; set; }
        public CoverImageOrientationDto ImageOrientation { get; set; }
    }
}