using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.Model;
using Xpand.ExpressApp.Web.PropertyEditors;
using EditorAliases = Xpand.ExpressApp.Editors.EditorAliases;

namespace Xpand.ExpressApp.Web.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WebApplication), "Resources.Toolbox_Module_System_Web.ico")]
    public sealed class XpandSystemAspNetModule : XpandModuleBase {
        public XpandSystemAspNetModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null) {
                Application.LoggedOn += ApplicationOnLoggedOn;
            }
        }

        void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            ((ShowViewStrategy)Application.ShowViewStrategy).CollectionsEditMode = ((IModelOptionsCollectionEditMode)Application.Model.Options).CollectionsEditMode;
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>();
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TimePropertyEditor, typeof(DateTime), typeof(ASPxTimePropertyEditor), false)));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsCollectionEditMode>();
            extenders.Add<IModelListView, IModelListViewOpenViewWhenNested>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRelativeDate>();
        }
    }
}
