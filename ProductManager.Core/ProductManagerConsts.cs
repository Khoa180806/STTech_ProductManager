using ProductManager.Debugging;

namespace ProductManager;

public class ProductManagerConsts
{
    public const string LocalizationSourceName = "ProductManager";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "263fdfcf1189441fb2074575f7a699c4";
}
