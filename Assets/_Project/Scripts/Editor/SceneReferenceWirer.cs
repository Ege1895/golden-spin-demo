using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GoldenSpin.UIModule;
using GoldenSpin.InstallerModule;
using GoldenSpin.ZoneModule;
using GoldenSpin.WheelModule;
using GoldenSpin.GameModule;
using GoldenSpin.RewardModule;

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
            WireTopBarPanel(scene);
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
            {
                if (go.name == name && go.scene.isLoaded)
                {
                    return go;
                }
            }

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

            if (go == null)
            {
                Debug.LogWarning($"[SceneReferenceWirer] Not found inactive: {name}");
                return null;
            }

            return go.GetComponent<T>();
        }

        private static void WireWheelPanel(Scene scene)
        {
            WheelUIPanel wp = FindInactive<WheelUIPanel>("Panel_Wheel");
            if (wp == null)
            {
                return;
            }

            Type t = wp.GetType();
            t.GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("Panel_Wheel")?.GetComponent<UnityEngine.CanvasGroup>());
            t.GetField("wheelTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_root_wheel")?.GetComponent<RectTransform>());
            t.GetField("slicesContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_container_slices")?.GetComponent<RectTransform>());
            t.GetField("indicatorTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_image_wheel_indicator")?.GetComponent<RectTransform>());
            t.GetField("uiButtonSpin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_button_spin")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("uiButtonCollect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_button_collect")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("uiTextWheelTitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_text_wheel_title")?.GetComponent<TMPro.TextMeshProUGUI>());
            t.GetField("uiImageSpinBackground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wp, FindGO("ui_image_wheel_glow")?.GetComponent<UnityEngine.UI.Image>());

            EditorUtility.SetDirty(wp);
            Debug.Log("[SceneReferenceWirer] WheelPanel wired.");
        }

        private static void WireZoneBarPanel(Scene scene)
        {
            ZoneBarUIPanel zbp = FindInactive<ZoneBarUIPanel>("Panel_ZoneBar");
            if (zbp == null)
            {
                return;
            }

            Type t = zbp.GetType();
            t.GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(zbp, FindGO("Panel_ZoneBar")?.GetComponent<UnityEngine.CanvasGroup>());
            t.GetField("zonesContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(zbp, FindGO("ui_container_zone_items")?.GetComponent<RectTransform>());
            t.GetField("scrollContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(zbp, FindGO("Panel_ZoneBar")?.GetComponent<RectTransform>());

            EditorUtility.SetDirty(zbp);
            Debug.Log("[SceneReferenceWirer] ZoneBarPanel wired.");
        }

        private static void WireRewardListPanel(Scene scene)
        {
            RewardListUIPanel rlp = FindInactive<RewardListUIPanel>("Panel_RewardList");
            if (rlp == null)
            {
                return;
            }

            Type t = rlp.GetType();
            t.GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(rlp, FindGO("Panel_RewardList")?.GetComponent<UnityEngine.CanvasGroup>());
            t.GetField("rewardsContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(rlp, FindGO("ui_container_rewards")?.GetComponent<RectTransform>());

            EditorUtility.SetDirty(rlp);
            Debug.Log("[SceneReferenceWirer] RewardListPanel wired.");
        }

        private static void WireBombPanel(Scene scene)
        {
            BombUIPanel bp = FindInactive<BombUIPanel>("Panel_Bomb");
            if (bp == null)
            {
                return;
            }

            Type t = bp.GetType();
            t.GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("Panel_Bomb")?.GetComponent<UnityEngine.CanvasGroup>());
            t.GetField("uiButtonGiveUp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_button_give_up")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("uiButtonRevive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_button_revive")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("uiButtonRevivePaid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_button_revive_paid")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("uiTextRevivePaidCostValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_text_revive_paid_cost_value")?.GetComponent<TMPro.TextMeshProUGUI>());
            t.GetField("uiTextBombTitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_text_bomb_title")?.GetComponent<TMPro.TextMeshProUGUI>());
            t.GetField("uiTextBombSubtitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_text_bomb_subtitle")?.GetComponent<TMPro.TextMeshProUGUI>());
            t.GetField("uiImageBombIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(bp, FindGO("ui_image_bomb_icon")?.GetComponent<UnityEngine.UI.Image>());

            EditorUtility.SetDirty(bp);
            Debug.Log("[SceneReferenceWirer] BombPanel wired.");
        }

        private static void WireCollectPanel(Scene scene)
        {
            CollectUIPanel cp = FindInactive<CollectUIPanel>("Panel_Collect");
            if (cp == null)
            {
                return;
            }

            Type t = cp.GetType();
            t.GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("Panel_Collect")?.GetComponent<UnityEngine.CanvasGroup>());
            t.GetField("uiButtonRevive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_button_revive")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("uiButtonGiveUp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_button_give_up")?.GetComponent<UnityEngine.UI.Button>());
            t.GetField("rewardSummaryContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_container_reward_summary")?.GetComponent<RectTransform>());
            t.GetField("uiTextCollectTitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_text_collect_title")?.GetComponent<TMPro.TextMeshProUGUI>());
            t.GetField("uiTextTotalRewardsValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cp, FindGO("ui_text_total_rewards_value")?.GetComponent<TMPro.TextMeshProUGUI>());

            EditorUtility.SetDirty(cp);
            Debug.Log("[SceneReferenceWirer] CollectPanel wired.");
        }

        private static void WireTopBarPanel(Scene scene)
        {
            TopBarUIPanel tbp = FindInactive<TopBarUIPanel>("ui_panel_top_bar");
            if (tbp == null)
            {
                return;
            }

            Type t = tbp.GetType();
            t.GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tbp, FindGO("ui_panel_top_bar")?.GetComponent<UnityEngine.CanvasGroup>());
            t.GetField("uiImageZoneDot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tbp, FindGO("ui_image_zone_dot")?.GetComponent<UnityEngine.UI.Image>());
            t.GetField("uiTextZoneLabelValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tbp, FindGO("ui_text_zone_label_value")?.GetComponent<TMPro.TextMeshProUGUI>());
            t.GetField("uiTextCurrencyAmountValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tbp, FindGO("ui_text_currency_amount_value")?.GetComponent<TMPro.TextMeshProUGUI>());

            EditorUtility.SetDirty(tbp);
            Debug.Log("[SceneReferenceWirer] TopBarPanel wired.");
        }

        private static void WireAppInstaller(Scene scene)
        {
            AppInstaller ai = FindInactive<AppInstaller>("GameContext");
            if (ai == null)
            {
                return;
            }

            Type t = ai.GetType();
            t.GetField("zoneConfigSO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, AssetDatabase.LoadAssetAtPath<ZoneConfigSO>(
                    "Assets/_Project/AddressableAssets/Data/ZoneConfigSO.asset"));
            t.GetField("wheelPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<WheelUIPanel>("Panel_Wheel"));
            t.GetField("zoneBarPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<ZoneBarUIPanel>("Panel_ZoneBar"));
            t.GetField("rewardListPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<RewardListUIPanel>("Panel_RewardList"));
            t.GetField("bombPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<BombUIPanel>("Panel_Bomb"));
            t.GetField("collectPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<CollectUIPanel>("Panel_Collect"));
            t.GetField("topBarPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<TopBarUIPanel>("ui_panel_top_bar"));
            t.GetField("sliceItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<WheelSliceItem>("WheelSliceItem"));
            t.GetField("zoneBarItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<ZoneBarItem>("ZoneBarItem"));
            t.GetField("rewardListItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ai, FindInactive<RewardListItem>("RewardListItem"));

            EditorUtility.SetDirty(ai);
            Debug.Log("[SceneReferenceWirer] AppInstaller wired.");
        }

        private static void WireWheelSliceItem(Scene scene)
        {
            WheelSliceItem item = FindInactive<WheelSliceItem>("WheelSliceItem");
            if (item == null)
            {
                return;
            }

            GameObject root = item.gameObject;
            Type t = item.GetType();
            t.GetField("uiImageSliceIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_slice_icon"));
            t.GetField("sliceIconFitter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<AspectRatioFitter>(root, "ui_image_slice_icon"));
            t.GetField("uiTextSliceLabelValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_slice_label_value"));
            t.GetField("bombIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChildGO(root, "ui_game_bomb_indicator"));

            EditorUtility.SetDirty(item);
            Debug.Log("[SceneReferenceWirer] WheelSliceItem wired.");
        }

        private static void WireZoneBarItem(Scene scene)
        {
            ZoneBarItem item = FindInactive<ZoneBarItem>("ZoneBarItem");
            if (item == null)
            {
                return;
            }

            GameObject root = item.gameObject;
            Type t = item.GetType();
            t.GetField("uiImageZoneBackground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_zone_background"));
            t.GetField("uiTextZoneNumberValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_zone_number_value"));
            t.GetField("uiImageZoneIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_zone_icon"));

            EditorUtility.SetDirty(item);
            Debug.Log("[SceneReferenceWirer] ZoneBarItem wired.");
        }

        private static void WireRewardListItem(Scene scene)
        {
            RewardListItem item = FindInactive<RewardListItem>("RewardListItem");
            if (item == null)
            {
                return;
            }

            GameObject root = item.gameObject;
            Type t = item.GetType();
            t.GetField("uiImageRewardIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<UnityEngine.UI.Image>(root, "ui_image_reward_icon"));
            t.GetField("uiTextRewardLabelValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, FindChild<TMPro.TextMeshProUGUI>(root, "ui_text_reward_label_value"));

            EditorUtility.SetDirty(item);
            Debug.Log("[SceneReferenceWirer] RewardListItem wired.");
        }
    }
}
