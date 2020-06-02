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
                _Last4 = Util.Decrypt(_Last4);
                _MaskedDisplay = Util.Decrypt(_MaskedDisplay);
                _NameOnAccount = Util.Decrypt(_NameOnAccount);
                _VaultId = Util.Decrypt(_VaultId);
                _encrypted = false;
            }
        }

        public void Encrypt()
        {
            if (!_encrypted.HasValue || _encrypted == false)
            {
                _Last4 = Util.Encrypt(_Last4);
                _MaskedDisplay = Util.Encrypt(_MaskedDisplay);
                _NameOnAccount = Util.Encrypt(_NameOnAccount);
                _VaultId = Util.Encrypt(_VaultId);
                _encrypted = true;
            }
        }
    }
}
