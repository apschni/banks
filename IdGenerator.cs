namespace Banks
{
    public class IdGenerator
    {
        private static int _accountstart = 0;
        private static int _transactionstart = 0;

        public static int AccountIdGenerate()
        {
            return _accountstart++;
        }

        public static int TransactionIdGenerate()
        {
            return _transactionstart++;
        }
    }
}