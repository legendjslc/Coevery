﻿using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Core.ViewModels;
using Orchard.Projections.Services;

namespace Coevery.Core.Controllers {
    public class FilterController : Controller {
        private readonly IProjectionManager _projectionManager;

        public FilterController(IProjectionManager projectionManager) {
            _projectionManager = projectionManager;
        }

        public ActionResult GetFieldFilters(string id) {
            if (string.IsNullOrWhiteSpace(id)) {
                return null;
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);

            string category = id + "ContentFields";
            var filters = _projectionManager.DescribeFilters()
                .Where(x => x.Category == category)
                .SelectMany(x => x.Descriptors);

            var fieldFilters = new List<FieldFilterViewModel>();
            foreach (var filter in filters) {
                string name = filter.Name.Text;
                string displayName = name.Substring(0, name.Length - ":Value".Length);
                fieldFilters.Add(new FieldFilterViewModel {
                    DisplayName = displayName,
                    FormName = filter.Form,
                    Type = filter.Type
                });
            }
            return Json(fieldFilters, JsonRequestBehavior.AllowGet);
        }
    }
}