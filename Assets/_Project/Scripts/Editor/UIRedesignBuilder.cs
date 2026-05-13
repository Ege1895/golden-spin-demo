using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using GoldenSpin.UIModule;
using GoldenSpin.InstallerModule;
using GoldenSpin.WheelModule;
using GoldenSpin.GameModule;

namespace GoldenSpin.Editor
{
    public static class UIRedesignBuilder
    {
        // ─── Colors ────────────────────────────────────────────────
        private static readonly Color ColCardBg        = new Color(1f,    1f,    1f,    0.025f);
        private static readonly Color ColCardBorder    = new Color(1f,    1f,    1f,    0.07f);
        private static readonly Color ColBadgeBg       = new Color(1f,    1f,    1f,    0.05f);
        private static readonly Color ColBadgeBorder   = new Color(1f,    1f,    1f,    0.09f);
        private static readonly Color ColCurrencyBg    = new Color(1f,    0.84f, 0f,    0.08f);
        private static readonly Color ColCurrencyBorder= new Color(1f,    0.84f, 0f,    0.20f);
        private static readonly Color ColCurrencyText  = new Color(1f,    0.84f, 0f,    1f);
        private static readonly Color ColSectionTitle  = new Color(0.40f, 0.40f, 0.40f, 1f);
        private static readonly Color ColZoneText      = new Color(0.73f, 0.73f, 0.73f, 1f);
        private static readonly Color ColDotNormal     = new Color(0.72f, 0.39f, 0.10f, 1f);

        private static readonly Color ColBtnSpinBg     = new Color(0.065f, 0.145f, 0.265f, 1f);
        private static readonly Color ColBtnSpinBorder = new Color(0.105f, 0.376f, 0.800f, 1f);
        private static readonly Color ColBtnSpinText   = new Color(0.42f,  0.69f,  1f,    1f);
        private static readonly Color ColBtnCollectOff = new Color(0.067f, 0.067f, 0.067f, 1f);
        private static readonly Color ColBtnCollectTxt = new Color(0.20f,  0.20f,  0.20f, 1f);
        private static readonly Color ColBtnGiveUp     = new Color(0.054f, 0.054f, 0.054f, 1f);
        private static readonly Color ColBtnReviveBg   = new Color(0.07f,  0.128f, 0.282f, 1f);
        private static readonly Color ColBtnReviveTxt  = new Color(0.40f,  0.60f,  1f,    1f);
        private static readonly Color ColBtnPaidBg     = new Color(0.415f, 0.282f, 0f,    1f);
        private static readonly Color ColBtnPaidText   = new Color(1f,    0.867f, 0.333f, 1f);
        private static readonly Color ColCountBadge    = new Color(1f,    0.84f,  0f,    0.10f);
        private static readonly Color ColCountBorder   = new Color(1f,    0.84f,  0f,    0.20f);
        private static readonly Color ColCountText     = new Color(1f,    0.84f,  0f,    1f);

        // ─── Sprites ──────────────────────────────────────────────
        private static Sprite spriteRound12;
        private static Sprite spriteBtnRect;
        private static Sprite spriteGoldIcon;

        // ─── Entry Point ──────────────────────────────────────────
        [MenuItem("GoldenSpin/Build UI Redesign")]
        public static void BuildUIRedesign()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("[UIRedesign] Stop Play Mode before running this builder!");
                return;
            }

            LoadSprites();

            var canvas = GameObject.Find("Canvas");
            if (canvas == null) { Debug.LogError("[UIRedesign] Canvas not found."); return; }

            var panelWheel   = canvas.transform.Find("Panel_Wheel")?.gameObject;
            var panelReward  = canvas.transform.Find("Panel_RewardList")?.gameObject;
            var panelZoneBar = canvas.transform.Find("Panel_ZoneBar")?.gameObject;
            var panelBomb    = canvas.transform.Find("Panel_Bomb")?.gameObject;

            if (!Validate(panelWheel,   "Panel_Wheel"))      return;
            if (!Validate(panelReward,  "Panel_RewardList")) return;
            if (!Validate(panelZoneBar, "Panel_ZoneBar"))    return;
            if (!Validate(panelBomb,    "Panel_Bomb"))       return;

            Undo.RegisterFullObjectHierarchyUndo(canvas, "GoldenSpin UI Redesign");

            BuildTopBar(panelWheel.transform);
            BuildActionRow(panelWheel.transform);
            RedesignRewardPanel(panelReward.transform);
            RedesignZoneBarPanel(panelZoneBar.transform);
            RedesignBombPopup(panelBomb.transform);
            WireSceneReferences(panelWheel.transform, panelBomb.transform);

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[UIRedesign] Done! Save the scene to persist changes.");
        }

        // ─── TOP BAR ──────────────────────────────────────────────
        private static void BuildTopBar(Transform panelWheel)
        {
            DestroyChild(panelWheel, "ui_panel_top_bar");

            var root = NewGO("ui_panel_top_bar", panelWheel, sibling: 0);
            SetRect(root, V2(0, 1), V2(1, 1), V2(0.5f, 1), V2(0, 0), V2(0, 90));
            // Dark near-transparent background
            AddImage(root, spriteRound12, Image.Type.Sliced, new Color(0.04f, 0.04f, 0.06f, 0.0f), raycast: false);

            // Add CanvasGroup so BasePanel.Awake won't hide it (alpha starts at 1)
            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = false;

            // Horizontal layout group
            var hlg = root.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(14, 14, 16, 8);
            hlg.spacing = 0;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;

            // ── Zone Badge (left) ──────────────────────────────────
            var badge = NewGO("ui_container_zone_badge", root.transform);
            var badgeLE = badge.AddComponent<LayoutElement>();
            badgeLE.preferredWidth = 118;
            badgeLE.flexibleWidth = 0;

            var badgeBg = AddImage(badge, spriteRound12, Image.Type.Sliced, ColBadgeBg, raycast: false);

            var badgeHLG = badge.AddComponent<HorizontalLayoutGroup>();
            badgeHLG.padding = new RectOffset(10, 12, 6, 6);
            badgeHLG.spacing = 7;
            badgeHLG.childAlignment = TextAnchor.MiddleLeft;
            badgeHLG.childForceExpandWidth = false;
            badgeHLG.childForceExpandHeight = false;
            badgeHLG.childControlWidth = false;
            badgeHLG.childControlHeight = false;

            var dot = NewGO("ui_image_zone_dot", badge.transform);
            var dotLE = dot.AddComponent<LayoutElement>();
            dotLE.preferredWidth = 8;
            dotLE.preferredHeight = 8;
            var dotImg = dot.AddComponent<Image>();
            dotImg.sprite = spriteRound12;
            dotImg.type = Image.Type.Simple;
            dotImg.color = ColDotNormal;
            dotImg.raycastTarget = false;

            var zoneTxt = NewGO("ui_text_zone_label_value", badge.transform);
            var zoneTxtLE = zoneTxt.AddComponent<LayoutElement>();
            zoneTxtLE.preferredWidth = 80;
            zoneTxtLE.preferredHeight = 26;
            AddTMP(zoneTxt, "Zone 1", 20, FontStyles.Bold, ColZoneText, TextAlignmentOptions.Left);

            // Spacer
            var spacer = NewGO("ui_spacer", root.transform);
            var spacerLE = spacer.AddComponent<LayoutElement>();
            spacerLE.flexibleWidth = 1;

            // ── Currency Pill (right) ──────────────────────────────
            var pill = NewGO("ui_container_currency_pill", root.transform);
            var pillLE = pill.AddComponent<LayoutElement>();
            pillLE.preferredWidth = 88;
            pillLE.flexibleWidth = 0;

            AddImage(pill, spriteRound12, Image.Type.Sliced, ColCurrencyBg, raycast: false);

            var pillHLG = pill.AddComponent<HorizontalLayoutGroup>();
            pillHLG.padding = new RectOffset(10, 12, 6, 6);
            pillHLG.spacing = 5;
            pillHLG.childAlignment = TextAnchor.MiddleCenter;
            pillHLG.childForceExpandWidth = false;
            pillHLG.childForceExpandHeight = false;
            pillHLG.childControlWidth = false;
            pillHLG.childControlHeight = false;

            var gemIcon = NewGO("ui_image_currency_icon", pill.transform);
            var gemLE = gemIcon.AddComponent<LayoutElement>();
            gemLE.preferredWidth = 22;
            gemLE.preferredHeight = 22;
            AddImage(gemIcon, spriteGoldIcon, Image.Type.Simple, Color.white, raycast: false);

            var currencyAmt = NewGO("ui_text_currency_amount_value", pill.transform);
            var amtLE = currencyAmt.AddComponent<LayoutElement>();
            amtLE.preferredWidth = 44;
            amtLE.preferredHeight = 26;
            AddTMP(currencyAmt, "25", 26, FontStyles.Bold, ColCurrencyText, TextAlignmentOptions.Center);

            // ── Wire TopBarUIPanel component ───────────────────────
            var topBarScript = root.GetComponent<TopBarUIPanel>() ?? root.AddComponent<TopBarUIPanel>();
            // Use SerializedObject to set private [SerializeField] references
            var so = new SerializedObject(topBarScript);
            so.FindProperty("canvasGroup").objectReferenceValue       = cg;
            so.FindProperty("uiImageZoneDot").objectReferenceValue    = dotImg;
            so.FindProperty("uiTextZoneLabelValue").objectReferenceValue   = zoneTxt.GetComponent<TextMeshProUGUI>();
            so.FindProperty("uiTextCurrencyAmountValue").objectReferenceValue = currencyAmt.GetComponent<TextMeshProUGUI>();
            so.ApplyModifiedProperties();
        }

        // ─── ACTION ROW ───────────────────────────────────────────
        private static void BuildActionRow(Transform panelWheel)
        {
            DestroyChild(panelWheel, "ui_action_row");

            var existingSpinBtn = panelWheel.Find("ui_button_spin");

            var row = NewGO("ui_action_row", panelWheel);
            SetRect(row, V2(0, 0), V2(1, 0), V2(0.5f, 0), V2(0, 580), V2(-28, 128));

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing               = 10;
            hlg.padding               = new RectOffset(0, 0, 0, 0);
            hlg.childAlignment        = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            BuildCollectButton(row.transform);

            if (existingSpinBtn != null)
            {
                existingSpinBtn.SetParent(row.transform, false);
                // Override to rectangular sprite
                var spinImg = existingSpinBtn.GetComponent<Image>();
                if (spinImg != null)
                {
                    spinImg.sprite = spriteBtnRect;
                    spinImg.type   = Image.Type.Sliced;
                    spinImg.color  = ColBtnSpinBg;
                    spinImg.raycastTarget = true;
                }
                EnsureLE(existingSpinBtn.gameObject, flexW: 1.3f);
                StyleSpinButton(existingSpinBtn.gameObject);
            }
            else
            {
                BuildSpinButton(row.transform);
            }
        }

        private static void BuildCollectButton(Transform parent)
        {
            var btn = NewGO("ui_button_collect", parent);
            var img = AddImage(btn, spriteBtnRect, Image.Type.Sliced, ColBtnCollectOff);
            var button = btn.GetComponent<Button>() ?? btn.AddComponent<Button>();
            button.targetGraphic = img;
            var cb = button.colors;
            cb.normalColor      = ColBtnCollectOff;
            cb.highlightedColor = new Color(0.10f, 0.30f, 0.10f);
            cb.pressedColor     = new Color(0.05f, 0.18f, 0.05f);
            cb.disabledColor    = new Color(0.04f, 0.04f, 0.04f);
            button.colors = cb;
            EnsureLE(btn, flexW: 1f);

            var vlg = btn.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment        = TextAnchor.MiddleCenter;
            vlg.spacing               = 3;
            vlg.padding               = new RectOffset(8, 8, 10, 10);
            vlg.childForceExpandWidth  = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth     = true;
            vlg.childControlHeight    = true;

            var label = NewGO("ui_text_collect_label", btn.transform);
            AddTMP(label, "COLLECT", 24, FontStyles.Bold, ColBtnCollectTxt, TextAlignmentOptions.Center);
            EnsureLE(label, prefH: 30f);

            var sub = NewGO("ui_text_collect_sub", btn.transform);
            AddTMP(sub, "Safe zones only", 18, FontStyles.Normal,
                   new Color(0.20f, 0.20f, 0.20f, 0.65f), TextAlignmentOptions.Center);
            EnsureLE(sub, prefH: 22f);
        }

        private static void BuildSpinButton(Transform parent)
        {
            var btn = NewGO("ui_button_spin", parent);
            AddImage(btn, spriteBtnRect, Image.Type.Sliced, ColBtnSpinBg);
            btn.AddComponent<Button>();
            EnsureLE(btn, flexW: 1.3f);
            StyleSpinButton(btn);
        }

        private static void StyleSpinButton(GameObject btn)
        {
            DestroyChild(btn.transform, "ui_text_spin_label");
            var label = NewGO("ui_text_spin_label", btn.transform);
            SetRect(label, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-8, 0));
            AddTMP(label, "SPIN", 32, FontStyles.Bold, ColBtnSpinText, TextAlignmentOptions.Center);
        }

        // ─── REWARD PANEL ─────────────────────────────────────────
        private static void RedesignRewardPanel(Transform panelReward)
        {
            // Card background
            var existBg = panelReward.Find("ui_image_panel_bg");
            if (existBg == null)
            {
                var bgGO = NewGO("ui_image_panel_bg", panelReward, sibling: 0);
                SetRect(bgGO, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-16, -8));
                AddImage(bgGO, spriteRound12, Image.Type.Sliced, ColCardBg, raycast: false);
            }
            else
            {
                var bg = existBg.GetComponent<Image>();
                if (bg) { bg.sprite = spriteRound12; bg.type = Image.Type.Sliced; bg.color = ColCardBg; }
            }

            DestroyChild(panelReward, "ui_text_title");
            DestroyChild(panelReward, "ui_section_header");

            var header = NewGO("ui_section_header", panelReward, sibling: 1);
            SetRect(header, V2(0, 1), V2(1, 1), V2(0.5f, 1), V2(0, -10), V2(-32, 52));

            var hlg = header.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment        = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            var title = NewGO("ui_text_section_title", header.transform);
            AddTMP(title, "COLLECTED REWARDS", 20, FontStyles.Bold, ColSectionTitle, TextAlignmentOptions.Left);
            EnsureLE(title, flexW: 1f);

            var countBadge = NewGO("ui_container_count_badge", header.transform);
            AddImage(countBadge, spriteRound12, Image.Type.Sliced, ColCountBadge, raycast: false);
            EnsureLE(countBadge, prefW: 68f, prefH: 34f);

            var countTxt = NewGO("ui_text_reward_count_value", countBadge.transform);
            SetRect(countTxt, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-8, -6));
            AddTMP(countTxt, "0", 20, FontStyles.Bold, ColCountText, TextAlignmentOptions.Center);

            var scrollView = panelReward.Find("Scroll View");
            if (scrollView != null)
            {
                var sRT = scrollView.GetComponent<RectTransform>();
                sRT.anchorMin = V2(0, 0);
                sRT.anchorMax = V2(1, 1);
                sRT.offsetMin = new Vector2(12, 10);
                sRT.offsetMax = new Vector2(-12, -70);
            }
        }

        // ─── ZONE BAR PANEL ───────────────────────────────────────
        private static void RedesignZoneBarPanel(Transform panelZoneBar)
        {
            var existBg = panelZoneBar.Find("ui_image_zonebar_bg");
            if (existBg != null)
            {
                var img = existBg.GetComponent<Image>();
                if (img) { img.sprite = spriteRound12; img.type = Image.Type.Sliced; img.color = ColCardBg; }
                SetRect(existBg.gameObject, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-16, -8));
            }

            DestroyChild(panelZoneBar, "ui_text_title");
            DestroyChild(panelZoneBar, "ui_section_header");

            var header = NewGO("ui_section_header", panelZoneBar, sibling: 1);
            SetRect(header, V2(0, 1), V2(1, 1), V2(0.5f, 1), V2(0, -8), V2(-32, 46));

            var hlg = header.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment        = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            var title = NewGO("ui_text_zone_bar_title", header.transform);
            AddTMP(title, "ZONE PROGRESS", 20, FontStyles.Bold, ColSectionTitle, TextAlignmentOptions.Left);
            EnsureLE(title, flexW: 1f);

            var cur = NewGO("ui_text_zone_current_value", header.transform);
            AddTMP(cur, "1 / 30", 20, FontStyles.Bold, ColCurrencyText, TextAlignmentOptions.Right);
            EnsureLE(cur, prefW: 100f);

            var container = panelZoneBar.Find("ui_container_zone_items");
            if (container != null)
            {
                var cRT = container.GetComponent<RectTransform>();
                cRT.anchorMin = V2(0, 0);
                cRT.anchorMax = V2(1, 1);
                cRT.offsetMin = new Vector2(12, 8);
                cRT.offsetMax = new Vector2(-12, -60);
            }
        }

        // ─── BOMB POPUP ───────────────────────────────────────────
        private static void RedesignBombPopup(Transform panelBomb)
        {
            var popup = panelBomb.Find("Popup_Bomb");
            if (!Validate(popup?.gameObject, "Popup_Bomb")) return;

            DestroyChild(popup, "ui_action_row");

            var btnGiveUp = popup.Find("ui_button_give_up");
            var btnRevive = popup.Find("ui_button_revive");

            var row = NewGO("ui_action_row", popup);
            SetRect(row, V2(0, 0), V2(1, 0), V2(0.5f, 0), V2(0, 24), V2(-28, 110));

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing               = 8;
            hlg.childAlignment        = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            if (btnGiveUp != null)
            {
                btnGiveUp.SetParent(row.transform, false);
                EnsureLE(btnGiveUp.gameObject, flexW: 1f);
                var img = btnGiveUp.GetComponent<Image>();
                if (img) { img.sprite = spriteBtnRect; img.type = Image.Type.Sliced; img.color = ColBtnGiveUp; }
                RefreshButtonText(btnGiveUp, "GIVE UP", 22, ColSectionTitle);
            }

            if (btnRevive != null)
            {
                btnRevive.SetParent(row.transform, false);
                EnsureLE(btnRevive.gameObject, flexW: 1f);
                var img = btnRevive.GetComponent<Image>();
                if (img) { img.sprite = spriteBtnRect; img.type = Image.Type.Sliced; img.color = ColBtnReviveBg; }
                RefreshButtonText(btnRevive, "REVIVE", 22, ColBtnReviveTxt);
            }

            BuildRevivePaidButton(row.transform);
        }

        private static void BuildRevivePaidButton(Transform parent)
        {
            var btn = NewGO("ui_button_revive_paid", parent);
            var img = AddImage(btn, spriteBtnRect, Image.Type.Sliced, ColBtnPaidBg);
            var button = btn.GetComponent<Button>() ?? btn.AddComponent<Button>();
            button.targetGraphic = img;
            EnsureLE(btn, flexW: 1.25f);

            var vlg = btn.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment        = TextAnchor.MiddleCenter;
            vlg.spacing               = 2;
            vlg.padding               = new RectOffset(6, 6, 8, 8);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth     = true;
            vlg.childControlHeight    = true;

            var costRow = NewGO("ui_container_cost", btn.transform);
            var chlg = costRow.AddComponent<HorizontalLayoutGroup>();
            chlg.childAlignment        = TextAnchor.MiddleCenter;
            chlg.spacing               = 4;
            chlg.childForceExpandWidth = false;
            chlg.childForceExpandHeight = false;
            chlg.childControlWidth     = true;
            chlg.childControlHeight    = true;
            EnsureLE(costRow, prefH: 32f);

            var gemTxt = NewGO("ui_image_gem_icon", costRow.transform);
            AddImage(gemTxt, spriteGoldIcon, Image.Type.Simple, Color.white, raycast: false);
            EnsureLE(gemTxt, prefW: 26f, prefH: 28f);

            var costAmt = NewGO("ui_text_revive_paid_cost_value", costRow.transform);
            AddTMP(costAmt, "25", 26, FontStyles.Bold, ColBtnPaidText, TextAlignmentOptions.Center);
            EnsureLE(costAmt, prefW: 42f, prefH: 32f);

            var keepLabel = NewGO("ui_text_keep_rewards", btn.transform);
            AddTMP(keepLabel, "KEEP REWARDS", 18, FontStyles.Bold, ColBtnPaidText, TextAlignmentOptions.Center);
            EnsureLE(keepLabel, prefH: 24f);
        }

        // ─── SCENE WIRING ─────────────────────────────────────────
        private static void WireSceneReferences(Transform panelWheel, Transform panelBomb)
        {
            // WheelUIPanel.uiButtonCollect
            var wheelPanel = panelWheel.GetComponent<WheelUIPanel>();
            if (wheelPanel != null)
            {
                var collectBtn = panelWheel.Find("ui_action_row/ui_button_collect");
                if (collectBtn != null)
                {
                    var so = new SerializedObject(wheelPanel);
                    so.FindProperty("uiButtonCollect").objectReferenceValue = collectBtn.GetComponent<Button>();
                    so.ApplyModifiedProperties();
                    Debug.Log("[UIRedesign] Wired WheelUIPanel.uiButtonCollect");
                }
            }

            // BombUIPanel.uiButtonRevivePaid + uiTextRevivePaidCostValue
            var bombPanel = panelBomb.GetComponent<BombUIPanel>();
            if (bombPanel != null)
            {
                var popup     = panelBomb.Find("Popup_Bomb");
                var revivePaid = popup?.Find("ui_action_row/ui_button_revive_paid");
                var costValue  = revivePaid?.Find("ui_container_cost/ui_text_revive_paid_cost_value");

                if (revivePaid != null || costValue != null)
                {
                    var so = new SerializedObject(bombPanel);
                    if (revivePaid != null)
                        so.FindProperty("uiButtonRevivePaid").objectReferenceValue = revivePaid.GetComponent<Button>();
                    if (costValue != null)
                        so.FindProperty("uiTextRevivePaidCostValue").objectReferenceValue = costValue.GetComponent<TMP_Text>();
                    so.ApplyModifiedProperties();
                    Debug.Log("[UIRedesign] Wired BombUIPanel revive-paid references");
                }
            }

            // AppInstaller.topBarPanel
            var appInstaller = Object.FindAnyObjectByType<AppInstaller>();
            if (appInstaller != null)
            {
                var topBarGO = panelWheel.Find("ui_panel_top_bar");
                var topBarPanel = topBarGO?.GetComponent<TopBarUIPanel>();
                if (topBarPanel != null)
                {
                    var so = new SerializedObject(appInstaller);
                    so.FindProperty("topBarPanel").objectReferenceValue = topBarPanel;
                    so.ApplyModifiedProperties();
                    Debug.Log("[UIRedesign] Wired AppInstaller.topBarPanel");
                }
            }
        }

        // ─── HELPERS ──────────────────────────────────────────────
        private static Sprite FindSprite(string name)
        {
            foreach (string guid in AssetDatabase.FindAssets($"t:Sprite {name}"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == name)
                    return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            Debug.LogWarning($"[UIRedesign] Sprite not found: {name}");
            return null;
        }

        private static void LoadSprites()
        {
            spriteRound12  = FindSprite("ui_card_frame_12px_neutral");
            spriteBtnRect  = FindSprite("UI_button_grey_standard");
            spriteGoldIcon = FindSprite("UI_icon_gold");
        }

        private static GameObject NewGO(string name, Transform parent, int sibling = -1)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(parent, false);
            if (sibling >= 0) go.transform.SetSiblingIndex(sibling);
            return go;
        }

        private static void SetRect(GameObject go, Vector2 ancMin, Vector2 ancMax,
            Vector2 piv, Vector2 aPos, Vector2 sDelta)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin        = ancMin;
            rt.anchorMax        = ancMax;
            rt.pivot            = piv;
            rt.anchoredPosition = aPos;
            rt.sizeDelta        = sDelta;
        }

        private static Image AddImage(GameObject go, Sprite sprite, Image.Type type,
            Color color, bool raycast = true)
        {
            var img = go.GetComponent<Image>() ?? go.AddComponent<Image>();
            img.sprite        = sprite;
            img.type          = type;
            img.color         = color;
            img.raycastTarget = raycast;
            return img;
        }

        private static TextMeshProUGUI AddTMP(GameObject go, string text, float size,
            FontStyles style, Color color, TextAlignmentOptions alignment)
        {
            var tmp = go.GetComponent<TextMeshProUGUI>() ?? go.AddComponent<TextMeshProUGUI>();
            tmp.text          = text;
            tmp.fontSize      = size;
            tmp.fontStyle     = style;
            tmp.color         = color;
            tmp.alignment     = alignment;
            tmp.raycastTarget = false;
            tmp.overflowMode  = TextOverflowModes.Ellipsis;
            return tmp;
        }

        private static LayoutElement EnsureLE(GameObject go,
            float flexW = -1, float flexH = -1, float prefW = -1, float prefH = -1)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            if (flexW >= 0) le.flexibleWidth  = flexW;
            if (flexH >= 0) le.flexibleHeight = flexH;
            if (prefW >= 0) le.preferredWidth  = prefW;
            if (prefH >= 0) le.preferredHeight = prefH;
            return le;
        }

        private static void RefreshButtonText(Transform btn, string text, float size, Color color)
        {
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.text      = text;
            tmp.fontSize  = size;
            tmp.color     = color;
            tmp.alignment = TextAlignmentOptions.Center;
        }

        private static void DestroyChild(Transform parent, string name)
        {
            var t = parent.Find(name);
            if (t != null) Object.DestroyImmediate(t.gameObject);
        }

        private static bool Validate(GameObject go, string name)
        {
            if (go == null) { Debug.LogError($"[UIRedesign] {name} not found."); return false; }
            return true;
        }

        private static Vector2 V2(float x, float y) => new Vector2(x, y);
    }
}
