//  Created by Sergey Ivonchik on 3/14/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import "SXDiscovery.h"

void SXDiscoveryInitialize(const char *settings) {
  NSString *settingsString = [NSString stringWithUTF8String:settings];
  SXDiscoverySettings *settingsObject = [SXDiscoverySettings discoverySettingsFromString:settingsString];
  NSLog(@"Discovery::init %@", [settingsString description]);
  [[SXDiscoveryWarpper sharedInstance] initialize:settingsObject];
}

void SXDiscoveryStart() {
  [[SXDiscoveryWarpper sharedInstance] start];
}

void SXDiscoveryStop() {
  [[SXDiscoveryWarpper sharedInstance] stop];
}

bool SXDiscoveryIsStarted() {
  return [[SXDiscoveryWarpper sharedInstance] isStarted];
}

bool SXDisciveryIsLocationEnabled() {
  return [[SXDiscoveryWarpper sharedInstance] isLocationServicesEnabled];
}
