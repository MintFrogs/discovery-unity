//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import "SXDiscoverySettings.h"

static const NSTimeInterval kDefaultInterval = 1500;

static const NSUInteger kInnerAccuracyNavigation = 1;
static const NSUInteger kInnerAccuracyBest = 2;
static const NSUInteger kInnerAccuracyTen = 3;
static const NSUInteger kInnerAccuracyHundred = 4;
static const NSUInteger kInnerAccuracyKilometer = 5;
static const NSUInteger kInnerAccuracyThreeKilometers = 6;

@implementation SXDiscoverySettings
+ (SXDiscoverySettings *)discoverySettingsFromString:(NSString *)aString {
  SXDiscoverySettings *settings = [[SXDiscoverySettings alloc] init];
  settings.updateInterval = kDefaultInterval;
  settings.accuracy = kCLLocationAccuracyBest;
  
  if (nil == aString || 0 == aString.length) {
    return settings;
  }
  
  NSArray<NSString *>* parts = [aString componentsSeparatedByString:@";"];
  NSUInteger interval = parts[0].integerValue;
  NSUInteger accuracy = parts[1].integerValue;
  
  settings.updateInterval = interval;
  
  if (kInnerAccuracyNavigation == accuracy) {
    settings.accuracy = kCLLocationAccuracyBestForNavigation;
  } else if (kInnerAccuracyBest == accuracy) {
    settings.accuracy = kCLLocationAccuracyBest;
  } else if (kInnerAccuracyTen == accuracy) {
    settings.accuracy = kCLLocationAccuracyNearestTenMeters;
  } else if (kInnerAccuracyHundred == accuracy) {
    settings.accuracy = kCLLocationAccuracyHundredMeters;
  } else if (kInnerAccuracyKilometer == accuracy) {
    settings.accuracy = kCLLocationAccuracyKilometer;
  } else if (kInnerAccuracyThreeKilometers == accuracy) {
    settings.accuracy = kCLLocationAccuracyThreeKilometers;
  } else {
    settings.accuracy = kCLLocationAccuracyBest;
  }
  
  return settings;
}
@end
