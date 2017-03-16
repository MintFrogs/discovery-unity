package android.discovery.mintfrogs.com.discoveryapp;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;

import com.google.android.gms.location.LocationSettingsStates;
import com.mintfrogs.discovery.android.Discovery;
import com.mintfrogs.discovery.android.Settings;

public class RootActivity extends AppCompatActivity {
  private static final String TAG = RootActivity.class.getSimpleName();
  private Discovery mDiscovery;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.root_activity);

    Log.d(TAG, "onCreate");
    mDiscovery = new Discovery(new Settings(), this);
  }

  @Override
  protected void onStart() {
    super.onStart();

    Log.d(TAG, "onStart");
    mDiscovery.isLocationServicesEnabled(new Discovery.OnLocationEnabledListener() {
      @Override
      public void onLocationEnabledResult(boolean isEnabled) {
        Log.d(TAG, "onLocationEnabledResult: " + isEnabled);

        if (isEnabled) {
          if (mDiscovery.hasLocationPermissions()) {
            mDiscovery.start();
          } else {
            mDiscovery.requestLocationPermissions();
          }
        } else {
          Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"false\")");
        }
      }
    }, true);
  }

  @Override
  protected void onStop() {
    Log.d(getClass().getSimpleName(), "stopping: " + mDiscovery.queryLastLocation());

    if (mDiscovery.isStarted()) {
      mDiscovery.stop();
    }

    super.onStop();
  }

  @Override
  public void onRequestPermissionsResult(int request, @NonNull String permissions[], @NonNull int[] results) {
    if (Discovery.PERMISSIONS_REQUEST == request) {
      if (results.length > 0 && results[0] == PackageManager.PERMISSION_GRANTED) {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, \"true\")");
        mDiscovery.start();
      } else {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, \"false\")");
      }
    }
  }

  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
    final LocationSettingsStates locationStates = LocationSettingsStates.fromIntent(intent);

    if (requestCode == Discovery.PERMISSIONS_REQUEST) {
      if (RESULT_OK == resultCode && locationStates.isLocationPresent()) {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"true\")");
      } else if (RESULT_CANCELED == resultCode) {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"false\")");
      }
    }
  }
}
