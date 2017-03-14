//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "SXDiscoverySettings.h"

@interface SXDiscoveryWarpper : NSObject
+ (instancetype)sharedInstance;

- (void)initialize:(SXDiscoverySettings *)settings;
- (void)start;
- (void)stop;
- (BOOL)isStarted;
@end
