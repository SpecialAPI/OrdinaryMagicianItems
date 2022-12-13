using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;

namespace OrdinaryMagicianItems
{
	public class AdvancedAmmoRegenSynergyProcessor : MonoBehaviour
	{
		public AdvancedAmmoRegenSynergyProcessor()
		{
			AmmoPerSecond = 0.1f;
			PreventGainWhileFiring = true;
		}

		public void Awake()
		{
			m_gun = GetComponent<Gun>();
		}

		private void Update()
		{
			if (m_gun.CurrentOwner && m_gun.OwnerHasSynergy(RequiredSynergy) && (!PreventGainWhileFiring || !m_gun.IsFiring))
			{
				m_ammoCounter += BraveTime.DeltaTime * AmmoPerSecond;
				if (m_ammoCounter > 1f)
				{
					int num = Mathf.FloorToInt(m_ammoCounter);
					m_ammoCounter -= num;
					m_gun.GainAmmo(num);
				}
			}
		}

		public void OnEnable()
		{
			if (m_gameTimeOnDisable > 0f)
			{
				m_ammoCounter += (Time.time - m_gameTimeOnDisable) * AmmoPerSecond;
				m_gameTimeOnDisable = 0f;
			}
		}

		public void OnDisable()
		{
			m_gameTimeOnDisable = Time.time;
		}

		public string RequiredSynergy;
		public float AmmoPerSecond;
		public bool PreventGainWhileFiring;
		private Gun m_gun;
		private float m_ammoCounter;
		private float m_gameTimeOnDisable;
	}
}
