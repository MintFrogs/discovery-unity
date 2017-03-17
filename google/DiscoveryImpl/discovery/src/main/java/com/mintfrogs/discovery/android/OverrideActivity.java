package com.mintfrogs.discovery.android;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.support.annotation.NonNull;
import android.util.Log;

import com.google.android.gms.location.LocationSettingsStates;
import com.unity3d.player.UnityPlayerActivity;

import static com.unity3d.player.UnityPlayer.UnitySendMessage;

public class OverrideActivity extends UnityPlayerActivity {
  @Override
  public void onRequestPermissionsResult(int request, @NonNull String permissions[], @NonNull int[] results) {
    if (0 == request) {
      // FIXME: In-house compatibility with generic permissions plugin
      if (results.length > 0 && results[0] == PackageManager.PERMISSION_GRANTED) {
        InnerUnitySendMessage("MfAndroidPermissionsOverride", "OnAllow", "");
      } else {
        InnerUnitySendMessage("MfAndroidPermissionsOverride", "OnDeny", "");
      }
    } else if (Discovery.PERMISSIONS_REQUEST == request) {
      if (results.length > 0 && results[0] == PackageManager.PERMISSION_GRANTED) {
        InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, "true");
      } else {
        InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, "false");
      }
    }
  }

  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
    super.onActivityResult(requestCode, resultCode, intent);
    Log.i(Discovery.TAG, "onActivityResult: " + requestCode + "/" + resultCode + "/" + intent);

    final LocationSettingsStates locationStates = LocationSettingsStates.fromIntent(intent);
    Log.i(Discovery.TAG, "onActivityResult-locationStates: " + locationStates);

    if (requestCode == Discovery.RESOLUTION_REQUEST) {
      if (RESULT_OK == resultCode) {
        InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, "true");
      } else if (RESULT_CANCELED == resultCode) {
        InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, "false");
      }
    }
  }

  public static void InnerUnitySendMessage(String object, String method, String args) {
    if (Discovery.isUnityEnv()) {
      UnitySendMessage(object, method, args);
    } else {
      Log.i(OverrideActivity.class.getSimpleName(), "InnerUnitySendMessage->" + object + "/" + method + "/" + args);
    }
  }
}
