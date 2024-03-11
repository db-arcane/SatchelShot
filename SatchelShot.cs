using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("SatchelShot", "db_arcane", "0.0.2")]
    [Description("Allows players to explode satchel charges with incendiary ammo")]
    class SatchelShot : RustPlugin
    {
        #region config
        private ConfigData configData;

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Flame Ammo")]
            public List<string> fireAmmo = new List<string>
            {
                "pistolbullet_fire (Projectile)",
                "riflebullet_fire (Projectile)",
                "arrow_fire (Projectile)",
                "shotgunbullet_fire (Projectile)"
            };
        }

        private bool LoadConfigVariables()
        {
            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected override void LoadDefaultConfig()
        {
            configData = new ConfigData();
            SaveConfig(configData);
        }

        private void SaveConfig(ConfigData config)
        {
            Config.WriteObject(config, true);
        }
#endregion

        void Init()
        {
            if (!LoadConfigVariables())
            {
                Puts("Config File issue detected. Please delete file, or check syntax and fix.");
                return;
            }
        }

        object OnPlayerAttack(BasePlayer attacker, HitInfo hitInfo)
        {
            // if no hit information, return
            if (hitInfo == null)
                return null;

            // if no target entity exists, return
            // this can occur when several bullets hit the same satchel, 
            //      which has already exploded.
            if (hitInfo.HitEntity == null)
                return null;

            // if target hit was not a satchel charge, return
            if (!hitInfo.HitEntity.PrefabName.Contains("satchelcharge"))
                return null;

            // cast the HitEntity as an TimedExplosive. If for some reason we can't, return
            TimedExplosive explosive = (TimedExplosive)hitInfo.HitEntity;
            if (explosive == null)
                return null;

            // if projectile is an allowed ammo type, explode satchel charge and return false
            // do not run default process, because target no longer exists after explosion     
            if (configData.fireAmmo.Contains(hitInfo.ProjectilePrefab.ToString()))
            {
                explosive.Explode();
                return false;
            }
            else
            {
                return null;
            }
        }
    }
}
