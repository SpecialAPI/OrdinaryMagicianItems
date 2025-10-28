using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using Alexandria.ItemAPI;
using UnityEngine;

namespace OrdinaryMagicianItems
{
    public class MagicianPistol
    {
        public static void Init()
        {
            var gun = ETGMod.Databases.Items.NewGun("Magician Pistol", "nerfed_witch_pistol");
            Game.Items.Rename("outdated_gun_mods:magician_pistol", "spapi:magician_pistol");
            gun.SetShortDescription("Fancy Schmancy");
            gun.SetLongDescription("Well this is just a really messy attempt at a Witch Pistol.\nThe shoddy craftsmanship makes it particularly awkward to reload, but you'll sure as hell look cool doing it!\n\n" +
                "The \"Magic\" part of this weapon is purely for cosmetic purposes, it's really just a standard magnum.");
            gun.AddProjectileModuleFrom("klobb", true, false);
            gun.TransformToTargetGun(PickupObjectDatabase.GetById(145) as Gun);
            gun.SetupSprite(null, "nerfed_witch_pistol_idle_001", 16);

            var projectile = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(145) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.CanTransmogrify = false;
            projectile.ChanceToTransmogrify = -1f;

            gun.reloadTime = 1.6f;
            gun.StarterGunForAchievement = true;
            gun.DefaultModule.projectiles[0] = projectile;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.gunClass = GunClass.PISTOL;

            var animator = gun.GetComponent<tk2dSpriteAnimator>();
            var offsets = new Dictionary<string, List<Vector2>>
            {
                [gun.idleAnimation] = new()
                {
                    new(0f, 0f)
                },
                [gun.shootAnimation] = new()
                {
                    new(0f, 0f),
                    new(-0.0625f, 0.0625f),
                    new(0f, -0.0625f),
                    new(0f, 0f),
                },
                [gun.reloadAnimation] = new()
                {
                    new(0f, -0.0625f),
                    new(0.125f, 0f),
                    new(0.125f, 0.0625f),
                    new(0.125f, 0.125f),
                    new(0.125f, 0.0625f),
                    new(0.125f, 0.125f),
                    new(0f, 0.0625f),
                    new(0f, -0.0625f)
                }
            };
            foreach(var kvp in offsets)
            {
                var frames = animator.GetClipByName(kvp.Key).frames;
                var offsetsForFrames = kvp.Value;

                for(var i = 0; i < frames.Length; i++)
                {
                    var frame = frames[i];
                    var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                    def.MakeOffset(offsetsForFrames[i]);
                }
            }

            var processor = gun.gameObject.AddComponent<ChangeReloadSpeedSynergyProcessor>();
            processor.SynergyReloadTime = 1.2f;
            processor.RequiredSynergy = Plugin.TwelveShotsSynergy;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            AddDualWieldSynergyProcessor(gun.PickupObjectId, 145, Plugin.TwelveShotsSynergy);
        }

        public static void AddDualWieldSynergyProcessor(int id1, int id2, CustomSynergyType synergy)
        {
            var dualWieldController1 = PickupObjectDatabase.GetById(id1).gameObject.AddComponent<DualWieldSynergyProcessor>();
            dualWieldController1.SynergyToCheck = synergy;
            dualWieldController1.PartnerGunID = id2;

            var dualWieldController2 = PickupObjectDatabase.GetById(id2).gameObject.AddComponent<DualWieldSynergyProcessor>();
            dualWieldController2.SynergyToCheck = synergy;
            dualWieldController2.PartnerGunID = id1;
        }
    }
}
