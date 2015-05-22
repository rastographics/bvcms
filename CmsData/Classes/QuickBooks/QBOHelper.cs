using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UtilityExtensions;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Data.Qbo;

namespace CmsData.Classes.QuickBooks
{
    public class QBOHelper : QuickBooksHelper
    {
        public List<Account> ListAllAccounts() // Max per page from QuickBooks is 100
        {
            int iX;

            List<Account> lTemp;
            List<Account> lReturn = new List<Account>();

            lTemp = getDataService().FindAll(new Account(), 1, 100).ToList<Account>();

            for (iX = 0; true; iX++)
            {
                if (lTemp.Count() != 100) break;

                lReturn.InsertRange(iX * 100, lTemp);

                lTemp = getDataService().FindAll(new Account(), iX + 1, 100).ToList<Account>();
            }

            lReturn.InsertRange(iX * 100, lTemp);

            return lReturn;
        }

        public Account GetAccountByID(string sAccountID)
        {
            Account acct = new Account();
            acct.Id = new IdType { Value = sAccountID };

            return getDataService().FindById<Account>(acct) as Account;
        }

        public JournalEntryLine TranslateJournalEntry(QBJournalEntryLine qbjel, bool bCredit)
        {
            JournalEntryLine jel = new JournalEntryLine();
            jel.Desc = qbjel.sDescrition;
            jel.Amount = qbjel.dAmount;
            jel.AmountSpecified = true;
            jel.AccountId = new IdType() { Value = qbjel.sAccountID };

            if (bCredit) jel.PostingType = PostingTypeEnum.Credit;
            else jel.PostingType = PostingTypeEnum.Debit;

            jel.PostingTypeSpecified = true;

            return jel;
        }

        public new int CommitJournalEntries(string sDescription, List<QBJournalEntryLine> jelEntries)
        {
            if (jelEntries == null) return 0;
            if (jelEntries.Count() == 0) return 0;

            JournalEntryLine[] entries = new JournalEntryLine[jelEntries.Count()];

            for (int iX = 0; iX < jelEntries.Count(); iX++)
            {
                entries[iX] = TranslateJournalEntry(jelEntries[iX], jelEntries[iX].bCredit);
            }

            JournalEntry jeNew = new JournalEntry();

            JournalEntryHeader jeh = new JournalEntryHeader();
            jeh.Note = sDescription;

            jeNew.Header = jeh;
            jeNew.Line = entries;

            JournalEntry jeMade = getDataService().Add(jeNew) as JournalEntry;

            if (jeMade.Id.Value.ToInt() > 0 && jeMade.SyncToken.ToInt() > -1) return jeMade.Id.Value.ToInt();
            else return 0;
        }
    }
}
