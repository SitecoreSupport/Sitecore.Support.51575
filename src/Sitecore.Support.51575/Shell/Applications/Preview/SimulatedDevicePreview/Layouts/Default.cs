using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.DeviceSimulation;
using Sitecore.Text;
using Sitecore.Web;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Sitecore.Support.Shell.Applications.Preview.SimulatedDevicePreview.Layouts
{
    public class Default : Page
    {
        protected HtmlGenericControl content;
        protected HtmlGenericControl device;
        protected HtmlForm form1;
        protected HtmlGenericControl RotateButton;
        protected Label RotateText;
        protected HiddenField Scale;
        protected HiddenField ScaleValues;
        protected HtmlIframe screen;
        protected HtmlGenericControl screenContainer;
        protected HiddenField SimulatorId;

        protected override NameValueCollection DeterminePostBackMode()
        {
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            Database database = Sitecore.Context.Database;
            if (database != null)
            {
                Simulator currentSimulator = DeviceSimulationUtil.GetCurrentSimulator(database);
                if (!(currentSimulator is NoneSimulator))
                {
                    string str = currentSimulator.Item.ID.ToShortID().ToString();
                    this.SimulatorId.Value = str;
                    UrlString url = new UrlString(base.Request.RawUrl)
                    {
                        ["sc_device_simulation"] = "1"
                    };
                    this.SetDeviceStyle(currentSimulator.Appearance);
                    this.SetScreenStyle(url, currentSimulator.Appearance);
                    RotatableSimulator rotatableSimulator = currentSimulator as RotatableSimulator;
                    if (rotatableSimulator != null)
                    {
                        this.SetOnRotationStyles(rotatableSimulator);
                    }
                    this.SetScale(currentSimulator.Appearance);
                    this.SetScaleValues(database);
                }
            }
        }

        private void SetDeviceOnRotationStyle(SimulatorAppearance appearance)
        {
            Assert.ArgumentNotNull(appearance, "appearance");
            this.device.Attributes["data-onrotation-height"] = appearance.BackgroundImageSize.Height + "px";
            this.device.Attributes["data-onrotation-width"] = appearance.BackgroundImageSize.Width + "px";
            this.device.Attributes["data-onrotation-background"] = $"url('{appearance.BackgroundImage}')";
        }

        private void SetDeviceStyle(SimulatorAppearance appearance)
        {
            Assert.ArgumentNotNull(appearance, "appearance");
            this.device.Style["height"] = appearance.BackgroundImageSize.Height + "px";
            this.device.Style["width"] = appearance.BackgroundImageSize.Width + "px";
            this.device.Style["left"] = "-" + Math.Round((double)(((double)appearance.BackgroundImageSize.Width) / 2.0)) + "px";
            this.device.Style["background-image"] = $"url('{appearance.BackgroundImage}')";
        }

        private void SetOnRotationStyles(RotatableSimulator rotatableSimulator)
        {
            Assert.ArgumentNotNull(rotatableSimulator, "rotatableSimulator");
            this.RotateButton.Visible = true;
            if (rotatableSimulator.IsRotated)
            {
                this.RotateButton.Attributes["class"] = "rotated";
            }
            using (new LanguageSwitcher(WebUtil.GetCookieValue("shell", "lang", Sitecore.Context.Language.Name)))
            {
                this.RotateText.Text = Translate.Text("Rotate");
            }
            this.SetDeviceOnRotationStyle(rotatableSimulator.OnRotationAppearance);
            this.SetScreenOnRoatationStyle(rotatableSimulator.OnRotationAppearance);
        }

        private void SetScale(SimulatorAppearance appearance)
        {
            Assert.ArgumentNotNull(appearance, "appearance");
            ushort? scale = appearance.Scale;
            ushort? nullable2 = scale;
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (!nullable4.HasValue)
            {
                this.Scale.Value = "100";
            }
            else
            {
                double num = Math.Round((double)(((double)scale.Value) / 100.0), 2);
                this.Scale.Value = scale.Value.ToString();
                string str = "scale(" + num.ToString(CultureInfo.InvariantCulture) + ")";
                string[] strArray = new string[] { "-moz-transform", "-webkit-transform", "-ms-transform", "transform" };
                foreach (string str2 in strArray)
                {
                    this.content.Style[str2] = str;
                }
            }
        }

        private void SetScaleValues(Database db)
        {
            Assert.ArgumentNotNull(db, "db");
            Item item = db.GetItem("{CCC9FA4C-C03E-4F68-B576-B423BEF01CB8}");
            if (item != null)
            {
                ListString str = new ListString();
                foreach (Item item2 in item.Children)
                {
                    uint num;
                    if (uint.TryParse(item2.Name, out num))
                    {
                        str.Add(num.ToString());
                    }
                }
                this.ScaleValues.Value = str.ToString();
            }
        }

        private void SetScreenOnRoatationStyle(SimulatorAppearance appearance)
        {
            Assert.ArgumentNotNull(appearance, "appearance");
            this.screenContainer.Attributes["data-onrotation-height"] = appearance.ScreenSize.Height + "px";
            this.screenContainer.Attributes["data-onrotation-width"] = appearance.ScreenSize.Width + "px";
            this.screenContainer.Attributes["data-onrotation-top"] = appearance.ScreenOffset.Y + "px";
            this.screenContainer.Attributes["data-onrotation-left"] = appearance.ScreenOffset.X + "px";
        }

        private void SetScreenStyle(UrlString url, SimulatorAppearance appearance)
        {
            Assert.ArgumentNotNull(url, "url");
            Assert.ArgumentNotNull(appearance, "appearance");
            this.screen.Attributes["src"] = url.ToString();
            this.screenContainer.Style["height"] = appearance.ScreenSize.Height + "px";
            this.screenContainer.Style["width"] = appearance.ScreenSize.Width + "px";
            this.screenContainer.Style["top"] = appearance.ScreenOffset.Y + "px";
            this.screenContainer.Style["left"] = appearance.ScreenOffset.X + "px";
        }
    }
}