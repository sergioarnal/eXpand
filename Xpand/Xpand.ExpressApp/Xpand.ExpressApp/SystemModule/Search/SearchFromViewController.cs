﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule.Search {
    public interface IModelMemberSearchMode {
        [Category(SearchFromViewController.AttributesCategory)]
        [Description("Control if member will be included on full text search")]
        SearchMemberMode SearchMemberMode { get; set; }
        
    }
    [ModelInterfaceImplementor(typeof(IModelMemberSearchMode), "ModelMember")]
    public interface IModelPropertyEditorSearchMode : IModelMemberSearchMode {
    }
    [ModelInterfaceImplementor(typeof(IModelMemberSearchMode), "ModelMember")]
    public interface IModelColumnSearchMode : IModelMemberSearchMode {

    }

    public enum FullTextSearchTargetPropertiesMode {
        AllSearchableMembers, VisibleColumns,IncludedColumns
    }
    public interface IModelClassFullTextSearch:IModelNode {
        [Category(SearchFromViewController.AttributesCategory)]
        FullTextSearchTargetPropertiesMode? FullTextSearchTargetPropertiesMode { get; set; }
        [Category(SearchFromViewController.AttributesCategory)]
        SearchMode? FullTextSearchMode { get; set; }
        [Category(SearchFromViewController.AttributesCategory)]
        [DataSourceProperty("ListViews")]
        IModelListView FullTextListView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> ListViews { get; }
    }
    [DomainLogic(typeof(IModelClassFullTextSearch))]
    public class ModelClassFullTextSearchDomainLogic {
        public static IModelList<IModelListView> Get_ListViews(IModelClassFullTextSearch modelClassFullTextSearch) {
            return new CalculatedModelNodeList<IModelListView>(modelClassFullTextSearch.Application.Views.OfType<IModelListView>());
        }
    }
    [ModelInterfaceImplementor(typeof(IModelClassFullTextSearch), "ModelClass")]
    public interface IModelListViewFullTextSearch:IModelClassFullTextSearch {
        
    }
    public class XpandSearchCriteriaBuilder : SearchCriteriaBuilder {
        readonly Dictionary<IModelColumn,IMemberInfo> _excludedColumns;
        readonly Dictionary<IModelColumn,IMemberInfo> includedColumns;

        public Dictionary<IModelColumn,IMemberInfo> ExcludedColumns {
            get { return _excludedColumns; }
        }

        public Dictionary<IModelColumn, IMemberInfo> IncludedColumns {
            get { return includedColumns; }
        }

        public XpandSearchCriteriaBuilder() {
        }

        public XpandSearchCriteriaBuilder(ITypeInfo typeInfo, View view) : base(typeInfo) {
            var listView = ((XpandListView)view);
            _excludedColumns = GetColumns(listView, SearchMemberMode.Exclude);
            includedColumns = GetColumns(listView, SearchMemberMode.Include);
        }

        Dictionary<IModelColumn,IMemberInfo> GetColumns(XpandListView listView, SearchMemberMode searchMemberMode) {
            return listView.Model.Columns.OfType<IModelColumnSearchMode>().Where(wrapper => wrapper.SearchMemberMode == searchMemberMode).OfType<IModelColumn>()
                        .Select(column => new{Column = column, Member = GetActualSearchProperty(column.PropertyName)}).ToDictionary(item => item.Column, item => item.Member);
        }

        public XpandSearchCriteriaBuilder(ITypeInfo typeInfo, ICollection<string> properties, string valueToSearch, GroupOperatorType valuesGroupOperatorType, bool includeNonPersistentMembers, SearchMode searchMode) : base(typeInfo, properties, valueToSearch, valuesGroupOperatorType, includeNonPersistentMembers, searchMode) {
        }

        public XpandSearchCriteriaBuilder(ITypeInfo typeInfo, ICollection<string> properties, string valueToSearch, GroupOperatorType valuesGroupOperatorType, bool includeNonPersistentMembers) : base(typeInfo, properties, valueToSearch, valuesGroupOperatorType, includeNonPersistentMembers) {
        }

        public XpandSearchCriteriaBuilder(ITypeInfo typeInfo, ICollection<string> properties, string valueToSearch, GroupOperatorType valuesGroupOperatorType) : base(typeInfo, properties, valueToSearch, valuesGroupOperatorType) {
        }

        protected override bool AllowSearchForMember(IMemberInfo memberInfo) {
            if (_excludedColumns != null && _excludedColumns.Select(pair => pair.Value).Contains(memberInfo)) return false;
            if (includedColumns != null && includedColumns.Select(pair => pair.Value) .Contains(memberInfo)) return true;
            return base.AllowSearchForMember(memberInfo);
        }
    }

    public class SearchFromViewController : ViewController, IModelExtender {
        public const string AttributesCategory = "eXpand.Search";
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberSearchMode>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorSearchMode>();
            extenders.Add<IModelColumn, IModelColumnSearchMode>();
            extenders.Add<IModelClass, IModelClassFullTextSearch>();
            extenders.Add<IModelListView, IModelListViewFullTextSearch>();
        }

        private string[] GetShownProperties(XpandSearchCriteriaBuilder criteriaBuilder) {
            var visibleProperties = new List<string>();
            var modelColumns = ((ListView)View).Model.Columns.GetVisibleColumns().Where(column => !criteriaBuilder.ExcludedColumns.Select(pair => pair.Key).Contains(column));
            foreach (IModelColumn column in modelColumns) {
                IMemberInfo memberInfo = null;
                if (column.ModelMember != null) {
                    memberInfo = new ObjectEditorHelperBase(column.ModelMember.MemberInfo.MemberTypeInfo, column).DisplayMember;
                }
                if (memberInfo != null) {
                    visibleProperties.Add(column.PropertyName + "." + memberInfo.Name);
                } else {
                    visibleProperties.Add(column.PropertyName);
                }
            }
            return visibleProperties.ToArray();
        }

        protected virtual ICollection<String> GetFullTextSearchProperties(XpandSearchCriteriaBuilder criteriaBuilder) {

            criteriaBuilder.IncludeNonPersistentMembers = false;
            FullTextSearchTargetPropertiesMode fullTextSearchTargetPropertiesMode = GetFullTextSearchTargetPropertiesMode();
            switch (fullTextSearchTargetPropertiesMode) {
                case FullTextSearchTargetPropertiesMode.AllSearchableMembers:
                    criteriaBuilder.FillSearchProperties();
                    criteriaBuilder.AddSearchProperties(GetShownProperties(criteriaBuilder));
                    break;
                case FullTextSearchTargetPropertiesMode.VisibleColumns:
                    var shownProperties = new List<string>(GetShownProperties(criteriaBuilder));
                    string friendlyKeyMemberName = FriendlyKeyPropertyAttribute.FindFriendlyKeyMemberName(View.ObjectTypeInfo, true);
                    if (!string.IsNullOrEmpty(friendlyKeyMemberName) && !shownProperties.Contains(friendlyKeyMemberName)) {
                        shownProperties.Add(friendlyKeyMemberName);
                    }
                    criteriaBuilder.SetSearchProperties(shownProperties);
                    break;
                case FullTextSearchTargetPropertiesMode.IncludedColumns: {
                    var properties = criteriaBuilder.IncludedColumns.Select(pair => pair.Value.Name).ToArray();
                    criteriaBuilder.SetSearchProperties(properties);
                    break;
                }
                default:
                    throw new ArgumentException(fullTextSearchTargetPropertiesMode.ToString(), "criteriaBuilder");
            }
            return criteriaBuilder.SearchProperties;
        }

        FullTextSearchTargetPropertiesMode GetFullTextSearchTargetPropertiesMode() {
            var fullTextSearchTargetPropertiesMode = (FullTextSearchTargetPropertiesMode) Frame.GetController<FilterController>().FullTextSearchTargetPropertiesMode;
            var textSearchTargetPropertiesMode = ((IModelListViewFullTextSearch) View.Model).FullTextSearchTargetPropertiesMode;
            if (textSearchTargetPropertiesMode.HasValue)
                fullTextSearchTargetPropertiesMode = textSearchTargetPropertiesMode.Value;
            return fullTextSearchTargetPropertiesMode;
        }
    }

}