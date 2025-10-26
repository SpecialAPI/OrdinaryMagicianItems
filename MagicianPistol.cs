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
            Gun gun = ETGMod.Databases.Items.NewGun("Magician Pistol", "nerfed_witch_pistol");
            Game.Items.Rename("outdated_gun_mods:magician_pistol", "spapi:magician_pistol");
            GunExt.SetShortDescription(gun, "Fancy Schmancy");
            GunExt.SetLongDescription(gun, "Well this is just a really messy attempt at a Witch Pistol.\nThe shoddy craftsmanship makes it particularly awkward to reload, but you'll sure as hell look cool doing it!\n\n" +
                "The \"Magic\" part of this weapon is purely for cosmetic purposes, it's really just a standard magnum.");
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.TransformToTargetGun(PickupObjectDatabase.GetById(145) as Gun);
            GunExt.SetupSprite(gun, null, "nerfed_witch_pistol_idle_001", 16);
            Projectile projectile = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(145) as Gun).DefaultModule.projectiles[0]);
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
            int index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    RemoveOffset(def);
                    MakeOffset(def, offsets[0][index]);
                }
                index++;
            }
            index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    RemoveOffset(def);
                    MakeOffset(def, offsets[1][index]);
                }
                index++;
            }
            index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    RemoveOffset(def);
                    MakeOffset(def, offsets[2][index]);
                }
                index++;
            }
            ChangeReloadSpeedSynergyProcessor processor = gun.gameObject.AddComponent<ChangeReloadSpeedSynergyProcessor>();
            processor.SynergyReloadTime = 1.2f;
            processor.RequiredSynergy = Plugin.TwelveShotsSynergy;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            AddDualWieldSynergyProcessor(gun.PickupObjectId, 145, Plugin.TwelveShotsSynergy);
        }

        public static void AddDualWieldSynergyProcessor(int id1, int id2, CustomSynergyType synergy)
        {
            DualWieldSynergyProcessor dualWieldController = PickupObjectDatabase.GetById(id1).gameObject.AddComponent<DualWieldSynergyProcessor>();
            dualWieldController.SynergyToCheck = synergy;
            dualWieldController.PartnerGunID = id2;
            DualWieldSynergyProcessor dualWieldController2 = PickupObjectDatabase.GetById(id2).gameObject.AddComponent<DualWieldSynergyProcessor>();
            dualWieldController2.SynergyToCheck = synergy;
            dualWieldController2.PartnerGunID = id1;
        }

        public static void RemoveOffset(tk2dSpriteDefinition def)
        {
            MakeOffset(def, -def.position0);
        }

        public static void MakeOffset(tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
        {
            float xOffset = offset.x;
            float yOffset = offset.y;
            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);
            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
            if (def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
            {
                def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
            }
        }

        public static void ConstructOffsetsFromAnchor(tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2? scale = null, bool fixesScale = false, bool changesCollider = true)
        {
            if (!scale.HasValue)
            {
                scale = new Vector2?(def.position3);
            }
            if (fixesScale)
            {
                Vector2 fixedScale = scale.Value - def.position0.XY();
                scale = new Vector2?(fixedScale);
            }
            float xOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
            {
                xOffset = -(scale.Value.x / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                xOffset = -scale.Value.x;
            }
            float yOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
            {
                yOffset = -(scale.Value.y / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                yOffset = -scale.Value.y;
            }
            MakeOffset(def, new Vector2(xOffset, yOffset), false);
            if (changesCollider && def.colliderVertices != null && def.colliderVertices.Length > 0)
            {
                float colliderXOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerLeft || anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.UpperLeft)
                {
                    colliderXOffset = (scale.Value.x / 2f);
                }
                else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    colliderXOffset = -(scale.Value.x / 2f);
                }
                float colliderYOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerLeft || anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.LowerRight)
                {
                    colliderYOffset = (scale.Value.y / 2f);
                }
                else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    colliderYOffset = -(scale.Value.y / 2f);
                }
                def.colliderVertices[0] += new Vector3(colliderXOffset, colliderYOffset, 0);
            }
        }

        public static List<List<Vector2>> offsets = new List<List<Vector2>>
        {
            new List<Vector2>
            {
                new Vector2(0f, 0f)
            },
            new List<Vector2>
            {
                new Vector2(0f, 0f),
                new Vector2(-0.0625f, 0.0625f),
                new Vector2(0f, -0.0625f),
                new Vector2(0f, 0f),
            },
            new List<Vector2>
            {
                new Vector2(0f, -0.0625f),
                new Vector2(0.125f, 0f),
                new Vector2(0.125f, 0.0625f),
                new Vector2(0.125f, 0.125f),
                new Vector2(0.125f, 0.0625f),
                new Vector2(0.125f, 0.125f),
                new Vector2(0f, 0.0625f),
                new Vector2(0f, -0.0625f)
            }
        };
    }
}
