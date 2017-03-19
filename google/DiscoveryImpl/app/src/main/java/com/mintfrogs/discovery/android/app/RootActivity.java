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
  private Discovery mDiscovery;
  private TextView mOutputText;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.root_activity);

    mDiscovery = new Discovery(new Settings(), this);

    mOutputText = (TextView) findViewById(R.id.output_text);
    mOutputText.setText("Created...");
  }

  @Override
  protected void onStart() {
    super.onStart();
    mOutputText.setText("Starting...");
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
    if (!mDiscovery.hasLocationPermissions()) {
      mOutputText.setText("hasPermissions: false/requesting");
      mDiscovery.requestLocationPermissions();
      return;
    }

    mDiscovery.isLocationServicesEnabled(new Discovery.OnLocationEnabledListener() {
      @Override
      public void onLocationEnabledResult(boolean isEnabled) {
        mOutputText.setText("onLocationEnabledResult: " + isEnabled);

        if (isEnabled) {
          mDiscovery.start();
        } else {
          Log.i(Discovery.TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"false\")");
          mOutputText.setText("LocationResult: Unresolved");
        }
      }
    }, true);
  }

  @Override
  public void onRequestPermissionsResult(int request, @NonNull String permissions[], @NonNull int[] results) {
    if (Discovery.PERMISSIONS_REQUEST == request) {
      if (results.length > 0 && results[0] == PackageManager.PERMISSION_GRANTED) {
        Log.i(Discovery.TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, \"true\")");
        startWithDiscovery();
      } else {
        Log.i(Discovery.TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_PERMISSIONS_CALLBACK, \"false\")");
        mOutputText.setText("PermissionsResult[Declined]: " + Arrays.toString(results));
      }
    }
  }

  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
    super.onActivityResult(requestCode, resultCode, intent);

    Log.i(Discovery.TAG, "onActivityResult: " + requestCode + "/" + resultCode + "/" + intent);
    mOutputText.setText("Result: " + requestCode);

    final LocationSettingsStates locationStates = LocationSettingsStates.fromIntent(intent);
    Log.i(Discovery.TAG, "onActivityResult-locationStates: " + locationStates);

    if (requestCode == Discovery.RESOLUTION_REQUEST) {
      if (RESULT_OK == resultCode) {
        Log.i(Discovery.TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"true\")");
        mOutputText.setText("LocationResult[Resolved]: " + resultCode);
        startWithDiscovery();
      } else if (RESULT_CANCELED == resultCode) {
        Log.i(Discovery.TAG, "InnerUnitySendMessage(Discovery.UNITY_OBJECT, Discovery.UNITY_RESOLUTION_CALLBACK, \"false\")");
        mOutputText.setText("LocationResult[Declined]: " + resultCode);
      }
    }
  }
}
