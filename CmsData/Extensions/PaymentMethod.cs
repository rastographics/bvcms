using UtilityExtensions;

namespace CmsData
{
    public partial class PaymentMethod
    {
        private bool? _encrypted;

        public void Decrypt()
        {
            if (!_encrypted.HasValue || _encrypted == true)
            {
                if (_CustomerId.HasValue()) _CustomerId = Util.Decrypt(_CustomerId);
                if (_Last4.HasValue()) _Last4 = Util.Decrypt(_Last4);
                if (_MaskedDisplay.HasValue()) _MaskedDisplay = Util.Decrypt(_MaskedDisplay);
                if (_NameOnAccount.HasValue()) _NameOnAccount = Util.Decrypt(_NameOnAccount);
                if (_VaultId.HasValue()) _VaultId = Util.Decrypt(_VaultId);
                _encrypted = false;
            }
        }

        public void Encrypt()
        {
            if (!_encrypted.HasValue || _encrypted == false)
            {
                if (_CustomerId.HasValue()) _CustomerId = Util.Encrypt(_CustomerId);
                if (_Last4.HasValue()) _Last4 = Util.Encrypt(_Last4);
                if (_MaskedDisplay.HasValue()) _MaskedDisplay = Util.Encrypt(_MaskedDisplay);
                if (_NameOnAccount.HasValue()) _NameOnAccount = Util.Encrypt(_NameOnAccount);
                if (_VaultId.HasValue()) _VaultId = Util.Encrypt(_VaultId);
                _encrypted = true;
            }
        }
    }
}
