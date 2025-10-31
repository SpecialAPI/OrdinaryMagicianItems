using Alexandria.ItemAPI;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AnimationType = Alexandria.ItemAPI.CompanionBuilder.AnimationType;
using DirectionType = DirectionalAnimation.DirectionType;
using FlipType = DirectionalAnimation.FlipType;

namespace OrdinaryMagicianItems
{
    public class Shanghai : PassiveItem
    {
        public static void Init()
        {
            var itemName = "Shanghai";
            var resourceName = "OrdinaryMagicianItems/Resources/shanghai_ammonomicon_icon";

            var obj = new GameObject(itemName);
            var item = obj.AddComponent<Shanghai>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            var shortDesc = "The Precious Thing";
            var longDesc = "A little sentient doll. No, it's not yours.\nWhen in danger, it will start following you around and will attack enemies.\nAlice won't forgive you if you lose it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");

            item.quality = ItemQuality.SPECIAL;
            item.CompanionGuid = "shanghai";
            BuildPrefab();
        }

        public static void BuildPrefab()
        {
            if (prefab != null || CompanionBuilder.companionDictionary.ContainsKey("shanghai"))
                return;

            prefab = CompanionBuilder.BuildPrefab("Shanghai", "shanghai", "OrdinaryMagicianItems/Resources/companion/idle_right/shanghai_idle_front_right_001", new IntVector2(9, 3), new IntVector2(8, 12));
            var companion = prefab.AddComponent<CompanionController>();
            companion.aiActor.IsNormalEnemy = false;
            companion.CanInterceptBullets = false;
            companion.specRigidbody.PixelColliders.Add(new PixelCollider()
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.EnemyHitBox,
                ManualWidth = 8,
                ManualHeight = 12,
                ManualOffsetX = 9,
                ManualOffsetY = 3
            });
            companion.specRigidbody.CollideWithOthers = true;
            companion.aiActor.CollisionDamage = 0f;
            companion.aiActor.MovementSpeed = 7.2f;
            companion.aiActor.HitByEnemyBullets = false;
            companion.aiActor.CanDropCurrency = false;

            prefab.AddAnimation("idle_right", "OrdinaryMagicianItems/Resources/companion/idle_right", 6, AnimationType.Idle, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("idle_left", "OrdinaryMagicianItems/Resources/companion/idle_left", 6, AnimationType.Idle, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("run_right", "OrdinaryMagicianItems/Resources/companion/run_right", 6, AnimationType.Move, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("run_left", "OrdinaryMagicianItems/Resources/companion/run_left", 6, AnimationType.Move, DirectionType.TwoWayHorizontal);

            tk2dSpriteCollectionData collection = companion.GetComponent<tk2dSpriteCollectionData>();
            if (!collection)
                collection = SpriteBuilder.ConstructCollection(companion.gameObject, $"{companion.name}_collection");

            var attackLeftFrames = new List<string>()
            {
                "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_001",
                "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_002",
                "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_003",
                "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_004",
                "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_005",
                "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_006"
            };
            var attackLeftAnim = SpriteBuilder.AddAnimation(companion.spriteAnimator, collection, [..attackLeftFrames.Select(x => SpriteBuilder.AddSpriteToCollection(x, collection))], "attack_left", tk2dSpriteAnimationClip.WrapMode.Once);
            attackLeftAnim.fps = 6;

            var attackRightFrames = new List<string>()
            {
                "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_001",
                "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_002",
                "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_003",
                "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_004",
                "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_005",
                "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_006"
            };
            var attackRightAnim = SpriteBuilder.AddAnimation(companion.spriteAnimator, collection, [.. attackRightFrames.Select(x => SpriteBuilder.AddSpriteToCollection(x, collection))], "attack_right", tk2dSpriteAnimationClip.WrapMode.Once);
            attackRightAnim.fps = 6;

            companion.aiAnimator.AssignDirectionalAnimation("attack", new DirectionalAnimation()
            {
                AnimNames =
                [
                    "attack_right",
                    "attack_left"
                ],
                Flipped =
                [
                    FlipType.None,
                    FlipType.None
                ],
                Type = DirectionType.TwoWayHorizontal,
                Prefix = string.Empty
            }, AnimationType.Other);

            var bs = prefab.GetComponent<BehaviorSpeculator>();
            bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior()
            {
                IdleAnimations = ["idle"],
                DisableInCombat = true
            });
            bs.MovementBehaviors.Add(new SeekTargetBehavior()
            {
                StopWhenInRange = false,
                CustomRange = -1,
                LineOfSight = true,
                ReturnToSpawn = false,
                SpawnTetherDistance = 0f,
                PathInterval = 0.25f,
                ExternalCooldownSource = false,
                SpecifyRange = false,
                MinActiveRange = 0f,
                MaxActiveRange = 0f
            });
            bs.TargetBehaviors.Add(new TargetPlayerBehavior()
            {
                Radius = 35f,
                LineOfSight = true,
                ObjectPermanence = true,
                SearchInterval = 0.25f,
                PauseOnTargetSwitch = false,
                PauseTime = 0.25f
            });
            bs.AttackBehaviors.Add(new ShanghaiAttackBehavior()
            {
                minLeapDistance = 1f,
                leapDistance = 2f,
                maxTravelDistance = 5f,
                leadAmount = 1f,
                leapTime = 0.4f,
                maximumChargeTime = 0.25f,
                chargeAnim = string.Empty,
                leapAnim = "attack"
            });
        }

        private void CreateCompanion(PlayerController owner)
        {
            if (owner.healthHaver != null && owner.healthHaver.GetCurrentHealthPercentage() >= 1)
                return;

            var guid = CompanionGuid;
            var companionPrefab = EnemyDatabase.GetOrLoadByGuid(guid);
            var spawnPos = owner.transform.position;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                spawnPos += new Vector3(1.125f, -0.3125f, 0f);

            var companion = Instantiate(companionPrefab.gameObject, spawnPos, Quaternion.identity);
            m_extantCompanion = companion;

            var companionController = m_extantCompanion.GetOrAddComponent<CompanionController>();
            companionController.Initialize(owner);
            if (companionController.specRigidbody)
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(companionController.specRigidbody, null, false);
        }

        public void ForceCompanionRegeneration(PlayerController owner, Vector2? overridePosition)
        {
            var respawnPos =
                overridePosition ??
                (m_extantCompanion != null ? m_extantCompanion.transform.position.XY() : null);
            DestroyCompanion();
            CreateCompanion(owner);

            if (!m_extantCompanion || respawnPos is not Vector2 realRespawnPos)
                return;

            m_extantCompanion.transform.position = realRespawnPos.ToVector3ZisY(0f);
            var rb = m_extantCompanion.GetComponent<SpeculativeRigidbody>();
            if (rb)
                rb.Reinitialize();
        }

        private void DestroyCompanion()
        {
            if (!m_extantCompanion)
                return;

            Destroy(m_extantCompanion);
            m_extantCompanion = null;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += HandleNewFloor;

            var hh = player.healthHaver;
            if (hh != null)
            {
                hh.OnHealthChanged += OnHealthChanged;
                hh.OnPreDeath += MaybeRevive;
            }

            CreateCompanion(player);
        }

        public void MaybeRevive(Vector2 deathDir)
        {
            var reviveItems = new List<int>()
            {
                421,
                422,
                423,
                424,
                425
            };

            foreach (PassiveItem passive in m_owner.passiveItems)
            {
                if (!reviveItems.Contains(passive.PickupObjectId))
                    continue;

                m_owner.RemovePassiveItem(passive.PickupObjectId);
                m_owner.healthHaver.ApplyHealing(1f);
                m_owner.ForceBlank();
                m_owner.ClearDeadFlags();

                if (GameManager.Instance == null)
                    break;
                if (m_currentTimeSlow != null)
                    GameManager.Instance.StopCoroutine(m_currentTimeSlow);
                m_currentTimeSlow = GameManager.Instance.StartCoroutine(PostReviveTimeSlow());
                break;
            }
        }

        public IEnumerator PostReviveTimeSlow()
        {
            BraveTime.ClearMultiplier(gameObject);
            BraveTime.RegisterTimeScaleMultiplier(0.1f, gameObject);

            var elapsed = 0f;
            var duration = 2.5f;
            while (elapsed < duration)
            {
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                BraveTime.ClearMultiplier(gameObject);
                BraveTime.RegisterTimeScaleMultiplier(Mathf.Lerp(0.1f, 1f, elapsed - (duration - 1f)), gameObject);
                yield return null;
            }

            BraveTime.ClearMultiplier(gameObject);
            yield break;
        }

        public void OnHealthChanged(float resultValue, float maxValue)
        {
            if(resultValue >= maxValue)
                DestroyCompanion();
            else if(m_extantCompanion == null)
                CreateCompanion(Owner);
        }

        private void HandleNewFloor(PlayerController obj)
        {
            DestroyCompanion();
            CreateCompanion(obj);
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            DestroyCompanion();

            if (player == null)
                return;

            player.OnNewFloorLoaded -= HandleNewFloor;
            if (player.healthHaver != null)
            {
                player.healthHaver.OnHealthChanged -= OnHealthChanged;
                player.healthHaver.OnPreDeath -= MaybeRevive;
            }
        }

        public string CompanionGuid;
        private Coroutine m_currentTimeSlow;
        private GameObject m_extantCompanion;
        public static GameObject prefab;
    }
}
