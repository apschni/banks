using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;

namespace Banks
{
    public class Bank
    {
        public decimal DebitPercent { get; set; }
        public int UnverifiedLimit { get; set; }
        public int CreditBelowZeroLimit { get; set; } // positive number (1000 for example)
        public decimal CreditCommission { get; set; }
        private List<Account> Accounts { get; }
        private List<InterestRate> InterestRates { get; }

        public decimal CheckDepositAccountBalanceAfterTime(Account account, DateTime depositExpirationDate)
        {
            // timeskip возвращает баланс по истечении д дней
            Account checkAccount = new DepositAccount(account, depositExpirationDate);
            PayDepositPercents(); 
            
            // TODO Копируем аккаунт, считаем для него проценты и возвращаем 
        }

        public void PayInterestsAndCommissions(DateTime dateTime)
        {
            // TODO Тут считаем все комиссии и проценты для всех аккаунтов
        }
        // TODO Методы изменения процентов и комиссии, лимитов
        // TODO Подписка на уведомление об изменениях процентов и комиссии, лимитов
        // TODO Консольный интерфейс
        
        // TODO Рассчитываем и платим проценты и комиссии не для всех счетов разом, а для одного

        public void PayDebitPercents(DateTime dateTime)
        {
            foreach (Account account in Accounts)
            {
                if (!(account is DebitAccount debitAccount)) continue;
                if (debitAccount.LastDayPercentWasAddedToBalance == default)
                {
                    debitAccount.LastDayPercentWasAddedToBalance = debitAccount.History[0].DateTime;
                }

                for (DateTime curDate = debitAccount.LastDayPercentWasAddedToBalance.Date;
                    curDate <= dateTime.Date;
                    curDate = curDate.AddDays(1))
                {
                    Transaction lastTransaction =
                        debitAccount.History.LastOrDefault(transaction => transaction.DateTime.Date <= curDate);
                    if (lastTransaction == null) continue;
                    decimal curBalance = lastTransaction.BalanceAfterTransaction;
                    account.TempInterestSum += (curBalance * (DebitPercent / 36500)) +
                                       (account.TempInterestSum * (DebitPercent / 36500));
                    if (curDate.Day == 1)
                    {
                        debitAccount.AddInterest(account.TempInterestSum, dateTime);
                        account.TempInterestSum = 0;
                    }
                }
            }
        }

        public void PayDepositPercents(DateTime dateTime)
        {
            foreach (Account account in Accounts)
            {
                if (!(account is DepositAccount depositAccount)) continue;
                if (depositAccount.LastDayPercentWasAddedToBalance == default)
                {
                    depositAccount.LastDayPercentWasAddedToBalance = depositAccount.History[0].DateTime;
                }

                decimal tempInterestSum = 0;

                for (DateTime curDate = depositAccount.LastDayPercentWasAddedToBalance.Date;
                    curDate <= dateTime.Date;
                    curDate = curDate.AddDays(1))
                {
                    Transaction lastTransaction =
                        depositAccount.History.LastOrDefault(transaction => transaction.DateTime.Date <= curDate);
                    if (lastTransaction == null) continue;
                    decimal curBalance = lastTransaction.BalanceAfterTransaction;
                    tempInterestSum += (curBalance * (currentDepositPercent / 36500)) +
                                       (tempInterestSum * (currentDepositPercent / 36500));
                    if (curDate.Day == 1)
                    {
                        depositAccount.AddInterest(tempInterestSum, dateTime);
                        tempInterestSum = 0;
                    }
                }

                depositAccount.AddInterest(tempInterestSum, dateTime);
            }
        }

        public void SubtractCreditCommissions(DateTime dateTime)
        {
            foreach (Account account in Accounts)
            {
                if (!(account is CreditAccount creditAccount)) continue;
                if (creditAccount.LastDayCommissionsWasSubtracted == default)
                {
                    creditAccount.LastDayCommissionsWasSubtracted = creditAccount.History[0].DateTime;
                }

                for (DateTime curDate = creditAccount.LastDayCommissionsWasSubtracted.Date;
                    curDate <= dateTime.Date;
                    curDate = curDate.AddMonths(1))
                {
                    Transaction lastTransaction =
                        creditAccount.History.LastOrDefault(transaction => transaction.DateTime.Date <= curDate);
                    if (lastTransaction == null) continue;
                    decimal curBalance = lastTransaction.BalanceAfterTransaction;
                    if (curBalance < 0) creditAccount.SubtractCommission(CreditCommission, dateTime);
                }
            }
        }

        private decimal CalculateDepositPercent(decimal depositAccountBalance)
        {
            // TODO fixed percent by start sum
            foreach (InterestRate interestRate in InterestRates)
            {
                if (depositAccountBalance < interestRate.Limit) return interestRate.Percent;
            }

            return InterestRates.Last().Percent;
        }
    }
}