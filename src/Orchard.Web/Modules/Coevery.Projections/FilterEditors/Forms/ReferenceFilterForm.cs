﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    public class ReferenceFilterForm : IFormProvider {
        public const string FormName = "ReferenceFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ReferenceFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var operators = new List<SelectListItem> {
                        new SelectListItem {Value = Convert.ToString(ReferenceOperator.MatchesAny), Text = T("Maches any").Text},
                        new SelectListItem {Value = Convert.ToString(ReferenceOperator.NotMatchesAny), Text = T("Not mathes any").Text}
                    };
                    return Shape.FilterEditors_ReferenceFilter(Id: FormName, Operators: operators);
                };

            context.Form(FormName, form);
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            var op = (ReferenceOperator)Enum.Parse(typeof(ReferenceOperator), (string)formState.Operator);
            string value = formState.Value;
            var items = value.Split('&').Select(int.Parse).ToArray();
            fieldName = fieldName.Split('.')[1];

            return T("");
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            var op = (ReferenceOperator) Enum.Parse(typeof (ReferenceOperator), (string) formState.Operator);
            string value = formState.Value;
            var items = value != null
                ? value.Split('&').Select(int.Parse).ToArray()
                : new[] {0};
            switch (op) {
                case ReferenceOperator.MatchesAny:
                    return x => x.In(property, items);
                case ReferenceOperator.NotMatchesAny:
                    return x => x.Not(a => a.In(property, items));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ReferenceOperator {
        MatchesAny,
        NotMatchesAny
    }
}