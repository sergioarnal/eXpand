﻿using DevExpress.Data.Summary;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Model.Options;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;
using Xpand.Persistent.Base.ModelAdapter;
using GridViewModelSynchronizer = Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model.GridViewModelSynchronizer;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model {
    public class GridViewLstEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public GridViewLstEditorDynamicModelSynchronizer(XpandGridListEditor columnViewEditor)
            : base(columnViewEditor, columnViewEditor.Model) {
            ModelSynchronizerList.Add(new GridViewViewOptionsSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new GridViewColumnOptionsSynchroniser(columnViewEditor));
            ModelSynchronizerList.Add(new XpandGridSummaryModelSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer((DevExpress.XtraGrid.Views.Base.ColumnView)columnViewEditor.GridView, columnViewEditor.Model));
        }
    }

    public class GridViewViewOptionsSynchronizer : ComponentSynchronizer<DevExpress.XtraGrid.Views.Grid.GridView, IModelOptionsGridView> {
        public GridViewViewOptionsSynchronizer(XpandGridListEditor control)
            : base((DevExpress.XtraGrid.Views.Grid.GridView)control.GridView, control.Model.GridViewOptions, ((IColumnViewEditor)control).OverrideViewDesignMode) {
        }
    }

    public class GridViewColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<XpandGridListEditor, IModelListViewOptionsGridView, IModelColumnOptionsGridView> {
        public GridViewColumnOptionsSynchroniser(XpandGridListEditor control)
            : base(control, control.Model) {
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return (DevExpress.XtraGrid.Views.Base.ColumnView)Control.GridView;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsGridView modelColumnOptionsView) {
            return modelColumnOptionsView.OptionsColumnGridView;
        }
    }

    public class XpandGridListEditorSynchronizer : ListEditorModelSynchronizer {
        public XpandGridListEditorSynchronizer(XpandGridListEditor gridListEditor)
            : base(gridListEditor) {
            ModelSynchronizerList.Add(new XpandGridViewModelSynchronizer(gridListEditor));
        }
    }

    public class XpandGridViewModelSynchronizer : GridViewModelSynchronizer {
        public XpandGridViewModelSynchronizer(XpandGridListEditor columnViewEditor)
            : base(columnViewEditor) {
        }
    }
    public class XpandGridSummaryModelSynchronizer : GridSummaryModelSynchronizer {
        public XpandGridSummaryModelSynchronizer(XpandGridListEditor xpandGridListEditor)
            : base((DevExpress.XtraGrid.Views.Grid.GridView)xpandGridListEditor.GridView, xpandGridListEditor.Model) {
        }

        protected override void RemoveGroupSummaryForProtectedColumns() {
            for (int i = Control.GroupSummary.Count - 1; i >= 0; i--) {
                ISummaryItem item = Control.GroupSummary[i];
                foreach (GridColumn column in Control.Columns) {
                    var xafGridColumn = column as XafGridColumn;
                    if ((xafGridColumn != null) && xafGridColumn.FieldName == item.FieldName && !new XafGridColumnWrapper(xafGridColumn).AllowGroupingChange) {
                        Control.GroupSummary.RemoveAt(i);
                    }
                }
            }
            var masterDetailXafGridView = Control as IColumnView;
            if (masterDetailXafGridView != null) masterDetailXafGridView.CanFilterGroupSummaryColumns = true;
        }
    }

    #region XAF GridLstEditor stuff
    public class GridLstEditorDynamicModelSynchronizer : ModelListSynchronizer {
        internal GridLstEditorDynamicModelSynchronizer(GridListEditor columnViewEditor, IModelListView viewDesignMode,
                                                       bool overrideViewDesignMode)
            : this(columnViewEditor.GridView, (IModelListViewOptionsGridView)viewDesignMode, overrideViewDesignMode) {
            
        }

        public GridLstEditorDynamicModelSynchronizer(XafGridView gridView, IModelListViewOptionsGridView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView) {
            ModelSynchronizerList.Add(new GridListEditorViewOptionsSynchronizer(gridView, modelListView.GridViewOptions, overrideViewDesignMode));
            ModelSynchronizerList.Add(new GridListEditorColumnOptionsSynchroniser(gridView, modelListView));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer(gridView, modelListView));
        }

        public GridLstEditorDynamicModelSynchronizer(GridListEditor columnViewEditor)
            : this(columnViewEditor, columnViewEditor.Model, false) {
        }

        public GridLstEditorDynamicModelSynchronizer(object control, IModelNode model) : base(control, model) {
        }
    }

    public class GridListEditorViewOptionsSynchronizer :
        ComponentSynchronizer<XafGridView, IModelOptionsGridView> {
        public GridListEditorViewOptionsSynchronizer(GridListEditor control, bool overrideViewDesignMode)
            : base(control.GridView, ((IModelListViewOptionsGridView)control.Model).GridViewOptions, overrideViewDesignMode) {
        }

        public GridListEditorViewOptionsSynchronizer(XafGridView control, IModelOptionsGridView modelNode, bool overrideViewDesignMode) : base(control, modelNode, overrideViewDesignMode) {
        }
    }
    public class GridListEditorColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<GridListEditor, IModelListViewOptionsGridView, IModelColumnOptionsGridView> {
        readonly XafGridView _gridView;

        public GridListEditorColumnOptionsSynchroniser(GridListEditor control)
            : base(control, (IModelListViewOptionsGridView)control.Model) {
        }

        public GridListEditorColumnOptionsSynchroniser(XafGridView gridView, IModelListViewOptionsGridView modelNode) : this(new GridListEditor(modelNode)) {
            _gridView= gridView;
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return _gridView ?? Control.GridView;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsGridView modelColumnOptionsView) {
            return modelColumnOptionsView.OptionsColumnGridView;
        }

    }

    #endregion

}
