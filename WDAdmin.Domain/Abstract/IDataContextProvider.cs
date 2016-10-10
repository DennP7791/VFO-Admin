using System.Data.Linq;

namespace WDAdmin.Domain.Abstract
{
    /// <summary>
    /// Interface definition for DataContextProvider
    /// </summary>
    public interface IDataContextProvider
    {
        /// <summary>
        /// Gets the dc.
        /// </summary>
        /// <value>The dc.</value>
        DataContext dc { get; }
    }
}