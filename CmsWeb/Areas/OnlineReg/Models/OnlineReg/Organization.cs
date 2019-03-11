using CmsData;
using CmsData.Codes;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        private int? _masterorgid;
        public int? masterorgid
        {
            get { return _masterorgid; }
            set
            {
                _masterorgid = value;
                if (value > 0)
                {
                    ParseSettings();
                }
            }
        }

        private int? _orgid;
        public int? Orgid
        {
            get { return _orgid; }
            set
            {
                _orgid = value;
                if (value > 0)
                {
                    CheckMasterOrg();
                    ParseSettings();
                }
            }
        }
        
        private Organization _masterOrg;
        public Organization masterorg
        {
            get
            {
                if (_masterOrg != null)
                {
                    return _masterOrg;
                }

                if (masterorgid.HasValue)
                {
                    _masterOrg = DbUtil.Db.LoadOrganizationById(masterorgid.Value);
                }

                return _masterOrg;
            }
        }

        public void CheckMasterOrg()
        {
            if (org != null && masterorgid == null &&
                (org.RegistrationTypeId == RegistrationTypeCode.UserSelects
                 || org.RegistrationTypeId == RegistrationTypeCode.ComputeOrgByAge
                 || org.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions))
            {
                _masterOrg = org;
                masterorgid = Orgid;
                _orgid = null;
                _org = null;
            }
        }

        private Organization _org;
        public Organization org
        {
            get
            {
                if (_org == null && Orgid.HasValue)
                {
                    //                    _org = Orgid == Util.CreateAccountCode
                    //                        ? CreateAccountOrg()
                    //                        : DbUtil.Db.LoadOrganizationById(Orgid.Value);
                    _org = DbUtil.Db.LoadOrganizationById(Orgid.Value);
                }
                return _org;
            }
        }
    }
}
