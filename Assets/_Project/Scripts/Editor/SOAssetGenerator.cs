using UnityEngine;
using UnityEditor;
using GoldenSpin.CoreModule;
using GoldenSpin.WheelModule;
using GoldenSpin.ZoneModule;

namespace GoldenSpin.Editor
{
    public static class SOAssetGenerator
    {
        private const string DATA_PATH = "Assets/_Project/AddressableAssets/Data";

        private const string CASH_LABEL    = "Cash";
        private const string WEAPON_LABEL  = "Weapon";
        private const string AMMO_LABEL    = "Ammo";
        private const string GOLD_LABEL    = "Gold";
        private const string SPECIAL_LABEL = "Special";

        private const int CASH_BASE_AMOUNT    = 100;
        private const int WEAPON_BASE_AMOUNT  = 1;
        private const int AMMO_BASE_AMOUNT    = 50;
        private const int GOLD_BASE_AMOUNT    = 200;
        private const int SPECIAL_BASE_AMOUNT = 500;

        private const string CASH_SPRITE_KEY    = "cash_icon";
        private const string WEAPON_SPRITE_KEY  = "weapon_icon";
        private const string AMMO_SPRITE_KEY    = "ammo_icon";
        private const string GOLD_SPRITE_KEY    = "gold_icon";
        private const string SPECIAL_SPRITE_KEY = "special_icon";

        [MenuItem("GoldenSpin/Generate SO Assets")]
        public static void GenerateAll()
        {
            RewardSO cash    = CreateSO<RewardSO>("Cash_RewardSO",    r =>
            {
                r.Type       = RewardType.Cash;
                r.Label      = CASH_LABEL;
                r.BaseAmount = CASH_BASE_AMOUNT;
                r.SpriteKey  = CASH_SPRITE_KEY;
            });

            RewardSO weapon  = CreateSO<RewardSO>("Weapon_RewardSO",  r =>
            {
                r.Type       = RewardType.Weapon;
                r.Label      = WEAPON_LABEL;
                r.BaseAmount = WEAPON_BASE_AMOUNT;
                r.SpriteKey  = WEAPON_SPRITE_KEY;
            });

            RewardSO ammo    = CreateSO<RewardSO>("Ammo_RewardSO",    r =>
            {
                r.Type       = RewardType.Ammo;
                r.Label      = AMMO_LABEL;
                r.BaseAmount = AMMO_BASE_AMOUNT;
                r.SpriteKey  = AMMO_SPRITE_KEY;
            });

            RewardSO gold    = CreateSO<RewardSO>("Gold_RewardSO",    r =>
            {
                r.Type       = RewardType.Gold;
                r.Label      = GOLD_LABEL;
                r.BaseAmount = GOLD_BASE_AMOUNT;
                r.SpriteKey  = GOLD_SPRITE_KEY;
            });

            RewardSO special = CreateSO<RewardSO>("Special_RewardSO", r =>
            {
                r.Type       = RewardType.Special;
                r.Label      = SPECIAL_LABEL;
                r.BaseAmount = SPECIAL_BASE_AMOUNT;
                r.SpriteKey  = SPECIAL_SPRITE_KEY;
            });

            WheelSliceSO normalSlice0 = CreateWheelSlice("NormalSlice_Cash",    SliceContentType.Reward, cash);
            WheelSliceSO normalSlice1 = CreateWheelSlice("NormalSlice_Weapon",  SliceContentType.Reward, weapon);
            WheelSliceSO normalSlice2 = CreateWheelSlice("NormalSlice_Ammo",    SliceContentType.Reward, ammo);
            WheelSliceSO normalSlice3 = CreateWheelSlice("NormalSlice_Gold",    SliceContentType.Reward, gold);
            WheelSliceSO normalSlice4 = CreateWheelSlice("NormalSlice_Cash2",   SliceContentType.Reward, cash);
            WheelSliceSO normalSlice5 = CreateWheelSlice("NormalSlice_Ammo2",   SliceContentType.Reward, ammo);
            WheelSliceSO normalSlice6 = CreateWheelSlice("NormalSlice_Weapon2", SliceContentType.Reward, weapon);
            WheelSliceSO normalBomb   = CreateWheelSlice("NormalSlice_Bomb",    SliceContentType.Bomb,   null);

            WheelConfigSO normalConfig = CreateSO<WheelConfigSO>("NormalWheel_WheelConfigSO", c =>
            {
                c.SpinType = SpinType.Normal;
                c.Slices   = new System.Collections.Generic.List<WheelSliceSO>
                {
                    normalSlice0, normalSlice1, normalSlice2, normalSlice3,
                    normalSlice4, normalSlice5, normalSlice6, normalBomb
                };
            });

            WheelSliceSO silverSlice0 = CreateWheelSlice("SilverSlice_Cash",    SliceContentType.Reward, cash);
            WheelSliceSO silverSlice1 = CreateWheelSlice("SilverSlice_Gold",    SliceContentType.Reward, gold);
            WheelSliceSO silverSlice2 = CreateWheelSlice("SilverSlice_Weapon",  SliceContentType.Reward, weapon);
            WheelSliceSO silverSlice3 = CreateWheelSlice("SilverSlice_Ammo",    SliceContentType.Reward, ammo);
            WheelSliceSO silverSlice4 = CreateWheelSlice("SilverSlice_Cash2",   SliceContentType.Reward, cash);
            WheelSliceSO silverSlice5 = CreateWheelSlice("SilverSlice_Gold2",   SliceContentType.Reward, gold);
            WheelSliceSO silverSlice6 = CreateWheelSlice("SilverSlice_Ammo2",   SliceContentType.Reward, ammo);
            WheelSliceSO silverSlice7 = CreateWheelSlice("SilverSlice_Weapon2", SliceContentType.Reward, weapon);

            WheelConfigSO silverConfig = CreateSO<WheelConfigSO>("SilverWheel_WheelConfigSO", c =>
            {
                c.SpinType = SpinType.Silver;
                c.Slices   = new System.Collections.Generic.List<WheelSliceSO>
                {
                    silverSlice0, silverSlice1, silverSlice2, silverSlice3,
                    silverSlice4, silverSlice5, silverSlice6, silverSlice7
                };
            });

            WheelSliceSO goldenSlice0 = CreateWheelSlice("GoldenSlice_Special",  SliceContentType.Reward, special);
            WheelSliceSO goldenSlice1 = CreateWheelSlice("GoldenSlice_Gold",     SliceContentType.Reward, gold);
            WheelSliceSO goldenSlice2 = CreateWheelSlice("GoldenSlice_Special2", SliceContentType.Reward, special);
            WheelSliceSO goldenSlice3 = CreateWheelSlice("GoldenSlice_Weapon",   SliceContentType.Reward, weapon);
            WheelSliceSO goldenSlice4 = CreateWheelSlice("GoldenSlice_Special3", SliceContentType.Reward, special);
            WheelSliceSO goldenSlice5 = CreateWheelSlice("GoldenSlice_Gold2",    SliceContentType.Reward, gold);
            WheelSliceSO goldenSlice6 = CreateWheelSlice("GoldenSlice_Special4", SliceContentType.Reward, special);
            WheelSliceSO goldenSlice7 = CreateWheelSlice("GoldenSlice_Cash",     SliceContentType.Reward, cash);

            WheelConfigSO goldenConfig = CreateSO<WheelConfigSO>("GoldenWheel_WheelConfigSO", c =>
            {
                c.SpinType = SpinType.Golden;
                c.Slices   = new System.Collections.Generic.List<WheelSliceSO>
                {
                    goldenSlice0, goldenSlice1, goldenSlice2, goldenSlice3,
                    goldenSlice4, goldenSlice5, goldenSlice6, goldenSlice7
                };
            });

            CreateSO<ZoneConfigSO>("ZoneConfigSO", z =>
            {
                z.NormalWheelConfig = normalConfig;
                z.SilverWheelConfig = silverConfig;
                z.GoldenWheelConfig = goldenConfig;
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SOAssetGenerator] All SO assets generated successfully.");
        }

        private static WheelSliceSO CreateWheelSlice(string name, SliceContentType contentType, RewardSO reward)
        {
            return CreateSO<WheelSliceSO>(name, s =>
            {
                s.ContentType = contentType;
                s.Reward      = reward;
            });
        }

        private static T CreateSO<T>(string fileName, System.Action<T> init) where T : ScriptableObject
        {
            string path     = $"{DATA_PATH}/{fileName}.asset";
            T existing      = AssetDatabase.LoadAssetAtPath<T>(path);

            if (existing != null)
            {
                init(existing);
                EditorUtility.SetDirty(existing);
                return existing;
            }

            T so = ScriptableObject.CreateInstance<T>();
            init(so);
            AssetDatabase.CreateAsset(so, path);
            return so;
        }
    }
}
