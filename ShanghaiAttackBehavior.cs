using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace OrdinaryMagicianItems
{
	public class ShanghaiAttackBehavior : AttackBehaviorBase
	{
		public ShanghaiAttackBehavior()
		{
			minLeapDistance = 1f;
			leapDistance = 4f;
			maxTravelDistance = 5f;
			leapTime = 0.75f;
			maximumChargeTime = 0.25f;
		}

		public override BehaviorResult Update()
		{
			base.Update();
			BehaviorResult behaviorResult = base.Update();
			if (behaviorResult != BehaviorResult.Continue)
			{
				return behaviorResult;
			}
			if (!m_aiActor.TargetRigidbody)
			{
				return BehaviorResult.Continue;
			}
			Vector2 vector = m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
			if (leadAmount > 0f)
			{
				Vector2 b = vector + m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
				vector = Vector2.Lerp(vector, b, leadAmount);
			}
			float num = Vector2.Distance(m_aiActor.specRigidbody.UnitCenter, vector);
			if (num > minLeapDistance && num < leapDistance)
			{
				m_state = State.Charging;
                if (!string.IsNullOrEmpty(chargeAnim))
				{
					m_aiAnimator.PlayForDuration(chargeAnim, maximumChargeTime, true, null, -1f, false);
				}
                else
                {
					charge_elapsed = maximumChargeTime;
                }
				m_aiActor.ClearPath();
				m_aiActor.BehaviorOverridesVelocity = true;
				m_aiActor.BehaviorVelocity = Vector2.zero;
				m_updateEveryFrame = true;
				return BehaviorResult.RunContinuous;
			}
			return BehaviorResult.Continue;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			if (m_state == State.Charging)
			{
				if (!m_aiAnimator.IsPlaying(chargeAnim) && charge_elapsed <= 0f)
				{
					m_state = State.Leaping;
					m_elapsed = 0f;
					if (!m_aiActor.TargetRigidbody || !m_aiActor.TargetRigidbody.enabled)
					{
						m_state = State.Idle;
						return ContinuousBehaviorResult.Finished;
					}
					Vector2 unitCenter = m_aiActor.specRigidbody.UnitCenter;
					Vector2 vector = m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
					if (leadAmount > 0f)
					{
						Vector2 b = vector + m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
						vector = Vector2.Lerp(vector, b, leadAmount);
					}
					float num = Vector2.Distance(unitCenter, vector);
					if (num > maxTravelDistance)
					{
						vector = unitCenter + (vector - unitCenter).normalized * maxTravelDistance;
						num = Vector2.Distance(unitCenter, vector);
					}
					m_aiActor.ClearPath();
					m_aiActor.BehaviorOverridesVelocity = true;
					m_aiActor.BehaviorVelocity = (vector - unitCenter).normalized * (num / leapTime);
					float facingDirection = m_aiActor.BehaviorVelocity.ToAngle();
					m_aiAnimator.LockFacingDirection = true;
					m_aiAnimator.FacingDirection = facingDirection;
					m_aiActor.PathableTiles = (CellTypes.FLOOR | CellTypes.PIT);
					m_aiActor.DoDustUps = false;
                    if (!string.IsNullOrEmpty(leapAnim))
					{
						m_aiAnimator.PlayUntilFinished(leapAnim, true, null, -1f, false);
					}
				}
                else
                {
					charge_elapsed = Mathf.Max(0f, charge_elapsed - BraveTime.DeltaTime);
                }
			}
			else if (m_state == State.Leaping)
			{
				m_elapsed += m_deltaTime;
				if (m_elapsed >= leapTime)
				{
					return ContinuousBehaviorResult.Finished;
				}
			}
			return ContinuousBehaviorResult.Continue;
		}

		public override void EndContinuousUpdate()
		{
			base.EndContinuousUpdate();
			if (m_aiActor.TargetRigidbody && m_aiActor.TargetRigidbody.healthHaver)
			{
				m_aiActor.TargetRigidbody.healthHaver.ApplyDamage(6f, m_aiActor.specRigidbody.Velocity, "Shanghai", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
			}
			m_state = State.Idle;
			m_aiActor.PathableTiles = CellTypes.FLOOR;
			m_aiActor.DoDustUps = true;
			m_aiActor.BehaviorOverridesVelocity = false;
			m_aiAnimator.LockFacingDirection = false;
			m_updateEveryFrame = false;
		}

		public override bool IsReady()
		{
			return true;
		}

		public override float GetMinReadyRange()
		{
			return leapDistance;
		}

		public override float GetMaxRange()
		{
			return leapDistance;
		}

		public float minLeapDistance;
		public float leapDistance;
		public float maxTravelDistance;
		public float leadAmount;
		public float leapTime;
		public float maximumChargeTime;
		public string chargeAnim;
		public string leapAnim;
		private float m_elapsed;
		private State m_state;
		private float charge_elapsed;
		private enum State
		{
			Idle,
			Charging,
			Leaping
		}
	}
}
