﻿using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveFiltering
{
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderRecursiveFiltering, "1=1", "1=1",
        Captions.ViewMessageRecursiveFiltering, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderRecursiveFiltering, "1=1", "1=1",
        Captions.HeaderRecursiveFiltering, Position.Top)]
    [XpandNavigationItem(Module.Captions.ListViewCotrol + Module.Captions.TreeListView + "RecursiveFiltering", "FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveFiltering.RFCustomer_ListView")]
    [DisplayFeatureModel("FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveFiltering.RFCustomer_ListView", "RecursiveFiltering")]
    public class RFCustomer:CustomerBase,ICategorizedItem
    {
        public RFCustomer(Session session) : base(session) {
        }

        ITreeNode ICategorizedItem.Category {
            get { return Category; }
            set { Category=value as RFCategory; }
        }
        private RFCategory _rfCategory;

        [Association("RFCategory-RFCustomers")]
        public RFCategory Category {
            get { return _rfCategory; }
            set { SetPropertyValue("Category", ref _rfCategory, value); }
        }
    }
}
