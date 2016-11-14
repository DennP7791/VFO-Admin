using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WDAdmin.WebUI.Infrastructure.ModelOperators;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Infrastructure;
using System.Configuration;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class HomeController.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;
        /// <summary>
        /// The _reflector
        /// </summary>
        private readonly ModelReflector _reflector;
        /// <summary>
        /// The _admin help file
        /// </summary>
        private readonly string _adminHelpFile = ConfigurationManager.AppSettings["AdminHelpFile"];
        /// <summary>
        /// The _vfo user help file
        /// </summary>
        private readonly string _vfoUserHelpFile = ConfigurationManager.AppSettings["VFOUserHelpFile"];

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public HomeController(IGenericRepository repository) : base(repository)
        {
            _repository = repository;
            _reflector = ModelReflector.GetInstance;
        }

        /// <summary>
        /// Homepage index page
        /// </summary>
        /// <returns>Index view</returns>
        [AuthorizeAccessAttribute("Home")]
        public ActionResult Index()
        {           
            //Reset Help variable in Session
            Session["Help"] = null;
            
            //Create HomeViewModel
            var model = new HomeViewModel();

            //Get bool properties from model and set the to values from MasterRightsModel
            var properties = _reflector.GetBoolPropertiesFromModel(model);
            model = (HomeViewModel)_reflector.SetRightsPropertiesInModel(model, properties, (MasterUserRightsModel)Session["Rights"]);

            //Prevent caching the page so users cannot use the back button when they log-out
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return View(model);
        }

        /// <summary>
        /// Homepage TopAdmin partial page
        /// </summary>
        /// <returns>TopAdminModule partial view</returns>
        [AuthorizeAccessAttribute("HomeModule1")]
        public ActionResult TopAdminModule()
        {
            var model = new TopAdminHomeModel {Customers = new List<Customer>()};

            //Get list of customers connected to therapist and put them in model
            var customers = (from cs in _repository.Get<UserGroup>()
                             where cs.UserGroupParentId != null && cs.CustomerId == null
                             select cs).ToList();


            foreach (var customer in customers.Select(cust => new Customer { CustomerId = cust.Id, CustomerName = cust.GroupName}))
            {
              /*  var userGroupVideoCatagoryCredential = (from ugvcc in _repository.Get<UserGroupVideoCatagoryCredential>()
                                                        where ugvcc.UserGroupId == customer.CustomerId
                                                        select ugvcc).SingleOrDefault();

                if(userGroupVideoCatagoryCredential == null)
                {
                    customer.isPassword = false;
                }else
                {
                    customer.isPassword = true;
                }*/
                model.Customers.Add(customer);
            }



            return PartialView(model);
        }

        /// <summary>
        /// Homepage Customer partial page
        /// </summary>
        /// <returns>Customer partial view</returns>
        [AuthorizeAccessAttribute("HomeModule2")]
        public ActionResult CustomerModule()
        {
            Session["Help"] = _adminHelpFile;
            var model = new CustomerHomeModel();
            return PartialView(model);
        }

        /// <summary>
        /// Homepage Admin partial page
        /// </summary>
        /// <returns>Admin partial view</returns>
        [AuthorizeAccessAttribute("HomeModule3")]
        public ActionResult AdminModule()
        {
            Session["Help"] = _adminHelpFile;
            var model = new AdminHomeModel();
            return PartialView(model);
        }

        /// <summary>
        /// Homepage VFO end-user page
        /// </summary>
        /// <returns>VFO end-user view</returns>
        [AuthorizeAccessAttribute("HomeModule4")]
        public ActionResult UserModule()
        {
            Session["Help"] = _vfoUserHelpFile;
            var model = new UserModel {UserId = (int) Session["UserID"]};
            return PartialView(model);
        }

        /// <summary>
        /// Webs the client.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult WebClient()
        {
            return View();
        }
    }
}