using System;
using System.Web.Mvc;
using Ninject;
using System.Web.Routing;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// ControllerFactory class with Ninject
    /// </summary>
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectControllerFactory"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public NinjectControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Gets the controller instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns>IController.</returns>
        protected override IController GetControllerInstance(RequestContext context, Type controllerType)
        {
            if (controllerType == null)
                return null;
            return (IController)_kernel.Get(controllerType);
        }
    }
}