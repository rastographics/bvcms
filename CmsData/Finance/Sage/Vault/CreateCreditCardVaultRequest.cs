
namespace CmsData.Finance.Sage.Vault
{
    internal class CreateCreditCardVaultRequest : VaultRequest
    {
        public CreateCreditCardVaultRequest(string id, string key, string expiration, string cardNumber)
            : base(id, key, "INSERT_CREDIT_CARD_DATA")
        {
            Data["EXPIRATION_DATE"] = expiration;
            Data["CARDNUMBER"] = cardNumber;
        }
    }
}
