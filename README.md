## Features

Allows players to blow up a satchel charge using an incendiary bullet or fire arrow. Regardless of whether the charge is lit, doused or a dud. 

With this plugin, a player could place a satchel charge on a door, douse it with a water gun to put out the fuse, then set the charge off remotely with a gun. Imaginative players may come up with other uses. 

SatchelShot also has options to relight the fuse instead of immediately detonating, and allow explosive rounds to blow up charges. 

## Configuration

Default configuration:

```json
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
```
### Explode On Hit

- `Explode On Hit` -- If true, blow up the satchel charge. If false, relight the fuse. Default value is true (explode).

### Fuse Allows Duds

- `Fuse Allows Duds` -- If true, a relit fuse may be a dud (requiring the player to shoot the charge again). If false, a relit fuse always explodes. Default value is true (allow duds).

### Explode on Explosive Ammo

- `Explode on Explosive Ammo` -- If true, allow explosive rifle bullets to blow up a charge. Default value is false (ignore explosive bullets).

### Flame Ammo

- `Flame Ammo` -- These are the ammunition types that will set off a satchel charge. The name in the list must be the projectile's prefab name. 

**Note:** Only projectiles will work; hatchets and other melee weapons will not. 

## Localization

## FAQ




