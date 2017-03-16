package com.mintfrogs.discovery.android.app;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.widget.TextView;

import com.google.android.gms.location.LocationSettingsStates;
import com.mintfrogs.discovery.android.Discovery;
import com.mintfrogs.discovery.android.Settings;

import java.util.Arrays;

@SuppressLint("SetTextI18n")
public class RootActivity extends AppCompatActivity {
  private static final String TAG = RootActivity.class.getSimpleName();
  private Discovery mDiscovery;
  private TextView mOuputText;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.root_activity);

    mDiscovery = new Discovery(new Settings(), this);

    mOuputText = (TextView) findViewById(R.id.output_text);
    mOuputText.setText("Created...");
  }

  @Override
  protected void onStart() {
    super.onStart();
    mOuputText.setText("Starting...");
    startWithDiscovery();
  }

  @Override
  protected void onStop() {
    Log.d(getClass().getSimpleName(), "stopping: " + mDiscovery.queryLastLocation());

    if (mDiscovery.isStarted()) {
      mDiscovery.stop();
    }

    super.onStop();
  }

  private void startWithDiscovery() {
    mDiscovery.isLocationServicesEnabled(new Discovery.OnLocationEnabledListener() {
      @Override
      public void onLocationEnabledResult(boolean isEnabled) {
        mOuputText.setText("onLocationEnabledResult: " + isEnabled);

        if (isEnabled) {
          if (mDiscovery.hasLocationPermissions()) {
            mDiscovery.start();
          } else {
            mDiscovery.requestLocationPermissions();
          }
        } else {
          Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"false\")");
          mOuputText.setText("LocationResult: Unresolved");
        }
      }
    }, true);
  }

  @Override
  public void onRequestPermissionsResult(int request, @NonNull String permissions[], @NonNull int[] results) {
    if (Discovery.PERMISSIONS_REQUEST == request) {
      if (results.length > 0 && results[0] == PackageManager.PERMISSION_GRANTED) {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, \"true\")");
        startWithDiscovery();
      } else {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, \"false\")");
        mOuputText.setText("PermissionsResult[Declined]: " + Arrays.toString(results));
      }
    }
  }

  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
    final LocationSettingsStates locationStates = LocationSettingsStates.fromIntent(intent);

    if (requestCode == Discovery.PERMISSIONS_REQUEST) {
      if (RESULT_OK == resultCode && locationStates.isLocationPresent()) {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"true\")");
        startWithDiscovery();
      } else if (RESULT_CANCELED == resultCode) {
        Log.i(TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"false\")");
        mOuputText.setText("LocationResult[Declined]: " + resultCode);
      }
    }
  }
}
