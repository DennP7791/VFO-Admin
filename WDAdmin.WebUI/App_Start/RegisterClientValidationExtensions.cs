using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(WDAdmin.WebUI.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace WDAdmin.WebUI.App_Start {

    /// <summary>
    /// Class RegisterClientValidationExtensions.
    /// </summary>
    public static class RegisterClientValidationExtensions 
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public static void Start() 
        {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}