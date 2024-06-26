﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;
using System.Globalization;

namespace OrdinaryMagicianItems
{
    public class Broom : PlayerItem
    {
		public static void Init()
		{
			string itemName = "Ordinary Broom";
			string resourceName = "OrdinaryMagicianItems/Resources/broom";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<Broom>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "Aesthetics Matter";
			string longDesc = "Grants flight until you hop off.\nAllows to steal in plain sight and run for your life.\n\nThis is a completely normal broom with no special properties whatsoever.\nSweeping dust is a broom's most common use, but this one " +
				"in particular is used by a certain Ordinary Magician in order to fly.\nAccording to her, though, she doesn't actually need the broom to fly, and simply uses it during flight for aesthetic purposes.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
			ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1);
			item.consumable = false;
			item.quality = ItemQuality.SPECIAL;
			Hook stealAttemptHook = new Hook(
				typeof(BaseShopController).GetMethod("AttemptToSteal"),
				typeof(Broom).GetMethod("StealAttemptHook")
			);
			Hook shopItemInteractHook = new Hook(
				typeof(ShopItemController).GetMethod("Interact"),
				typeof(Broom).GetMethod("ShopItemInteractHook")
			);
			GameObject broomObj = new GameObject("BroomAttachment");
			ItemBuilder.AddSpriteToObject(broomObj.name, "OrdinaryMagicianItems/Resources/broom_obj/broom_front", broomObj);
			DontDestroyOnLoad(broomObj);
			tk2dBaseSprite sprite = broomObj.GetComponent<tk2dBaseSprite>();
			tk2dSpriteAnimator animator = broomObj.AddComponent<tk2dSpriteAnimator>();
			animator.Library = broomObj.AddComponent<tk2dSpriteAnimation>();
			List<int> ids = new List<int>
			{
				sprite.spriteId,
				SpriteBuilder.AddSpriteToCollection("OrdinaryMagicianItems/Resources/broom_obj/broom_back", sprite.Collection),
				SpriteBuilder.AddSpriteToCollection("OrdinaryMagicianItems/Resources/broom_obj/broom_front_left", sprite.Collection),
				SpriteBuilder.AddSpriteToCollection("OrdinaryMagicianItems/Resources/broom_obj/broom_front_right", sprite.Collection),
				SpriteBuilder.AddSpriteToCollection("OrdinaryMagicianItems/Resources/broom_obj/broom_back_right", sprite.Collection),
				SpriteBuilder.AddSpriteToCollection("OrdinaryMagicianItems/Resources/broom_obj/broom_back_left", sprite.Collection)
			};
			List<tk2dSpriteAnimationClip> clips = new List<tk2dSpriteAnimationClip>();
			foreach(int id in ids)
            {
				tk2dSpriteDefinition def = sprite.Collection.spriteDefinitions[id];
				Vector2 defOffset = def.name == "broom_back_left" ? new Vector2(0f, -0.0625f) : Vector2.zero;
				MagicianPistol.ConstructOffsetsFromAnchor(def, tk2dBaseSprite.Anchor.MiddleCenter, null, false, false);
				MagicianPistol.MakeOffset(def, defOffset, false);
				int id2 = SpriteBuilder.AddSpriteToCollection("OrdinaryMagicianItems/Resources/broom_obj/" + def.name + "2", sprite.Collection);
				tk2dSpriteDefinition def2 = sprite.Collection.spriteDefinitions[id2];
				MagicianPistol.ConstructOffsetsFromAnchor(def2, tk2dBaseSprite.Anchor.MiddleCenter, null, false, false);
				MagicianPistol.MakeOffset(def2, defOffset + new Vector2(0f, -0.0625f), false);
				tk2dSpriteAnimationFrame firstFrame = new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = sprite.Collection };
				tk2dSpriteAnimationFrame secondFrame = new tk2dSpriteAnimationFrame { spriteId = id2, spriteCollection = sprite.Collection };
				tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip()
				{
					fps = 6,
					frames = new tk2dSpriteAnimationFrame[] { firstFrame, secondFrame },
					loopStart = 0,
					maxFidgetDuration = 0f,
					minFidgetDuration = 0f,
					name = def.name,
					wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
				};
				clips.Add(clip);
			}
			animator.Library.clips = clips.ToArray();
			item.prefabToAttachToPlayer = broomObj;
		}

		public void InitializeCallbacks(PlayerController player)
		{
			player.OnEnteredCombat += OnCombatEntered;
			player.OnUsedBlank += Shoot;
			m_callbacksInitialized = true;
		}

		public void DeinitializeCallbacks(PlayerController player)
		{
			player.OnEnteredCombat -= OnCombatEntered;
			player.OnUsedBlank -= Shoot;
			m_callbacksInitialized = false;
		}

		public void Shoot(PlayerController player, int i)
        {
			if (player.PlayerHasActiveSynergy("Incident-Resolver kit")) 
			{
				GameObject go = SpawnManager.SpawnProjectile(PickupObjectDatabase.GetById(508).GetComponent<Gun>().DefaultModule.projectiles[0].gameObject, player.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, player.FacingDirection));
				Projectile proj = go.GetComponent<Projectile>();
				if (proj != null)
				{
					proj.Owner = player;
					proj.Shooter = player.specRigidbody;
				} 
			}
        }

		public void OnCombatEntered()
        {
			if(PickedUp && LastOwner != null && LastOwner.CurrentRoom != null && LastOwner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All) != null && 
				LastOwner.PlayerHasActiveSynergy("Incident-Resolver kit"))
            {
				foreach (AIActor aiactor in LastOwner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.RoomClear))
				{
					if (aiactor != null && aiactor.healthHaver != null && aiactor.healthHaver.IsBoss)
					{
						LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(GlobalItemIds.Blank).gameObject, LastOwner);
						LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(GlobalItemIds.SmallHeart).gameObject, LastOwner);
						foreach (PlayerItem active in LastOwner.activeItems)
						{
							active.DidDamage(LastOwner, 100);
						}
						return;
					}
				}
            }
        }

        public static void ShopItemInteractHook(Action<ShopItemController, PlayerController> orig, ShopItemController self, PlayerController player)
        {
			CurrentBuyingPlayer = player;
			orig(self, player);
			CurrentBuyingPlayer = null;
        }

		public static bool StealAttemptHook(Func<BaseShopController, bool> orig, BaseShopController self)
        {
			if(CurrentBuyingPlayer != null && PassiveItem.IsFlagSetForCharacter(CurrentBuyingPlayer, typeof(Broom)))
            {
				self.NotifyStealFailed();
				return true;
            }
			return orig(self);
        }

		public override void DoEffect(PlayerController user)
		{
			if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
			{
				return;
			}
			IsCurrentlyActive = true;
			user.SetIsFlying(true, "broom", true, false);
			user.AdditionalCanDodgeRollWhileFlying.SetOverride("broom", true, null);
			user.SetCapableOfStealing(true, "broom", null);
			PassiveItem.IncrementFlag(user, typeof(Broom));
			instanceAttachment = user.RegisterAttachedObject(prefabToAttachToPlayer, "", 0f);
			instanceAttachmentSprite = instanceAttachment.GetComponent<tk2dSprite>();
		}

		public void LateUpdate()
		{
			if (instanceAttachment && PickedUp && LastOwner)
			{
				instanceAttachment.transform.position = LastOwner.sprite.WorldCenter + new Vector2(0f, -0.5f);
				instanceAttachmentSprite.UpdateZDepth();
			}
		}

		protected void Deactivate(PlayerController user)
		{
			IsCurrentlyActive = false;
			user.SetIsFlying(false, "broom", true, false);
			user.AdditionalCanDodgeRollWhileFlying.RemoveOverride("broom");
			user.SetCapableOfStealing(false, "broom", null);
			PassiveItem.DecrementFlag(user, typeof(Broom));
			user.DeregisterAttachedObject(instanceAttachment, true);
			instanceAttachmentSprite = null;
			user.stats.RecalculateStats(user, false, false);
		}

		public override void Update()
		{
			base.Update();
            if (PickedUp && !m_callbacksInitialized)
            {
				InitializeCallbacks(LastOwner);
            }
			if (IsCurrentlyActive)
			{
				string suffix = LastOwner.GetBaseAnimationSuffix(false);
				if (suffix.EndsWith("_right") && LastOwner.sprite.FlipX)
				{
					suffix = suffix.Substring(0, suffix.LastIndexOf('_')) + "_left";
				}
				if (instanceAttachment != null && instanceAttachmentSprite.spriteAnimator != null && !instanceAttachmentSprite.spriteAnimator.IsPlaying("broom" + suffix))
				{
					instanceAttachmentSprite.spriteAnimator.Play("broom" + suffix);
				}
				if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || (LastOwner != null && LastOwner.IsDodgeRolling))
				{
					Deactivate(LastOwner);
				}
			}
		}

		public override void OnPreDrop(PlayerController user)
		{
			if (IsCurrentlyActive)
			{
				Deactivate(user);
			}
            if (m_callbacksInitialized)
            {
				DeinitializeCallbacks(user);
            }
		}

		public override void OnItemSwitched(PlayerController user)
		{
			if (IsCurrentlyActive)
			{
				Deactivate(user);
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if(LastOwner != null && m_callbacksInitialized)
			{
				DeinitializeCallbacks(LastOwner);
			}
		}

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && !user.IsDodgeRolling;
        }

        public GameObject prefabToAttachToPlayer;
		private GameObject instanceAttachment;
		private tk2dBaseSprite instanceAttachmentSprite;
		private bool m_callbacksInitialized;
		public static PlayerController CurrentBuyingPlayer;
	}
}
