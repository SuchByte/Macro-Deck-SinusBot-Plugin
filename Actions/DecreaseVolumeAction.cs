﻿using Newtonsoft.Json.Linq;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using SuchByte.SinusBotPlugin.GUI;
using SuchByte.SinusBotPlugin.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuchByte.SinusBotPlugin.Actions
{
    public class DecreaseVolumeAction : PluginAction
    {
        public override string Name => PluginLanguageManager.PluginStrings.ActionDecreaseVolume;
        public override string Description => PluginLanguageManager.PluginStrings.ActionDecreaseVolumeDescription;
        public override bool CanConfigure => true;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            if (Main.Sinusbot == null || Main.Sinusbot.LoggedIn == false) return;
            if (String.IsNullOrWhiteSpace(this.Configuration)) return;
            try
            {
                string instanceId = JObject.Parse(this.Configuration)["instanceId"].ToString();
                Main.Sinusbot.DecreaseVolume(instanceId);
            }
            catch { }
        }

        public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
        {
            return new SelectInstanceDialog(this);
        }
    }
}
