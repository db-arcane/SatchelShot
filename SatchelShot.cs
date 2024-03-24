using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("SatchelShot", "db_arcane", "1.0.6")]
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
			// if hitInfo or HitEntity are null, return
			if (hitInfo?.HitEntity == null)
				return null;
			
            // if HitEntity is not a satchel charge, return
            if (!hitInfo.HitEntity.PrefabName.Contains("satchelcharge"))
                return null;

            // if not hit with a projectile, return
            if (!hitInfo.IsProjectile())
                return null;

            // cast the HitEntity as an DudTimedExplosive. 
            DudTimedExplosive explosive = (DudTimedExplosive)hitInfo.HitEntity;

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
