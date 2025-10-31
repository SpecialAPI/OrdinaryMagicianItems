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
        public float minLeapDistance = 1f;
        public float leapDistance = 4f;
        public float maxTravelDistance = 5f;
        public float leadAmount;
        public float leapTime = 0.75f;
        public float maximumChargeTime = 0.25f;
        public string chargeAnim;
        public string leapAnim;
        private float m_elapsed;
        private State m_state;
        private float charge_elapsed;

        public override BehaviorResult Update()
        {
            var behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;

            if (!m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;

            var targetPos = m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            if (leadAmount > 0f)
            {
                var predictedPos = targetPos + m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
                targetPos = Vector2.Lerp(targetPos, predictedPos, leadAmount);
            }
            var dist = Vector2.Distance(m_aiActor.specRigidbody.UnitCenter, targetPos);
            if (dist <= minLeapDistance || dist >= leapDistance)
                return BehaviorResult.Continue;

            m_state = State.Charging;
            if (!string.IsNullOrEmpty(chargeAnim))
                m_aiAnimator.PlayForDuration(chargeAnim, maximumChargeTime, true, null, -1f, false);
            else
                charge_elapsed = maximumChargeTime;

            m_aiActor.ClearPath();
            m_aiActor.BehaviorOverridesVelocity = true;
            m_aiActor.BehaviorVelocity = Vector2.zero;
            m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (m_state == State.Charging)
            {
                if (m_aiAnimator.IsPlaying(chargeAnim) || charge_elapsed > 0f)
                {
                    charge_elapsed = Mathf.Max(0f, charge_elapsed - BraveTime.DeltaTime);
                    return ContinuousBehaviorResult.Continue;
                }

                m_state = State.Leaping;
                m_elapsed = 0f;
                if (!m_aiActor.TargetRigidbody || !m_aiActor.TargetRigidbody.enabled)
                {
                    m_state = State.Idle;
                    return ContinuousBehaviorResult.Finished;
                }

                var selfPos = m_aiActor.specRigidbody.UnitCenter;
                var targetPos = m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                if (leadAmount > 0f)
                {
                    var predictedPos = targetPos + m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
                    targetPos = Vector2.Lerp(targetPos, predictedPos, leadAmount);
                }

                var dist = Vector2.Distance(selfPos, targetPos);
                if (dist > maxTravelDistance)
                {
                    targetPos = selfPos + (targetPos - selfPos).normalized * maxTravelDistance;
                    dist = Vector2.Distance(selfPos, targetPos);
                }

                m_aiActor.ClearPath();
                m_aiActor.BehaviorOverridesVelocity = true;
                m_aiActor.BehaviorVelocity = (targetPos - selfPos).normalized * (dist / leapTime);
                m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
                m_aiActor.DoDustUps = false;

                var facingDirection = m_aiActor.BehaviorVelocity.ToAngle();
                m_aiAnimator.LockFacingDirection = true;
                m_aiAnimator.FacingDirection = facingDirection;

                if (!string.IsNullOrEmpty(leapAnim))
                    m_aiAnimator.PlayUntilFinished(leapAnim, true, null, -1f, false);
            }
            else if (m_state == State.Leaping)
            {
                m_elapsed += m_deltaTime;
                if (m_elapsed >= leapTime)
                    return ContinuousBehaviorResult.Finished;
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (m_aiActor.TargetRigidbody && m_aiActor.TargetRigidbody.healthHaver)
                m_aiActor.TargetRigidbody.healthHaver.ApplyDamage(6f, m_aiActor.specRigidbody.Velocity, "Shanghai", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);

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

        private enum State
        {
            Idle,
            Charging,
            Leaping
        }
    }
}
