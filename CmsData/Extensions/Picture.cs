using System.Linq;

namespace CmsData
{
    public partial class Picture
    {
        public string ThumbUrl
        {
            get
            {
                if (!ThumbId.HasValue && LargeId.HasValue)
                {
                    var large = ImageData.DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == LargeId);
                    if(large != null)
                        ThumbId = large.CreateNewTinyImage().Id;
                }
                return $"/TinyImage/{ThumbId ?? -1}?v={CreatedDate?.Ticks ?? 0}";
            }
        }
        public string SmallUrl => $"/Portrait/{SmallId ?? SmallMissingGenericId}?v={CreatedDate?.Ticks ?? 0}";

        public string SmallMaleUrl => $"/Portrait/{SmallId ?? SmallMissingMaleId}?v={CreatedDate?.Ticks ?? 0}";

        public string SmallFemaleUrl => $"/Portrait/{SmallId ?? SmallMissingFemaleId}?v={CreatedDate?.Ticks ?? 0}";

        public string MediumUrl => $"/Portrait/{MediumId ?? MediumMissingGenericId}?v={CreatedDate?.Ticks ?? 0}";

        public string MediumMaleUrl => $"/Portrait/{MediumId ?? MediumMissingMaleId}?v={CreatedDate?.Ticks ?? 0}";

        public string MediumFemaleUrl => $"/Portrait/{MediumId ?? MediumMissingFemaleId}?v={CreatedDate?.Ticks ?? 0}";

        public string LargeUrl => $"/Portrait/{LargeId ?? MediumMissingGenericId}?v={CreatedDate?.Ticks ?? 0}";

        public string LargeMaleUrl => $"/Portrait/{LargeId ?? MediumMissingMaleId}?v={CreatedDate?.Ticks ?? 0}";

        public string LargeFemaleUrl => $"/Portrait/{LargeId ?? MediumMissingFemaleId}?v={CreatedDate?.Ticks ?? 0}";

        public string PortraitBgPos => X.HasValue || Y.HasValue ? $"{X ?? 0}% {Y ?? 0}%" : "top";

        public const int SmallMissingGenericId = -6;
        public const int SmallMissingMaleId = -7;
        public const int SmallMissingFemaleId = -8;

        public const int MediumMissingGenericId = -2;
        public const int MediumMissingMaleId = -4;
        public const int MediumMissingFemaleId = -5;
    }
}
