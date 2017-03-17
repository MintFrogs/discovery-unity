//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "SXDiscoveryWarpper.h"

FOUNDATION_EXPORT double DiscoveryVersionNumber;
FOUNDATION_EXPORT const unsigned char DiscoveryVersionString[];
FOUNDATION_EXPORT void UnitySendMessage(const char *, const char *, const char *);

#ifdef __cplusplus
extern "C" {
#endif
  
  void SXDiscoveryInitialize(const char *sttings);
  void SXDiscoveryStart();
  void SXDiscoveryStop();
  bool SXDiscoveryIsStarted();
  bool SXDisciveryIsLocationEnabled();
  
#ifdef __cplusplus
}
#endif
