﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("SatchelShot", "db_arcane", "1.0.3")]
    [Description("Allows players to explode satchel charges with incendiary ammo")]
    class SatchelShot : RustPlugin
    {
        #region config
        private ConfigData configData;

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Explode On Hit")]
            public bool explodeOnHit = true;

            [JsonProperty(PropertyName = "Fuse Allows Duds")]
            public bool allowDuds = true;

            [JsonProperty(PropertyName = "Explode on Explosive Ammo")]
            public bool allowExplosiveAmmo = false;

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

            // cast the HitEntity as an DudTimedExplosive. If for some reason we can't, return
            DudTimedExplosive explosive = (DudTimedExplosive)hitInfo.HitEntity;
            if (explosive == null)
                return null;

			// dummy variables for method calls
			ItemDefinition splashType = new ItemDefinition();
			Vector3 fromPos = new Vector3(0, 0, 0);

            // if projectile is an allowed ammo type, explode satchel charge or light its fuse
            if (configData.fireAmmo.Contains(hitInfo.ProjectilePrefab.ToString()))
            {
                // if explodeOnHit is true, explode the satchel charge
                if (configData.explodeOnHit)
                {
                    // call DoSplash() to cancel previous Explode() timer
                    explosive.DoSplash(splashType, 0);
                    explosive.dudChance = 0;
                    explosive.Explode();
                }
                else // light the fuse
                {
                    explosive.DoSplash(splashType, 0);

                    // if allowDuds is false, set dudChance to zero
                    if (!configData.allowDuds)
                        explosive.dudChance = 0;

                    explosive.Ignite(fromPos);
                    explosive.WaterCheck();
                }
            }
            else
            {
                // if projectile is an explosive .556 round and allowExplosiveAmmo is true, explode the satchel charge
				if (configData.allowExplosiveAmmo && (hitInfo.ProjectilePrefab.ToString().Contains("riflebullet_explosive (Projectile)")))
				{
                    explosive.DoSplash(splashType, 0);
                    explosive.dudChance = 0;
                    explosive.Explode();
				}	
            }
            return null;
        }
    }
}
