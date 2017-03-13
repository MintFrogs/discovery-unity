package android.discovery.mintfrogs.com.discoveryapp;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;

import com.mintfrogs.discovery.android.Discovery;
import com.mintfrogs.discovery.android.Settings;

public class RootActivity extends AppCompatActivity {
  private Discovery mDiscovery;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.root_activity);

    mDiscovery = new Discovery(new Settings(), getApplicationContext());
  }

  @Override
  protected void onStart() {
    super.onStart();
    mDiscovery.start();
  }

  @Override
  protected void onStop() {
    Log.d(getClass().getSimpleName(), "stopping: " + mDiscovery.queryLastLocation());
    mDiscovery.stop();
    super.onStop();
  }
}
