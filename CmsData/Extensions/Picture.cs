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

        public string SmallUrl => $"/Portrait/{SmallId ?? -6}?v={CreatedDate?.Ticks ?? 0}";

        public string SmallMaleUrl => $"/Portrait/{SmallId ?? -7}?v={CreatedDate?.Ticks ?? 0}";

        public string SmallFemaleUrl => $"/Portrait/{SmallId ?? -8}?v={CreatedDate?.Ticks ?? 0}";

        public string MediumUrl => $"/Portrait/{MediumId ?? -2}?v={CreatedDate?.Ticks ?? 0}";

        public string MediumMaleUrl => $"/Portrait/{MediumId ?? -4}?v={CreatedDate?.Ticks ?? 0}";

        public string MediumFemaleUrl => $"/Portrait/{MediumId ?? -5}?v={CreatedDate?.Ticks ?? 0}";

        public string LargeUrl => $"/Portrait/{LargeId ?? -2}?v={CreatedDate?.Ticks ?? 0}";

        public string LargeMaleUrl => $"/Portrait/{LargeId ?? -4}?v={CreatedDate?.Ticks ?? 0}";

        public string LargeFemaleUrl => $"/Portrait/{LargeId ?? -5}?v={CreatedDate?.Ticks ?? 0}";
    }
}
