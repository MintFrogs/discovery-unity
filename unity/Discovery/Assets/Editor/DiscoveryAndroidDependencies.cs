using System.Collections.Generic;
using UnityEditor;

[InitializeOnLoad]
public class DiscoveryAndroidDependencies : AssetPostprocessor
{
#if UNITY_ANDROID
  public static object svcSupport;
#endif

  static DiscoveryAndroidDependencies()
  {
    RegisterDependencies();
  }

  public static void RegisterDependencies()
  {
#if UNITY_ANDROID
    RegisterAndroidDependencies();
#endif
  }

  public static void RegisterAndroidDependencies()
  {
    var playServicesSupport = Google.VersionHandler.FindClass(
      "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");

    if (playServicesSupport == null)
    {
      return;
    }

    svcSupport = svcSupport ?? Google.VersionHandler.InvokeStaticMethod(
      playServicesSupport, "CreateInstance",
      new object[]
      {
        "GooglePlayGames",
        EditorPrefs.GetString("AndroidSdkRoot"),
        "ProjectSettings"
      });

    Google.VersionHandler.InvokeInstanceMethod(
      svcSupport, "DependOn",
      new object[]
      {
        "com.google.android.gms",
        "play-services-location",
        "10.2.0"
      },
      new Dictionary<string, object>
      {
        {"packageIds", new[] {"extra-google-m2repository"}}
      });

    Google.VersionHandler.InvokeInstanceMethod(
      svcSupport, "DependOn",
      new object[] {"com.android.support", "support-v4", "25.2.+"},
      new Dictionary<string, object>
      {
        {"packageIds", new[] {"extra-android-m2repository"}}
      });
  }
}
