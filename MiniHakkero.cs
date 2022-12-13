using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using ItemAPI;
using UnityEngine;

namespace OrdinaryMagicianItems
{
    public class MiniHakkero : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Mini-Hakkero", "mini_hakkero");
            Game.Items.Rename("outdated_gun_mods:minihakkero", "spapi:mini_hakkero");
            MiniHakkero controller = gun.gameObject.AddComponent<MiniHakkero>();
            GunExt.SetShortDescription(gun, "It's all about firepower!");
            GunExt.SetLongDescription(gun, "Made by a master smith and forged with the mythical hihi'irokane metal, this Magic Furnace takes the form of an octagonal block of wood with the eight Taoist trigrams inscribed on the front.\n\n" +
                "It allows its' wielder to channel and amplify their magic energy into a beam of destructive power.");
            GunExt.SetupSprite(gun, null, "mini_hakkero_idle2_001", 8);
            GunExt.SetAnimationFPS(gun, gun.idleAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(508) as Gun, true, false);
            Gun duelingLaser = PickupObjectDatabase.GetById(508) as Gun;
            gun.UsesRechargeLikeActiveItem = true;
            gun.ActiveItemStyleRechargeAmount = duelingLaser.ActiveItemStyleRechargeAmount;
            gun.reloadTime = 1.6f;
            controller.idle2Animation = GunExt.UpdateAnimation(gun, "idle2");
            controller.normalIdleAnimation = GunExt.UpdateAnimation(gun, "idle");
            gun.InfiniteAmmo = true;
            gun.gunSwitchGroup = "ChargeLaser";
            gun.barrelOffset.transform.localPosition += new Vector3(-0.55f, -0.160f, 0f);
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void Update()
        {
            base.Update();
            if (gun.RemainingActiveCooldownAmount <= 0f || gun.OwnerHasSynergy("Cheat against the Impossible"))
            {
                gun.idleAnimation = normalIdleAnimation;
            }
            else
            {
                gun.idleAnimation = idle2Animation;
            }
            if(lastIdleAnimation != gun.idleAnimation)
            {
                gun.PlayIdleAnimation();
            }
            if (gun.OwnerHasSynergy("Cheat against the Impossible"))
            {
                gun.RemainingActiveCooldownAmount = Mathf.Max(0f, gun.RemainingActiveCooldownAmount - 10f * BraveTime.DeltaTime);
            }
            lastIdleAnimation = gun.idleAnimation;
        }

        public void LateUpdate()
        {
            tk2dBaseSprite sprite = gun.GetSprite();
            float num = gun.CurrentAngle;
            if (gun.CurrentOwner is PlayerController)
            {
                PlayerController playerController = gun.CurrentOwner as PlayerController;
                num = BraveMathCollege.Atan2Degrees(playerController.unadjustedAimPoint.XY() - gun.transform.parent.position.XY());
            }
            int num2 = BraveMathCollege.AngleToOctant(num + 90f);
            if (num2 == 1 || num2 == 2 || num2 == 3)
            {
                sprite.HeightOffGround = 0.075f;
            }
            else
            {
                sprite.HeightOffGround = -0.075f;
            }
            sprite.UpdateZDepth();
        }

        public string lastIdleAnimation;
        public string normalIdleAnimation;
        public string idle2Animation;
    }
}
