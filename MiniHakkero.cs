using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using Alexandria.ItemAPI;
using UnityEngine;

namespace OrdinaryMagicianItems
{
    public class MiniHakkero : GunBehaviour
    {
        public string lastIdleAnimation;
        public string normalIdleAnimation;
        public string idle2Animation;

        public static void Init()
        {
            var gun = ETGMod.Databases.Items.NewGun("Mini-Hakkero", "mini_hakkero");
            Game.Items.Rename("outdated_gun_mods:minihakkero", "spapi:mini_hakkero");
            gun.SetShortDescription("It's all about firepower!");
            gun.SetLongDescription("Made by a master smith and forged with the mythical hihi'irokane metal, this Magic Furnace takes the form of an octagonal block of wood with the eight Taoist trigrams inscribed on the front.\n\n" +
                "It allows its' wielder to channel and amplify their magic energy into a beam of destructive power.");
            gun.SetupSprite(null, "mini_hakkero_idle2_001", 8);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(508) as Gun, true, false);

            var duelingLaser = PickupObjectDatabase.GetById(508) as Gun;
            gun.UsesRechargeLikeActiveItem = true;
            gun.ActiveItemStyleRechargeAmount = duelingLaser.ActiveItemStyleRechargeAmount;
            gun.reloadTime = 1.6f;
            gun.InfiniteAmmo = true;
            gun.gunSwitchGroup = "ChargeLaser";
            gun.barrelOffset.transform.localPosition += new Vector3(-0.55f, -0.160f, 0f);
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.gunClass = GunClass.PISTOL;

            var controller = gun.gameObject.AddComponent<MiniHakkero>();
            controller.idle2Animation = GunExt.UpdateAnimation(gun, "idle2");
            controller.normalIdleAnimation = GunExt.UpdateAnimation(gun, "idle");

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void Update()
        {
            if (GenericOwner == null)
                return;

            var hasSynergy = PlayerOwner && PlayerOwner.HasActiveBonusSynergy(Plugin.CheatAgainstTheImpossibleSynergy);
            if (hasSynergy)
                gun.RemainingActiveCooldownAmount = Mathf.Max(0f, gun.RemainingActiveCooldownAmount - 10f * BraveTime.DeltaTime);

            if (gun.RemainingActiveCooldownAmount <= 0f || hasSynergy)
                gun.idleAnimation = normalIdleAnimation;
            else
                gun.idleAnimation = idle2Animation;

            if(lastIdleAnimation != gun.idleAnimation)
                gun.PlayIdleAnimation();
            lastIdleAnimation = gun.idleAnimation;
        }

        public void LateUpdate()
        {
            if (GenericOwner == null)
                return;

            var sprite = gun.GetSprite();
            var angle = GenericOwner is PlayerController player ?
                (player.unadjustedAimPoint.XY() - gun.transform.parent.position.XY()).ToAngle() :
                gun.CurrentAngle;

            var octant = BraveMathCollege.AngleToOctant(angle + 90f);
            if (octant == 1 || octant == 2 || octant == 3)
                sprite.HeightOffGround = 0.075f;
            else
                sprite.HeightOffGround = -0.075f;

            sprite.UpdateZDepth();
        }
    }
}
