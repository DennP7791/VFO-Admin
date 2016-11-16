using System.Collections.Generic;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Rules visibility of HomeView modules
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether [home module1].
        /// </summary>
        /// <value><c>true</c> if [home module1]; otherwise, <c>false</c>.</value>
        public bool HomeModule1 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [home module2].
        /// </summary>
        /// <value><c>true</c> if [home module2]; otherwise, <c>false</c>.</value>
        public bool HomeModule2 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [home module3].
        /// </summary>
        /// <value><c>true</c> if [home module3]; otherwise, <c>false</c>.</value>
        public bool HomeModule3 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [home module4].
        /// </summary>
        /// <value><c>true</c> if [home module4]; otherwise, <c>false</c>.</value>
        public bool HomeModule4 { get; set; }

        public bool HomeModule5 { get; set; }
    }

    /// <summary>
    /// TopAdmin module model
    /// </summary>
    public class TopAdminHomeModel
    {
        /// <summary>
        /// Gets or sets the customers.
        /// </summary>
        /// <value>The customers.</value>
        public List<Customer> Customers { get; set; }
        
    }

    /// <summary>
    /// Admin module model
    /// </summary>
    public class CustomerHomeModel {}

    /// <summary>
    /// Admin module model
    /// </summary>
    public class AdminHomeModel { }

    /// <summary>
    /// VFO module model
    /// </summary>
    public class UserModel
    {
        public int UserId { get; set; }
    }

    /// <summary>
    /// Helper class for customers
    /// </summary>
    public class Customer
    {
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public bool isPassword { get; set; }
    }
}