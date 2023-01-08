using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using Gungeon;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Collections;
using BepInEx;

namespace OrdinaryMagicianItems
{
    [BepInPlugin("spapi.etg.ordinarymagicianitems", "Ordinary Magician Items", "1.0.4")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager man)
        {
            FakePrefabHooks.Init();
            ItemBuilder.Init();
            MagicianPistol.Init();
            MiniHakkero.Init();
            Broom.Init();
            Shanghai.Init();
            CustomSynergies.Add("Twelve Shots", new List<string> { "witch_pistol", "spapi:magician_pistol" }, ignoreLichEyeBullets: false);
            CustomSynergies.Add("Cheat against the Impossible", new List<string> { "staff_of_firepower", "spapi:mini_hakkero" }, ignoreLichEyeBullets: false);
            CustomSynergies.Add("Incident-Resolver kit", new List<string> { "spapi:ordinary_broom", "backpack" }, ignoreLichEyeBullets: false);
            CustomSynergies.Add("Extend!", new List<string> { "spapi:shanghai" }, new List<string> { "heart_holster", "heart_lunchbox", "heart_locket", "heart_bottle", "heart_purse" }, ignoreLichEyeBullets: false);
            AdvancedAmmoRegenSynergyProcessor processor = Game.Items["staff_of_firepower"].gameObject.AddComponent<AdvancedAmmoRegenSynergyProcessor>();
            processor.PreventGainWhileFiring = false;
            processor.RequiredSynergy = "Cheat against the Impossible";
            processor.AmmoPerSecond = 1f;
        }

        public static void DebugLog(string message)
        {
            Debug.Log(message);
        }

        public static void ConsoleLog(string message)
        {
            ETGModConsole.Log(message);
        }
    }
}