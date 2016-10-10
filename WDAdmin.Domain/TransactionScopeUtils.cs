using System.Transactions;

namespace WDAdmin.Domain
{
    /// <summary>
    /// Set-up of the TransactionScope class to change the default isolation level & extend timeout
    /// Read: http://blogs.msdn.com/b/dbrowne/archive/2010/05/21/using-new-transactionscope-considered-harmful.aspx
    /// </summary>
    public static class TransactionScopeUtils
    {
        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions
                                         {
                                             IsolationLevel = IsolationLevel.ReadCommitted,
                                             Timeout = TransactionManager.MaximumTimeout
                                         };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}