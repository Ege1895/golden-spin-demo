using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GoldenSpin.UIModule;
using GoldenSpin.InstallerModule;
using GoldenSpin.ZoneModule;

namespace GoldenSpin.Editor
{
    public static class SceneReferenceWirer
    {
        [MenuItem("GoldenSpin/Wire Scene References")]
        public static void WireAll()
        {
            Scene scene = SceneManager.GetActiveScene();

            WireWheelPanel(scene);
            WireZoneBarPanel(scene);
            WireRewardListPanel(scene);
            WireBombPanel(scene);
            WireCollectPanel(scene);
            WireWheelSliceItem(scene);
            WireZoneBarItem(scene);
            WireRewardListItem(scene);
            WireAppInstaller(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[SceneReferenceWirer] All references wired and scene saved.");
        }

        private static T Find<T>(string path) where T : Component
        {
            GameObject go = GameObject.Find(path);
            if (go == null)
            {
                Debug.LogWarning($"[SceneReferenceWirer] Not found: {path}");
                return null;
            }
            return go.GetComponent<T>();
        }

        private static GameObject FindGO(string name)
        {
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in all)
                if (go.name == name && go.scene.isLoaded) return go;
            return null;
        }

        private static GameObject FindChildGO(GameObject parent, string name)
        {
            Transform t = parent.transform.Find(name);
            return t != null ? t.gameObject : null;
        }

        private static T FindChild<T>(GameObject parent, string name) where T : Component
        {
            GameObject go = FindChildGO(parent, name);
            return go != null ? go.GetComponent<T>() : null;
        }

        private static T FindInactive<T>(string name) where T : Component
        {
            GameObject go = FindGO(name);
            if (go == null) { Debug.LogWarning($"[SceneReferenceWirer] Not found inactive: {name}"); return null; }
            return go.GetComponent<T>();
        }

        private static void WireWheelPanel(Scene scene)
        {
            WheelPanel wp = FindInactive<WheelPanel>("Panel_Wheel");
            if (wp == null) return;

            wp.GetType().GetField("canvasGroup",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("Panel_Wheel")?.GetComponent<UnityEngine.CanvasGroup>());
            wp.GetType().GetField("wheelTransform",    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_root_wheel")?.GetComponent<RectTransform>());
            wp.GetType().GetField("slicesContainer",   System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_container_slices")?.GetComponent<RectTransform>());
            wp.GetType().GetField("indicatorTransform",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_image_wheel_indicator")?.GetComponent<RectTransform>());
            wp.GetType().GetField("uiButtonSpin",      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_button_spin")?.GetComponent<UnityEngine.UI.Button>());
            wp.GetType().GetField("uiButtonLeave",     System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_button_leave")?.GetComponent<UnityEngine.UI.Button>());
            wp.GetType().GetField("uiTextZoneTypeValue",   System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_text_zone_type_value")?.GetComponent<TMPro.TextMeshProUGUI>());
            wp.GetType().GetField("uiTextZoneNumberValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_text_zone_number_value")?.GetComponent<TMPro.TextMeshProUGUI>());
            wp.GetType().GetField("uiImageSpinBackground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_image_wheel_glow")?.GetComponent<UnityEngine.UI.Image>());

            EditorUtility.SetDirty(wp);
            Debug.Log("[SceneReferenceWirer] WheelPanel wired.");
        }

        private static void WireZoneBarPanel(Scene scene)
        {
            ZoneBarPanel zbp = FindInactive<ZoneBarPanel>("Panel_ZoneBar");
            if (zbp == null) return;

            zbp.GetType().GetField("canvasGroup",    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(zbp, FindGO("Panel_ZoneBar")?.GetComponent<UnityEngine.CanvasGroup>());
            zbp.GetType().GetField("zonesContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(zbp, FindGO("ui_container_zone_items")?.GetComponent<RectTransform>());
            zbp.GetType().GetField("scrollContent",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(zbp, FindGO("Panel_ZoneBar")?.GetComponent<RectTransform>());

            EditorUtility.SetDirty(zbp);
            Debug.Log("[SceneReferenceWirer] ZoneBarPanel wired.");
        }

        private static void WireRewardListPanel(Scene scene)
        {
            RewardListPanel rlp = FindInactive<RewardListPanel>("Panel_RewardList");
            if (rlp == null) return;

            rlp.GetType().GetField("canvasGroup",      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(rlp, FindGO("Panel_RewardList")?.GetComponent<UnityEngine.CanvasGroup>());
            rlp.GetType().GetField("rewardsContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(rlp, FindGO("ui_container_rewards")?.GetComponent<RectTransform>());

            EditorUtility.SetDirty(rlp);
            Debug.Log("[SceneReferenceWirer] RewardListPanel wired.");
        }

        private static void WireBombPanel(Scene scene)
        {
            BombPanel bp = FindInactive<BombPanel>("Panel_Bomb");
            if (bp == null) return;

            bp.GetType().GetField("canvasGroup",           System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("Panel_Bomb")?.GetComponent<UnityEngine.CanvasGroup>());
            bp.GetType().GetField("uiButtonRestart",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_button_restart")?.GetComponent<UnityEngine.UI.Button>());
            bp.GetType().GetField("uiButtonRevive",        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_button_revive")?.GetComponent<UnityEngine.UI.Button>());
            bp.GetType().GetField("uiTextReviveCostValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_text_revive_cost_value")?.GetComponent<TMPro.TextMeshProUGUI>());
            bp.GetType().GetField("uiTextBombTitle",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_text_bomb_title")?.GetComponent<TMPro.TextMeshProUGUI>());
            bp.GetType().GetField("uiTextBombSubtitle",    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_text_bomb_subtitle")?.GetComponent<TMPro.TextMeshProUGUI>());
            bp.GetType().GetField("uiImageBombIcon",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_image_bomb_icon")?.GetComponent<UnityEngine.UI.Image>());

            EditorUtility.SetDirty(bp);
            Debug.Log("[SceneReferenceWirer] BombPanel wired.");
        }

        private static void WireCollectPanel(Scene scene)
        {
            CollectPanel cp = FindInactive<CollectPanel>("Panel_Collect");
            if (cp == null) return;

            cp.GetType().GetField("canvasGroup",              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("Panel_Collect")?.GetComponent<UnityEngine.CanvasGroup>());
            cp.GetType().GetField("uiButtonPlayAgain",        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_button_play_again")?.GetComponent<UnityEngine.UI.Button>());
            cp.GetType().GetField("rewardSummaryContainer",   System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_container_reward_summary")?.GetComponent<RectTransform>());
            cp.GetType().GetField("uiTextCollectTitle",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_text_collect_title")?.GetComponent<TMPro.TextMeshProUGUI>());
            cp.GetType().GetField("uiTextTotalRewardsValue",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_text_total_rewards_value")?.GetComponent<TMPro.TextMeshProUGUI>());
            cp.GetType().GetField("confettiEffect",           System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("confettiEffect"));

            EditorUtility.SetDirty(cp);
            Debug.Log("[SceneReferenceWirer] CollectPanel wired.");
        }

        private static void WireAppInstaller(Scene scene)
        {
            AppInstaller ai = FindInactive<AppInstaller>("GameContext");
            if (ai == null) return;

            ai.GetType().GetField("zoneConfigSO",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, AssetDatabase.LoadAssetAtPath<ZoneConfigSO>(
                    "Assets/_Project/AddressableAssets/Data/ZoneConfigSO.asset"));
            ai.GetType().GetField("wheelPanel",         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<WheelPanel>("Panel_Wheel"));
            ai.GetType().GetField("zoneBarPanel",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<ZoneBarPanel>("Panel_ZoneBar"));
            ai.GetType().GetField("rewardListPanel",    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<RewardListPanel>("Panel_RewardList"));
            ai.GetType().GetField("bombPanel",          System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<BombPanel>("Panel_Bomb"));
            ai.GetType().GetField("collectPanel",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<CollectPanel>("Panel_Collect"));
            ai.GetType().GetField("sliceItemPrefab",    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<WheelSliceItem>("WheelSliceItem"));
            ai.GetType().GetField("zoneBarItemPrefab",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<ZoneBarItem>("ZoneBarItem"));
            ai.GetType().GetField("rewardListItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<RewardListItem>("RewardListItem"));

            EditorUtility.SetDirty(ai);
            Debug.Log("[SceneReferenceWirer] AppInstaller wired.");
        }

        private static void WireWheelSliceItem(Scene scene)
        {
            WheelSliceItem item = FindInactive<WheelSliceItem>("WheelSliceItem");
            if (item == null) return;

            GameObject root = item.gameObject;
            var f = item.GetType();
            f.GetField("uiImageSliceBackground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_slice_background"));
            f.GetField("uiImageSliceIcon",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_slice_icon"));
            f.GetField("uiTextSliceLabelValue",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_slice_label_value"));
            f.GetField("bombIndicator",          System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChildGO(root, "ui_game_bomb_indicator"));

            EditorUtility.SetDirty(item);
            Debug.Log("[SceneReferenceWirer] WheelSliceItem wired.");
        }

        private static void WireZoneBarItem(Scene scene)
        {
            ZoneBarItem item = FindInactive<ZoneBarItem>("ZoneBarItem");
            if (item == null) return;

            GameObject root = item.gameObject;
            var f = item.GetType();
            f.GetField("uiImageZoneBackground",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_zone_background"));
            f.GetField("uiTextZoneNumberValue",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_zone_number_value"));
            f.GetField("uiImageZoneIcon",        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_zone_icon"));

            EditorUtility.SetDirty(item);
            Debug.Log("[SceneReferenceWirer] ZoneBarItem wired.");
        }

        private static void WireRewardListItem(Scene scene)
        {
            RewardListItem item = FindInactive<RewardListItem>("RewardListItem");
            if (item == null) return;

            GameObject root = item.gameObject;
            var f = item.GetType();
            f.GetField("uiImageRewardIcon",       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_reward_icon"));
            f.GetField("uiTextRewardLabelValue",  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_reward_label_value"));
            f.GetField("uiTextRewardAmountValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_reward_amount_value"));

            EditorUtility.SetDirty(item);
            Debug.Log("[SceneReferenceWirer] RewardListItem wired.");
        }
    }
}
