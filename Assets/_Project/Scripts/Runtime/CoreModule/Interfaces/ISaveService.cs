namespace GoldenSpin.CoreModule
{
    public interface ISaveService
    {
        bool HasKey(string key);

        int LoadInt(string key, int defaultValue);

        void SaveInt(string key, int value);

        void DeleteKey(string key);
    }
}
