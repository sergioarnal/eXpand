﻿using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp {
    public class ViewFactory {

        static void CheckDetailViewId(String detailViewId, Type objectType) {
            if (String.IsNullOrEmpty(detailViewId)) {
                throw new Exception(
                    SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToFindADetailViewForType,
                                                                 objectType.FullName));
            }
        }

        public static ListView CreateListView(XafApplication xafApplication, string viewId, CollectionSourceBase collectionSource,
                                              bool isRoot) {
            IModelView modelView = xafApplication.FindModelView(viewId);
            if (modelView == null) {
                throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.NodeWasNotFound,viewId));
            }
            var modelListView = ((IModelListView)modelView);
            if (modelListView == null)
            {
                throw new ArgumentException(string.Format(
                    "A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
                    DevExpress.ExpressApp.DetailView.InfoNodeName, DevExpress.ExpressApp.ListView.InfoNodeName, modelListView.Id));
            }
            var result = new ListView(collectionSource, xafApplication, isRoot);
            result.SetInfo(modelListView);
            return result;
        }

        public static DetailView CreateDetailView(XafApplication xafApplication, string viewId, object obj,
                                                  ObjectSpace objectSpace, bool isRoot) {
            if (obj != null) {
                CheckDetailViewId(viewId, obj.GetType());
            }
            IModelView modelView = xafApplication.FindModelView(viewId);
            if (!(modelView is IModelDetailView)) {
                throw new ArgumentException(string.Format(
                    "A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
                    null, DevExpress.ExpressApp.DetailView.InfoNodeName, viewId));
            }
            return new DetailView((IModelDetailView) modelView, objectSpace, obj, xafApplication, isRoot);
        }
    }
}