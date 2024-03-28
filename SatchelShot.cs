using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("SatchelShot", "db_arcane", "1.0.7")]
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

            [JsonProperty(PropertyName = "Explode Timed Explosive")]
            public bool explodeTimedExplosive = false;

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

            // if not hit with a projectile, return
            if (!hitInfo.IsProjectile())
                return null;

            // dummy variables for method calls
            ItemDefinition splashType = new ItemDefinition();
            Vector3 fromPos = new Vector3(0, 0, 0);

            // if HitEntity is a satchel charge 
            if (hitInfo.HitEntity.PrefabName.Contains("satchelcharge"))
            {
                // cast the HitEntity as an DudTimedExplosive. 
                DudTimedExplosive explosive = (DudTimedExplosive)hitInfo.HitEntity;

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
                        // if allowDuds is false, set dudChance to zero
                        if (!configData.allowDuds)
                            explosive.dudChance = 0;

                        explosive.DoSplash(splashType, 0);
                        explosive.SetFuse(explosive.GetRandomTimerTime());
                    }
                }
                else
                {
                    // if projectile is an explosive .556 round and allowExplosiveAmmo is true, explode the satchel charge
                    if (configData.allowExplosiveAmmo && (hitInfo.ProjectilePrefab.ToString().Contains("riflebullet_explosive (Projectile)")))
                    {
                        explosive.dudChance = 0;
                        explosive.DoSplash(splashType, 0);
                        explosive.Explode();
                    }
                }
                return null;
            }

            // if HitEntity is a c4 and explodeTimedExplosive is true
            if (hitInfo.HitEntity.PrefabName.Contains("explosive.timed") && configData.explodeTimedExplosive)
            {
                // cast HitEntity as RFTimedExplosive
                RFTimedExplosive explosive = (RFTimedExplosive)hitInfo.HitEntity;

                // if projectile is an allowed ammo type, or it's an explosive .556 round and allowExplosiveAmmo is true
                if (configData.fireAmmo.Contains(hitInfo.ProjectilePrefab.ToString()) ||
                        configData.allowExplosiveAmmo && (hitInfo.ProjectilePrefab.ToString().Contains("riflebullet_explosive (Projectile)")))
                {
                    // if RFFrequency is set, set it to 0 and stop any scheduled explosion
                    if (explosive.GetFrequency() > 0)
                    {
                        if (explosive.IsInvoking(new Action(((TimedExplosive)explosive).Explode)))
                            explosive.CancelInvoke(new Action(((TimedExplosive)explosive).Explode));

                        explosive.SetFrequency(0);
                    }
                    explosive.SetFuse(0f);
                }
                return null;
            }

            // otherwise, 
            return null;
        }
    }
}
