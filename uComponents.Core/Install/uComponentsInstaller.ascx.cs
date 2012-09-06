﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using uComponents.Core.Shared;
using umbraco;
using umbraco.IO;

namespace uComponents.Core.Install
{
	/// <summary>
	/// 
	/// </summary>
	public partial class uComponentsInstaller : UserControl
	{
		/// <summary>
		/// Gets the logo.
		/// </summary>
		/// <value>The logo.</value>
		protected string Logo
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(uComponentsInstaller), "uComponents.Core.Shared.Resources.Images.ucomponents-logo-small.png");
			}
		}

		/// <summary>
		/// Handles the PreInit event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			var types = Assembly.GetExecutingAssembly().GetTypes().ToList();
			types.Sort(delegate(Type t1, Type t2) { return t1.Name.CompareTo(t2.Name); });

			var notFoundHandlers = new Dictionary<string, string>();
			var xsltExtensions = new Dictionary<string, string>();

			foreach (var type in types)
			{
				string ns = type.Namespace;
				if (string.IsNullOrEmpty(ns))
				{
					continue;
				}

				if (ns == "uComponents.Core.NotFoundHandlers")
				{
					notFoundHandlers.Add(type.FullName.Replace("uComponents.Core.", string.Empty), type.Name);
					continue;
				}

				if (ns == "uComponents.Core.XsltExtensions" && type.IsPublic && !type.IsSerializable)
				{
					xsltExtensions.Add(type.FullName, type.Name);
					continue;
				}
			}

			// bind the UI Modules options.
			this.cblUiModules.DataSource = Settings.AppKeys_UiModules;
			this.cblUiModules.DataTextField = "Value";
			this.cblUiModules.DataValueField = "Key";
			this.cblUiModules.DataBind();

			// bind the data-types.
			this.cblNotFoundHandlers.DataSource = notFoundHandlers;
			this.cblNotFoundHandlers.DataTextField = "Value";
			this.cblNotFoundHandlers.DataValueField = "Key";
			this.cblNotFoundHandlers.DataBind();

			// bind the XSLT extensions.
			this.cblXsltExtensions.DataSource = xsltExtensions;
			this.cblXsltExtensions.DataTextField = "Value";
			this.cblXsltExtensions.DataValueField = "Key";
			this.cblXsltExtensions.DataBind();

			// disable the dashboard control checkbox
			try
			{
				var dashboardXml = xmlHelper.OpenAsXmlDocument(SystemFiles.DashboardConfig);
				if (dashboardXml.SelectSingleNode("//section[@alias = 'uComponentsInstaller']") != null)
				{
					this.phDashboardControl.Visible = false;
				}
			}
			catch { }
		}

		/// <summary>
		/// Handles the Click event of the btnActivate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnActivate_Click(object sender, EventArgs e)
		{
			var successes = new List<string>();
			var failures = new List<string>();

			var xml = new XmlDocument();

			// Razor Model Binding
			try
			{
				xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_AddAppConfigKey\" key=\"{0}\" value=\"{1}\" />", Constants.AppKey_RazorModelBinding, (!this.cbDisableRazorModelBinding.Checked).ToString().ToLower()));
				umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(this.cbDisableRazorModelBinding.Text, "uComponents_AddAppConfigKey", xml.FirstChild);
				successes.Add(this.cbDisableRazorModelBinding.Text);
			}
			catch (Exception ex)
			{
				failures.Add(string.Concat(this.cbDisableRazorModelBinding.Text, " (", ex.Message, ")"));
			}

			// Not Found Handlers
			foreach (ListItem item in this.cblNotFoundHandlers.Items)
			{
				if (item.Selected)
				{
					try
					{
						xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_Add404Handler\" assembly=\"uComponents.Core\" type=\"{0}\" />", item.Value));
						umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(item.Text, "uComponents_Add404Handler", xml.FirstChild);
						successes.Add(item.Text);
					}
					catch (Exception ex)
					{
						failures.Add(string.Concat(item.Text, " (", ex.Message, ")"));
					}
				}
			}

			// UI Modules
			if (this.cbUiModules.Checked)
			{
				try
				{
					xml.LoadXml("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_AddHttpModule\" />");
					umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(this.cbUiModules.Text, "uComponents_AddHttpModule", xml.FirstChild);
					successes.Add(this.cbUiModules.Text);
				}
				catch (Exception ex)
				{
					failures.Add(string.Concat(this.cbUiModules.Text, " (", ex.Message, ")"));
				}

				// adds the appSettings keys for the UI modules (drag-n-drop and tray-peek)
				foreach (ListItem item in this.cblUiModules.Items)
				{
					try
					{
						xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_AddAppConfigKey\" key=\"{0}\" value=\"{1}\" />", item.Value, item.Selected.ToString().ToLower()));
						umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(item.Text, "uComponents_AddAppConfigKey", xml.FirstChild);
						successes.Add(item.Text);
					}
					catch (Exception ex)
					{
						failures.Add(string.Concat(item.Text, " (", ex.Message, ")"));
					}
				}
			}

			// XSLT extensions
			foreach (ListItem item in this.cblXsltExtensions.Items)
			{
				if (item.Selected)
				{
					try
					{
						xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"addXsltExtension\" assembly=\"uComponents.Core\" type=\"{0}\" extensionAlias=\"ucomponents.{1}\" />", item.Value, item.Text.ToLower()));
						umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(item.Text, "addXsltExtension", xml.FirstChild);
						successes.Add(item.Text);
					}
					catch (Exception ex)
					{
						failures.Add(string.Concat(item.Text, " (", ex.Message, ")"));
					}
				}
			}

			if (this.cbDashboardControl.Checked)
			{
				var title = "Dashboard control";
				xml.LoadXml("<Action runat=\"install\" undo=\"true\" alias=\"addDashboardSection\" dashboardAlias=\"uComponentsInstaller\"><section><areas><area>developer</area></areas><tab caption=\"uComponents: Activator\"><control addPanel=\"true\">/umbraco/plugins/uComponents/uComponentsInstaller.ascx</control></tab></section></Action>");
				umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(title, "addDashboardSection", xml.FirstChild);
				successes.Add(title);
			}

			// set the feedback controls to hidden
			this.Failure.Visible = this.Success.Visible = false;

			// display failure messages
			if (failures.Count > 0)
			{
				this.Failure.type = umbraco.uicontrols.Feedback.feedbacktype.error;
				this.Failure.Text = "There were errors with the following components:<br />" + string.Join("<br />", failures.ToArray());
				this.Failure.Visible = true;
			}

			// display success messages
			if (successes.Count > 0)
			{
				this.Success.type = umbraco.uicontrols.Feedback.feedbacktype.success;
				this.Success.Text = "Successfully installed the following components: " + string.Join(", ", successes.ToArray());
				this.Success.Visible = true;
			}
		}
	}
}