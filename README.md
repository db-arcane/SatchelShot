## Features

The SatchelShot plugin allows players to blow up a satchel charge using an incendiary bullet or fire arrow. Regardless of whether the charge is lit, doused or a dud. 

With this plugin, a player could place a satchel charge on a door, douse it with a water gun to put out the fuse, then set the charge off remotely with a gun. Imaginative players may come up with other uses. 

SatchelShot has options to relight the fuse instead of exploding immediately, allow explosive rounds to blow up a charge, and allow timed explosive charges (C4) to also be blown up. 

## Configuration

Default configuration:

```json
{
  "Explode On Hit": true,
  "Fuse Allows Duds": true,
  "Explode on Explosive Ammo": true,
  "Explode Timed Explosive":false,
  "Flame Ammo": [
    "pistolbullet_fire (Projectile)",
    "riflebullet_fire (Projectile)",
    "arrow_fire (Projectile)",
    "shotgunbullet_fire (Projectile)"
  ]
}
```
### Explode On Hit

- `Explode On Hit` -- If true, blow up the satchel charge. If false, relight the fuse instead. Default value is true (explode).

### Fuse Allows Duds

- `Fuse Allows Duds` -- If true, a relit fuse may be a dud (requiring the player to shoot the charge again). If false, a relit fuse always explodes. Default value is true (allow duds).

### Explode on Explosive Ammo

- `Explode on Explosive Ammo` -- If true, allow explosive rifle bullets to blow up a charge. Default value is false (ignore explosive bullets).

### Explode Timed Explosive

- `Explode Timed Explosive` -- If true, blow up the timed explosive when hit. If false, there is no effect. The same types of ammo that affect a satchel charge will also affect a timed explosive. Default value is false (no effect).

### Flame Ammo

- `Flame Ammo` -- These are the ammunition types that will set off a satchel charge. The name in the list must be the projectile's prefab name. 

**Note:** Only projectiles will work; hatchets and other melee weapons will not. 

## Localization

## FAQ

**Can this plugin allow other projectiles to set off a satchel charge?**
<p>	Yes, additional prefab names can be added to the Flame Ammo list. Here are a few that you may want to try:</p>

```json
	arrow_bone (Projectile)
	arrow_fire (Projectile)
	arrow_hv (Projectile)
	arrow_wooden (Projectile)
	handmade_shell.projectile (Projectile)
	pistolbullet (Projectile)
	pistolbullet_fire (Projectile)
	riflebullet {Projectile)
	riflebullet_explosive {Projectile)
	riflebullet_fire {Projectile)
	shotgunbullet (Projectile)
	shotgunbullet_fire (Projectile)
	shotgunslug (Projectile)
```
