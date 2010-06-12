﻿using DevExpress.Xpo;
using eXpand.ExpressApp.ConditionalControllerState.Security;
using eXpand.ExpressApp.PivotChart.ShowInAnalysis;

namespace eXpand.ExpressApp.PivotChart.Security {
    [NonPersistent]
    public class ShowInAnalysisPermission : ControllerStateRulePermission {
        public ShowInAnalysisPermission() {
            ControllerType = typeof (ShowInAnalysisViewController).FullName;
            NormalCriteria = "1=1";
        }

        public override string ToString() {
            return string.Format("{1}: {0}", ID, GetType().Name);
        }
    }
}