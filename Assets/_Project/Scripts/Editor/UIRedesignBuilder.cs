using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

namespace GoldenSpin.Editor
{
    /// <summary>
    /// Builds the full UI redesign layout in the active scene.
    /// Run via menu: GoldenSpin → Build UI Redesign
    /// </summary>
    public static class UIRedesignBuilder
    {
        // ─── Colors ────────────────────────────────────────────────
        private static readonly Color ColPanelBg       = new Color(0.055f, 0.055f, 0.08f,  0.97f);
        private static readonly Color ColPanelBorder   = new Color(1f,    1f,    1f,    0.06f);
        private static readonly Color ColBadgeBg       = new Color(1f,    1f,    1f,    0.05f);
        private static readonly Color ColCurrencyBg    = new Color(1f,    0.84f, 0f,    0.09f);
        private static readonly Color ColCurrencyText  = new Color(1f,    0.84f, 0f,    1f);
        private static readonly Color ColSectionTitle  = new Color(0.38f, 0.38f, 0.38f, 1f);
        private static readonly Color ColZoneText      = new Color(0.72f, 0.72f, 0.72f, 1f);
        private static readonly Color ColDotNormal     = new Color(0.72f, 0.39f, 0.10f, 1f);
        private static readonly Color ColBtnSpin       = new Color(0.07f, 0.15f, 0.27f, 1f);
        private static readonly Color ColBtnSpinText   = new Color(0.42f, 0.69f, 1f,   1f);
        private static readonly Color ColBtnCollectOff = new Color(0.06f, 0.06f, 0.06f, 1f);
        private static readonly Color ColBtnCollectOn  = new Color(0.07f, 0.22f, 0.07f, 1f);
        private static readonly Color ColBtnCollectTxt = new Color(0.25f, 0.25f, 0.25f, 1f);
        private static readonly Color ColBtnGiveUp     = new Color(0.06f, 0.06f, 0.06f, 1f);
        private static readonly Color ColBtnRevive     = new Color(0.07f, 0.15f, 0.27f, 1f);
        private static readonly Color ColBtnPaid       = new Color(0.42f, 0.26f, 0.00f, 1f);
        private static readonly Color ColBtnPaidText   = new Color(1f,    0.90f, 0.35f, 1f);
        private static readonly Color ColCountBadge    = new Color(1f,    0.84f, 0f,    0.1f);

        // ─── Sprites (loaded at build time) ───────────────────────
        private static Sprite spriteCardFrame;
        private static Sprite spriteCardFrame12;
        private static Sprite spriteBtnGrey;
        private static Sprite spriteBtnOrange;

        // ─── Entry Point ──────────────────────────────────────────
        [MenuItem("GoldenSpin/Build UI Redesign")]
        public static void BuildUIRedesign()
        {
            LoadSprites();

            var canvas = GameObject.Find("Canvas");
            if (canvas == null) { Debug.LogError("[UIRedesign] Canvas not found."); return; }

            var panelWheel   = canvas.transform.Find("Panel_Wheel")?.gameObject;
            var panelReward  = canvas.transform.Find("Panel_RewardList")?.gameObject;
            var panelZoneBar = canvas.transform.Find("Panel_ZoneBar")?.gameObject;
            var panelBomb    = canvas.transform.Find("Panel_Bomb")?.gameObject;

            if (!Validate(panelWheel,   "Panel_Wheel"))   return;
            if (!Validate(panelReward,  "Panel_RewardList")) return;
            if (!Validate(panelZoneBar, "Panel_ZoneBar")) return;
            if (!Validate(panelBomb,    "Panel_Bomb"))    return;

            Undo.RegisterFullObjectHierarchyUndo(canvas, "GoldenSpin UI Redesign");

            BuildTopBar(panelWheel.transform);
            BuildActionRow(panelWheel.transform);
            RedesignRewardPanel(panelReward.transform);
            RedesignZoneBarPanel(panelZoneBar.transform);
            RedesignBombPopup(panelBomb.transform);

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[UIRedesign] Done! Save the scene to persist changes.");
        }

        // ─── TOP BAR ──────────────────────────────────────────────
        // Anchored to top of Panel_Wheel. Height 100px.
        private static void BuildTopBar(Transform panelWheel)
        {
            DestroyChild(panelWheel, "ui_panel_top_bar");

            var root = NewGO("ui_panel_top_bar", panelWheel, sibling: 0);
            SetRect(root, ancMin: V2(0, 1), ancMax: V2(1, 1), piv: V2(0.5f, 1),
                          aPos: V2(0, 0), sDelta: V2(0, 100));
            AddImage(root, spriteCardFrame, Image.Type.Sliced, new Color(0.04f, 0.04f, 0.06f, 0.95f), raycast: false);

            // ── Zone Badge ──────────────────────────────────────────
            var badge = NewGO("ui_container_zone_badge", root.transform);
            SetRect(badge, ancMin: V2(0, 0.5f), ancMax: V2(0, 0.5f), piv: V2(0, 0.5f),
                           aPos: V2(16, 0), sDelta: V2(218, 52));
            AddImage(badge, spriteCardFrame12, Image.Type.Sliced, ColBadgeBg, raycast: false);

            var dot = NewGO("ui_image_zone_dot", badge.transform);
            SetRect(dot, ancMin: V2(0, 0.5f), ancMax: V2(0, 0.5f), piv: V2(0, 0.5f),
                         aPos: V2(12, 0), sDelta: V2(22, 22));
            var dotImg = AddImage(dot, spriteCardFrame12, Image.Type.Sliced, ColDotNormal, raycast: false);

            var zoneTxt = NewGO("ui_text_zone_label_value", badge.transform);
            SetRect(zoneTxt, ancMin: V2(0, 0), ancMax: V2(1, 1), piv: V2(0.5f, 0.5f),
                             aPos: V2(22, 0), sDelta: V2(-28, 0));
            AddTMP(zoneTxt, "Zone 1", 30, FontStyles.Bold, ColZoneText, TextAlignmentOptions.Left);

            // ── Currency Pill ────────────────────────────────────────
            var pill = NewGO("ui_container_currency_pill", root.transform);
            SetRect(pill, ancMin: V2(1, 0.5f), ancMax: V2(1, 0.5f), piv: V2(1, 0.5f),
                          aPos: V2(-16, 0), sDelta: V2(152, 52));
            AddImage(pill, spriteCardFrame12, Image.Type.Sliced, ColCurrencyBg, raycast: false);

            var gemIcon = NewGO("ui_image_currency_icon", pill.transform);
            SetRect(gemIcon, ancMin: V2(0, 0), ancMax: V2(0, 1), piv: V2(0, 0.5f),
                             aPos: V2(8, 0), sDelta: V2(38, -10));
            AddTMP(gemIcon, "💎", 32, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);

            var currencyAmt = NewGO("ui_text_currency_amount_value", pill.transform);
            SetRect(currencyAmt, ancMin: V2(0, 0), ancMax: V2(1, 1), piv: V2(0.5f, 0.5f),
                                 aPos: V2(22, 0), sDelta: V2(-16, -8));
            AddTMP(currencyAmt, "25", 36, FontStyles.Bold, ColCurrencyText, TextAlignmentOptions.Center);
        }

        // ─── ACTION ROW ───────────────────────────────────────────
        // Anchored above Panel_RewardList (y=600), height 135px.
        private static void BuildActionRow(Transform panelWheel)
        {
            DestroyChild(panelWheel, "ui_action_row");

            // Move existing spin button out temporarily
            var existingSpinBtn = panelWheel.Find("ui_button_spin");

            var row = NewGO("ui_action_row", panelWheel);
            SetRect(row, ancMin: V2(0, 0), ancMax: V2(1, 0), piv: V2(0.5f, 0),
                         aPos: V2(0, 605), sDelta: V2(-32, 130));

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 12;
            hlg.padding = new RectOffset(0, 0, 0, 0);
            hlg.childAlignment        = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            // COLLECT button (flex 1)
            BuildCollectButton(row.transform);

            // Reparent or rebuild SPIN button (flex 1.4)
            if (existingSpinBtn != null)
            {
                existingSpinBtn.SetParent(row.transform, false);
                var spinRT = existingSpinBtn.GetComponent<RectTransform>();
                spinRT.anchorMin = V2(0, 0);
                spinRT.anchorMax = V2(1, 1);
                spinRT.offsetMin = spinRT.offsetMax = Vector2.zero;

                var spinImg = existingSpinBtn.GetComponent<Image>();
                if (spinImg != null) spinImg.color = ColBtnSpin;

                EnsureLE(existingSpinBtn.gameObject, flexW: 1.4f);
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
            var img = AddImage(btn, spriteBtnGrey, Image.Type.Sliced, ColBtnCollectOff);
            var button = btn.AddComponent<Button>();
            button.targetGraphic = img;
            var cb = button.colors;
            cb.normalColor      = ColBtnCollectOff;
            cb.highlightedColor = new Color(0.10f, 0.30f, 0.10f);
            cb.pressedColor     = new Color(0.05f, 0.18f, 0.05f);
            cb.disabledColor    = new Color(0.04f, 0.04f, 0.04f);
            button.colors = cb;
            EnsureLE(btn, flexW: 1f);

            var vlg = btn.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment       = TextAnchor.MiddleCenter;
            vlg.spacing              = 4;
            vlg.padding              = new RectOffset(8, 8, 10, 10);
            vlg.childForceExpandWidth  = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth    = true;
            vlg.childControlHeight   = true;

            var label = NewGO("ui_text_collect_label", btn.transform);
            var lTmp = AddTMP(label, "🔒  COLLECT", 28, FontStyles.Bold, ColBtnCollectTxt, TextAlignmentOptions.Center);
            EnsureLE(label, prefH: 36f);

            var sub = NewGO("ui_text_collect_sub", btn.transform);
            AddTMP(sub, "Safe zones only", 21, FontStyles.Normal,
                   new Color(0.25f, 0.25f, 0.25f, 0.65f), TextAlignmentOptions.Center);
            EnsureLE(sub, prefH: 26f);
        }

        private static void BuildSpinButton(Transform parent)
        {
            var btn = NewGO("ui_button_spin", parent);
            var img = AddImage(btn, spriteBtnGrey, Image.Type.Sliced, ColBtnSpin);
            var button = btn.AddComponent<Button>();
            button.targetGraphic = img;
            EnsureLE(btn, flexW: 1.4f);
            StyleSpinButton(btn);
        }

        private static void StyleSpinButton(GameObject btn)
        {
            // Remove old texts if any, add fresh label
            var existingLabel = btn.transform.Find("ui_text_spin_label");
            if (existingLabel == null)
            {
                var label = NewGO("ui_text_spin_label", btn.transform);
                SetRect(label, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(0, 0));
                AddTMP(label, "▶  SPIN", 34, FontStyles.Bold, ColBtnSpinText, TextAlignmentOptions.Center);
            }
        }

        // ─── REWARD PANEL ─────────────────────────────────────────
        private static void RedesignRewardPanel(Transform panelReward)
        {
            // Restyle existing bg
            var existBg = panelReward.Find("ui_image_panel_bg");
            if (existBg == null)
            {
                var bgGO = NewGO("ui_image_panel_bg", panelReward, sibling: 0);
                SetRect(bgGO, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-16, -8));
                AddImage(bgGO, spriteCardFrame, Image.Type.Sliced, ColPanelBg, raycast: false);
            }
            else
            {
                var bgImg = existBg.GetComponent<Image>();
                if (bgImg) { bgImg.sprite = spriteCardFrame; bgImg.type = Image.Type.Sliced; bgImg.color = ColPanelBg; }
            }

            // Remove old title
            DestroyChild(panelReward, "ui_text_title");
            DestroyChild(panelReward, "ui_section_header");

            // Build section header
            var header = NewGO("ui_section_header", panelReward, sibling: 1);
            SetRect(header, V2(0, 1), V2(1, 1), V2(0.5f, 1), V2(0, -10), V2(-32, 56));

            var hlg = header.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment        = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            var title = NewGO("ui_text_section_title", header.transform);
            AddTMP(title, "COLLECTED REWARDS", 24, FontStyles.Bold, ColSectionTitle, TextAlignmentOptions.Left);
            EnsureLE(title, flexW: 1f);

            var countBadge = NewGO("ui_container_count_badge", header.transform);
            AddImage(countBadge, spriteCardFrame12, Image.Type.Sliced, ColCountBadge, raycast: false);
            EnsureLE(countBadge, prefW: 76f, prefH: 38f);

            var countTxt = NewGO("ui_text_reward_count_value", countBadge.transform);
            SetRect(countTxt, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-8, -6));
            AddTMP(countTxt, "0", 24, FontStyles.Bold, ColCurrencyText, TextAlignmentOptions.Center);

            // Push scroll view down
            var scrollView = panelReward.Find("Scroll View");
            if (scrollView != null)
            {
                var sRT = scrollView.GetComponent<RectTransform>();
                sRT.anchorMin = V2(0, 0);
                sRT.anchorMax = V2(1, 1);
                sRT.offsetMin = new Vector2(12, 10);
                sRT.offsetMax = new Vector2(-12, -74);
            }
        }

        // ─── ZONE BAR PANEL ───────────────────────────────────────
        private static void RedesignZoneBarPanel(Transform panelZoneBar)
        {
            // Restyle bg
            var existBg = panelZoneBar.Find("ui_image_zonebar_bg");
            if (existBg != null)
            {
                var img = existBg.GetComponent<Image>();
                if (img) { img.sprite = spriteCardFrame; img.type = Image.Type.Sliced; img.color = ColPanelBg; }
                SetRect(existBg.gameObject, V2(0, 0), V2(1, 1), V2(0.5f, 0.5f), V2(0, 0), V2(-16, -8));
            }

            DestroyChild(panelZoneBar, "ui_text_title");
            DestroyChild(panelZoneBar, "ui_section_header");

            var header = NewGO("ui_section_header", panelZoneBar, sibling: 1);
            SetRect(header, V2(0, 1), V2(1, 1), V2(0.5f, 1), V2(0, -8), V2(-32, 50));

            var hlg = header.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment        = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            var title = NewGO("ui_text_zone_bar_title", header.transform);
            AddTMP(title, "ZONE PROGRESS", 22, FontStyles.Bold, ColSectionTitle, TextAlignmentOptions.Left);
            EnsureLE(title, flexW: 1f);

            var cur = NewGO("ui_text_zone_current_value", header.transform);
            AddTMP(cur, "1 / 30", 22, FontStyles.Bold, ColCurrencyText, TextAlignmentOptions.Right);
            EnsureLE(cur, prefW: 110f);

            // Push zone items container down below header
            var container = panelZoneBar.Find("ui_container_zone_items");
            if (container != null)
            {
                var cRT = container.GetComponent<RectTransform>();
                cRT.anchorMin = V2(0, 0);
                cRT.anchorMax = V2(1, 1);
                cRT.offsetMin = new Vector2(12, 8);
                cRT.offsetMax = new Vector2(-12, -64);
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
            SetRect(row, V2(0, 0), V2(1, 0), V2(0.5f, 0), V2(0, 24), V2(-32, 110));

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing               = 10;
            hlg.childAlignment        = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth     = true;
            hlg.childControlHeight    = true;

            // Reparent & restyle Give Up
            if (btnGiveUp != null)
            {
                btnGiveUp.SetParent(row.transform, false);
                ResetLayoutChild(btnGiveUp.gameObject, flexW: 1f);
                var img = btnGiveUp.GetComponent<Image>();
                if (img) { img.sprite = spriteBtnGrey; img.color = ColBtnGiveUp; }
                RefreshButtonText(btnGiveUp, "🗑  GIVE UP", 26, ColSectionTitle);
            }

            // Reparent & restyle Revive (free)
            if (btnRevive != null)
            {
                btnRevive.SetParent(row.transform, false);
                ResetLayoutChild(btnRevive.gameObject, flexW: 1f);
                var img = btnRevive.GetComponent<Image>();
                if (img) { img.sprite = spriteBtnGrey; img.color = ColBtnRevive; }
                RefreshButtonText(btnRevive, "↩  REVIVE", 26, ColBtnSpinText);
            }

            // New paid revive button
            BuildRevivePaidButton(row.transform);
        }

        private static void BuildRevivePaidButton(Transform parent)
        {
            var btn = NewGO("ui_button_revive_paid", parent);
            var img = AddImage(btn, spriteBtnGrey, Image.Type.Sliced, ColBtnPaid);
            var button = btn.AddComponent<Button>();
            button.targetGraphic = img;
            var cb = button.colors;
            cb.normalColor      = ColBtnPaid;
            cb.highlightedColor = new Color(0.60f, 0.38f, 0f);
            cb.pressedColor     = new Color(0.28f, 0.16f, 0f);
            cb.disabledColor    = new Color(0.14f, 0.09f, 0f, 0.7f);
            button.colors = cb;
            EnsureLE(btn, flexW: 1.3f);

            var vlg = btn.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment        = TextAnchor.MiddleCenter;
            vlg.spacing               = 4;
            vlg.padding               = new RectOffset(6, 6, 8, 8);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth     = true;
            vlg.childControlHeight    = true;

            // Cost row: 💎 + amount
            var costRow = NewGO("ui_container_cost", btn.transform);
            var chlg = costRow.AddComponent<HorizontalLayoutGroup>();
            chlg.childAlignment        = TextAnchor.MiddleCenter;
            chlg.spacing               = 4;
            chlg.childForceExpandWidth = false;
            chlg.childForceExpandHeight = false;
            chlg.childControlWidth     = true;
            chlg.childControlHeight    = true;
            EnsureLE(costRow, prefH: 34f);

            var gemTxt = NewGO("ui_image_gem_icon", costRow.transform);
            AddTMP(gemTxt, "💎", 26, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);
            EnsureLE(gemTxt, prefW: 30f, prefH: 30f);

            var costAmt = NewGO("ui_text_revive_paid_cost_value", costRow.transform);
            AddTMP(costAmt, "25", 30, FontStyles.Bold, ColBtnPaidText, TextAlignmentOptions.Center);
            EnsureLE(costAmt, prefW: 48f, prefH: 34f);

            // KEEP REWARDS label
            var keepLabel = NewGO("ui_text_keep_rewards", btn.transform);
            AddTMP(keepLabel, "KEEP REWARDS", 20, FontStyles.Bold, ColBtnPaidText, TextAlignmentOptions.Center);
            EnsureLE(keepLabel, prefH: 26f);
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
            spriteCardFrame  = FindSprite("ui_card_frame_gardient");
            spriteCardFrame12 = FindSprite("ui_card_frame_12px_neutral");
            spriteBtnGrey    = FindSprite("UI_button_grey_standard");
            spriteBtnOrange  = FindSprite("UI_button_orange_standard");
        }

        private static GameObject NewGO(string name, Transform parent, int sibling = -1)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(parent, false);
            if (sibling >= 0) go.transform.SetSiblingIndex(sibling);
            return go;
        }

        private static void SetRect(GameObject go,
            Vector2 ancMin, Vector2 ancMax, Vector2 piv,
            Vector2 aPos, Vector2 sDelta)
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
            img.sprite         = sprite;
            img.type           = type;
            img.color          = color;
            img.raycastTarget  = raycast;
            return img;
        }

        private static TextMeshProUGUI AddTMP(GameObject go, string text, float size,
            FontStyles style, Color color, TextAlignmentOptions alignment)
        {
            var tmp = go.GetComponent<TextMeshProUGUI>() ?? go.AddComponent<TextMeshProUGUI>();
            tmp.text           = text;
            tmp.fontSize       = size;
            tmp.fontStyle      = style;
            tmp.color          = color;
            tmp.alignment      = alignment;
            tmp.raycastTarget  = false;
            tmp.overflowMode   = TextOverflowModes.Ellipsis;
            return tmp;
        }

        private static LayoutElement EnsureLE(GameObject go,
            float flexW = -1, float flexH = -1,
            float prefW = -1, float prefH = -1)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            if (flexW >= 0) le.flexibleWidth  = flexW;
            if (flexH >= 0) le.flexibleHeight = flexH;
            if (prefW >= 0) le.preferredWidth  = prefW;
            if (prefH >= 0) le.preferredHeight = prefH;
            return le;
        }

        private static void ResetLayoutChild(GameObject go, float flexW = 1f)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            EnsureLE(go, flexW: flexW);
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
