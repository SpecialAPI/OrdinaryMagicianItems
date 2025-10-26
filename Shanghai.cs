using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using Alexandria.ItemAPI;
using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = Alexandria.ItemAPI.CompanionBuilder.AnimationType;
using FlipType = DirectionalAnimation.FlipType;
using MonoMod.RuntimeDetour;
using System.Collections;

namespace OrdinaryMagicianItems
{
    public class Shanghai : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Shanghai";
            string resourceName = "OrdinaryMagicianItems/Resources/shanghai_ammonomicon_icon";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Shanghai>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "The Precious Thing";
            string longDesc = "A little sentient doll. No, it's not yours.\nWhen in danger, it will start following you around and will attack enemies.\nAlice won't forgive you if you lose it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.SPECIAL;
            item.CompanionGuid = "shanghai";
            item.Synergies = new CompanionTransformSynergy[0];
            item.CompanionPastGuid = "";
            item.UsesAlternatePastPrefab = false;
            item.PreventRespawnOnFloorLoad = false;
            item.HasGunTransformationSacrificeSynergy = false;
            item.GunTransformationSacrificeSynergy = CustomSynergyType.AIR_BUSTER;
            item.SacrificeGunID = -1;
            item.SacrificeGunDuration = 0f;
            item.BabyGoodMimicOrbitalOverridden = false;
            item.OverridePlayerOrbitalItem = null;
            BuildPrefab();
        }

        public static void BuildPrefab()
        {
            if (prefab == null && !CompanionBuilder.companionDictionary.ContainsKey("shanghai"))
            {
                prefab = CompanionBuilder.BuildPrefab("Shanghai", "shanghai", "OrdinaryMagicianItems/Resources/companion/idle_right/shanghai_idle_front_right_001", new IntVector2(9, 3), new IntVector2(8, 12));
                var companion = prefab.AddComponent<CompanionController>();
                PixelCollider collider = new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyHitBox,
                    ManualWidth = 8,
                    ManualHeight = 12,
                    ManualOffsetX = 9,
                    ManualOffsetY = 3
                };
                companion.aiActor.IsNormalEnemy = false;
                companion.CanInterceptBullets = false;
                companion.specRigidbody.PixelColliders.Add(collider);
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
                List<string> attackLeftFrames = new List<string>
                {
                    "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_001",
                    "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_002",
                    "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_003",
                    "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_004",
                    "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_005",
                    "OrdinaryMagicianItems/Resources/companion/attack_left/shanghai_attack_front_left_006"
                };
                List<int> indices = new List<int>();
                for (int i = 0; i < attackLeftFrames.Count; i++)
                {
                    indices.Add(SpriteBuilder.AddSpriteToCollection(attackLeftFrames[i], collection));
                }
                tk2dSpriteAnimationClip clip = SpriteBuilder.AddAnimation(companion.spriteAnimator, collection, indices, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once);
                clip.fps = 6;
                List<string> attackRightFrames = new List<string>
                {
                    "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_001",
                    "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_002",
                    "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_003",
                    "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_004",
                    "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_005",
                    "OrdinaryMagicianItems/Resources/companion/attack_right/shanghai_attack_front_right_006"
                };
                List<int> indices2 = new List<int>();
                for (int i = 0; i < attackRightFrames.Count; i++)
                {
                    indices2.Add(SpriteBuilder.AddSpriteToCollection(attackRightFrames[i], collection));
                }
                tk2dSpriteAnimationClip clip2 = SpriteBuilder.AddAnimation(companion.spriteAnimator, collection, indices2, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once);
                clip2.fps = 6;
                DirectionalAnimation animation = new DirectionalAnimation()
                {
                    AnimNames = new string[]
                    {
                        "attack_right",
                        "attack_left"
                    },
                    Flipped = new FlipType[]
                    {
                        FlipType.None,
                        FlipType.None
                    },
                    Type = DirectionType.TwoWayHorizontal,
                    Prefix = string.Empty
                };
                companion.aiAnimator.AssignDirectionalAnimation("attack", animation, AnimationType.Other);
                var bs = prefab.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" }, DisableInCombat = true });
                bs.MovementBehaviors.Add(new SeekTargetBehavior() { StopWhenInRange = false, CustomRange = -1, LineOfSight = true, ReturnToSpawn = false, SpawnTetherDistance = 0f, PathInterval = 0.25f, ExternalCooldownSource = false,
                    SpecifyRange = false, MinActiveRange = 0f, MaxActiveRange = 0f });
                bs.TargetBehaviors.Add(new TargetPlayerBehavior() { Radius = 35f, LineOfSight = true, ObjectPermanence = true, SearchInterval = 0.25f, PauseOnTargetSwitch = false, PauseTime = 0.25f });
                bs.AttackBehaviors.Add(new ShanghaiAttackBehavior() { minLeapDistance = 1f, leapDistance = 2f, maxTravelDistance = 5f, leadAmount = 1f, leapTime = 0.4f, maximumChargeTime = 0.25f, chargeAnim = string.Empty, leapAnim = "attack" });
            }
        }

        public GameObject ExtantCompanion
        {
            get
            {
                return m_extantCompanion;
            }
        }

        private void CreateCompanion(PlayerController owner)
        {
            if (owner.healthHaver != null && owner.healthHaver.GetCurrentHealthPercentage() >= 1)
            {
                return;
            }
            if (PreventRespawnOnFloorLoad)
            {
                return;
            }
            if (BabyGoodMimicOrbitalOverridden)
            {
                GameObject extantCompanion = PlayerOrbitalItem.CreateOrbital(owner, (!OverridePlayerOrbitalItem.OrbitalFollowerPrefab) ? OverridePlayerOrbitalItem.OrbitalPrefab.gameObject : OverridePlayerOrbitalItem.OrbitalFollowerPrefab.gameObject, OverridePlayerOrbitalItem.OrbitalFollowerPrefab, null);
                m_extantCompanion = extantCompanion;
                return;
            }
            string guid = CompanionGuid;
            m_lastActiveSynergyTransformation = -1;
            if (UsesAlternatePastPrefab && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
            {
                guid = CompanionPastGuid;
            }
            else if (Synergies.Length > 0)
            {
                for (int i = 0; i < Synergies.Length; i++)
                {
                    if (owner.HasActiveBonusSynergy(Synergies[i].RequiredSynergy, false))
                    {
                        guid = Synergies[i].SynergyCompanionGuid;
                        m_lastActiveSynergyTransformation = i;
                    }
                }
            }
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
            Vector3 vector = owner.transform.position;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
            {
                vector += new Vector3(1.125f, -0.3125f, 0f);
            }
            GameObject extantCompanion2 = Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
            m_extantCompanion = extantCompanion2;
            CompanionController orAddComponent = m_extantCompanion.GetOrAddComponent<CompanionController>();
            orAddComponent.Initialize(owner);
            if (orAddComponent.specRigidbody)
            {
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
            }
            if (orAddComponent.companionID == CompanionController.CompanionIdentifier.BABY_GOOD_MIMIC)
            {
                GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_GOT_BABY_MIMIC, true);
            }
        }

        public void ForceCompanionRegeneration(PlayerController owner, Vector2? overridePosition)
        {
            bool flag = false;
            Vector2 vector = Vector2.zero;
            if (m_extantCompanion)
            {
                flag = true;
                vector = m_extantCompanion.transform.position.XY();
            }
            if (overridePosition != null)
            {
                flag = true;
                vector = overridePosition.Value;
            }
            DestroyCompanion();
            CreateCompanion(owner);
            if (m_extantCompanion && flag)
            {
                m_extantCompanion.transform.position = vector.ToVector3ZisY(0f);
                SpeculativeRigidbody component = m_extantCompanion.GetComponent<SpeculativeRigidbody>();
                if (component)
                {
                    component.Reinitialize();
                }
            }
        }

        public void ForceDisconnectCompanion()
        {
            m_extantCompanion = null;
        }

        private void DestroyCompanion()
        {
            if (!m_extantCompanion)
            {
                return;
            }
            Destroy(m_extantCompanion);
            m_extantCompanion = null;
        }

        public override void Update()
        {
            base.Update();
            if (!Dungeon.IsGenerating && m_owner && Synergies.Length > 0)
            {
                if (!UsesAlternatePastPrefab || GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST)
                {
                    bool flag = false;
                    for (int i = Synergies.Length - 1; i >= 0; i--)
                    {
                        if (m_owner.HasActiveBonusSynergy(Synergies[i].RequiredSynergy, false))
                        {
                            if (m_lastActiveSynergyTransformation != i)
                            {
                                DestroyCompanion();
                                CreateCompanion(m_owner);
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (!flag && m_lastActiveSynergyTransformation != -1)
                    {
                        DestroyCompanion();
                        CreateCompanion(m_owner);
                    }
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += HandleNewFloor;
            if (player.healthHaver != null)
            {
                player.healthHaver.OnHealthChanged += OnHealthChanged;
                player.healthHaver.OnPreDeath += MaybeRevive;
            }
            CreateCompanion(player);
        }

        public void MaybeRevive(Vector2 deathDir)
        {
            foreach (PassiveItem passive in m_owner.passiveItems)
            {
                if (new List<int> { 421, 422, 423, 424, 425 }.Contains(passive.PickupObjectId))
                {
                    m_owner.RemovePassiveItem(passive.PickupObjectId);
                    m_owner.healthHaver.ApplyHealing(1f);
                    m_owner.ForceBlank();
                    m_owner.ClearDeadFlags();
                    if(GameManager.Instance != null)
                    {
                        if (m_currentTimeSlow != null)
                        {
                            GameManager.Instance.StopCoroutine(m_currentTimeSlow);
                        }
                        m_currentTimeSlow = GameManager.Instance.StartCoroutine(PostReviveTimeSlow());
                    }
                    break;
                }
            }
        }

        public IEnumerator PostReviveTimeSlow()
        {
            BraveTime.ClearMultiplier(gameObject);
            BraveTime.RegisterTimeScaleMultiplier(0.1f, gameObject);
            float elapsed = 0f;
            float duration = 2.5f;
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
            {
                DestroyCompanion();
            }
            else if(ExtantCompanion == null)
            {
                CreateCompanion(Owner);
            }
        }

        private void HandleNewFloor(PlayerController obj)
        {
            DestroyCompanion();
            if (!PreventRespawnOnFloorLoad)
            {
                CreateCompanion(obj);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DestroyCompanion();
            player.OnNewFloorLoaded -= HandleNewFloor;
            if (player.healthHaver != null)
            {
                player.healthHaver.OnHealthChanged -= OnHealthChanged;
                player.healthHaver.OnPreDeath -= MaybeRevive;
            }
            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            if (m_owner != null)
            {
                PlayerController owner = m_owner;
                owner.OnNewFloorLoaded -= HandleNewFloor;
                if (owner.healthHaver != null)
                {
                    owner.healthHaver.OnHealthChanged -= OnHealthChanged;
                    owner.healthHaver.OnPreDeath -= MaybeRevive;
                }
            }
            DestroyCompanion();
            base.OnDestroy();
        }

        public string CompanionGuid;
        private Coroutine m_currentTimeSlow;
        public bool UsesAlternatePastPrefab;
        public string CompanionPastGuid;
        public CompanionTransformSynergy[] Synergies;
        public bool PreventRespawnOnFloorLoad;
        public bool HasGunTransformationSacrificeSynergy;
        public CustomSynergyType GunTransformationSacrificeSynergy;
        public int SacrificeGunID;
        public float SacrificeGunDuration;
        public bool BabyGoodMimicOrbitalOverridden;
        public PlayerOrbitalItem OverridePlayerOrbitalItem;
        private int m_lastActiveSynergyTransformation;
        private GameObject m_extantCompanion;
        public static GameObject prefab;
    }
}
