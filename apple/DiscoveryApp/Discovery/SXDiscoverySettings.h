//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

@interface SXDiscoverySettings : NSObject
@property (nonatomic, assign) NSTimeInterval updateInterval;
@property (nonatomic, assign) CLLocationAccuracy accuracy;

+ (SXDiscoverySettings *)discoverySettingsFromString:(NSString *)aString;
@end
