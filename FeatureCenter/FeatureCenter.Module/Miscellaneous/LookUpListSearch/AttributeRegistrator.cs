﻿using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.LookUpListSearch
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(Customer)) {
               yield return new CloneViewAttribute(CloneViewType.ListView, "LookUpListViewSearch_LookupListView");
            }
            if (typesInfo.Type==typeof(Order)) {
                yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderLookUpListSearch, "1=1", "1=1", Captions.ViewMessageLookUpListSearch, Position.Bottom){ViewType = ViewType.DetailView, View = "LookUpListViewSearch_DetailView"};
                yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderLookUpListSearch, "1=1", "1=1", Captions.HeaderLookUpListSearch, Position.Top) { ViewType = ViewType.DetailView, View = "LookUpListViewSearch_DetailView" };
                yield return new NavigationItemAttribute(Captions.Miscellaneous + "Lookup ListView Search", "LookUpListViewSearch_DetailView");
                yield return new CloneViewAttribute(CloneViewType.DetailView, "LookUpListViewSearch_DetailView");
                yield return new DisplayFeatureModelAttribute("LookUpListViewSearch_DetailView");
            }
        }
    }
}