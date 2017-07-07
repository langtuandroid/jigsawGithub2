//
//  AlternativeInputUnity.c
//  Unity-iPhone
//
//  Created by Raimundas Sakalauskas on 15/09/15.
//
//

#import <objc/runtime.h>
#include <stdio.h>
#include <stdlib.h>
#include "AlternativeInputUnity.h"
#include "UnityView.h"


@implementation AlternativeInput

//load is called once per class
+ (void) load
{
    [self replaceMethods];
    
}

//replaces Touch methods in UnityView.mm, also adds an implementation of method traitCollectionDidChange
//(if user navigates to accessibility settings and changes ForceTouch settings the callback method in Unity will be called).
+ (void) replaceMethods
{
    SEL touchBegin = @selector(touchesBegan:withEvent:);
    Method orig = class_getInstanceMethod([UnityView class], touchBegin);
    Method replacement = class_getInstanceMethod([self class], touchBegin);
    method_exchangeImplementations(orig, replacement);
    
    SEL touchEnd = @selector(touchesEnded:withEvent:);
    Method orig1 = class_getInstanceMethod([UnityView class], touchEnd);
    Method replacement1 = class_getInstanceMethod([self class], touchEnd);
    method_exchangeImplementations(orig1, replacement1);
    
    SEL touchCancel = @selector(touchesCancelled:withEvent:);
    Method orig2 = class_getInstanceMethod([UnityView class], touchCancel);
    Method replacement2 = class_getInstanceMethod([self class], touchCancel);
    method_exchangeImplementations(orig2, replacement2);
    
    SEL touchMove = @selector(touchesMoved:withEvent:);
    Method orig3 = class_getInstanceMethod([UnityView class], touchMove);
    Method replacement3 = class_getInstanceMethod([self class], touchMove);
    method_exchangeImplementations(orig3, replacement3);

    if(NSClassFromString(@"UITraitCollection")) //only available in iOS8
        class_addMethod([UnityView class], @selector(traitCollectionDidChange:), (IMP)TraitCollectionDidChange, [[NSString stringWithFormat:@"@:%s", @encode(UITraitCollection)] UTF8String]);
}

//replacement methods
- (void)touchesBegan:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesBegin(touches, event);
    [[AlternativeInput instance] addTouches:touches];
}
- (void)touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesEnded(touches, event);
    [[AlternativeInput instance] removeTouches:touches];
}
- (void)touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesCancelled(touches, event);
    [[AlternativeInput instance] removeTouches:touches];
}
- (void)touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesMoved(touches, event);
    [[AlternativeInput instance] updateTouches:touches];
}

static void TraitCollectionDidChange(id self, SEL _cmd, UITraitCollection *previousTraitCollection)
{
    if ([[self traitCollection] respondsToSelector:@selector(forceTouchCapability)])
    {
        if ((previousTraitCollection.forceTouchCapability != [self traitCollection].forceTouchCapability) && [[AlternativeInput instance] callbackGameObject] && [[AlternativeInput instance] callbackMethod])
        {
            
            UnitySendMessage(cStringCopy([[[AlternativeInput instance] callbackGameObject] UTF8String]), cStringCopy([[[AlternativeInput instance] callbackMethod] UTF8String]), cStringCopy([[NSString stringWithFormat:@"%d",[[AlternativeInput instance] getForceTouchState]] UTF8String]));
        }
    }
}
//end replacement methods


//lazy instantiate
+ (id) instance
{
    static AlternativeInput *instance = nil;
    @synchronized(self) {
        if (instance == nil)
            instance = [[self alloc] init];
    }
    return instance;
}

@synthesize touches = _touches;
@synthesize trackingEnabled = _trackingEnabled;
@synthesize callbackMethod = _callbackMethod;
@synthesize callbackGameObject = _callbackGameObject;
@synthesize unityScreenSize = _unityScreenSize;

//lazy instantiate
- (NSMutableDictionary *)touches
{
    if (!_touches) _touches = [[NSMutableDictionary alloc] init];
    return _touches;
}

- (void) addTouches:(NSSet*)touches
{
    if (!self.trackingEnabled)
        return;
    
    for (UITouch *touch in touches)
    {
        NSString *touchId = [self guidForTouch:touch];
        if (!touchId)
            [self.touches setObject:touch forKey:[self newGuid]];
    }
}

- (void) updateTouches:(NSSet*)touches
{
    if (!self.trackingEnabled)
        return;
    
    for (UITouch *touch in touches)
    {
        NSString *touchId = [self guidForTouch:touch];
        if (!touchId)
            [self.touches setObject:touch forKey:[self newGuid]];
    }
}

- (void) removeTouches:(NSSet*)touches
{
    for (UITouch *touch in touches)
    {
        NSString *touchId = [self guidForTouch:touch];
        if (touchId)
            [self.touches removeObjectForKey:touchId];
        else
            NSLog(@"Touch Not Found!");
    }
}

- (int)getForceTouchState
{
    UIView *unityView = UnityGetGLView();
    if (NSClassFromString(@"UITraitCollection")) //Only available in iOS 8
    {
        if ([unityView.traitCollection respondsToSelector:@selector(forceTouchCapability)])
        {
            if (unityView.traitCollection.forceTouchCapability == UIForceTouchCapabilityAvailable)
                return 1;
            else if (unityView.traitCollection.forceTouchCapability == UIForceTouchCapabilityUnavailable)
                return 2;
            else if (unityView.traitCollection.forceTouchCapability == UIForceTouchCapabilityUnknown)
                return 3;
            else
                return 4;
        }
        else
        {
            return 4;
        }
    }
    else
    {
        return 4;
    }
}

- (void) setCallbackMethod:(NSString *)method forGameObject:(NSString *)gameObject
{
    //This is problematic.
    //String object received by calling [NSString stringWithUTF8String:] uses reference to const char* passed by managed code.
    //At the end of the method call from managed code, unity releases all the variables passed to native code.
    //See the problem here?:) Basically if we simply assing reference to strings passed as arguments, by the
    //time we need them they will be released (Only in 4.6, in 5.0+ it was fixed).
    //Objective-c compiler is optimized therefore [NSString stringWithString:] and [NSString copy] don't do deep
    //copy; resulting objects address same memory address. Therefore mutable copy is the only way to make deep copy
    //and retain the memory from being released by Unity engine.

    _callbackMethod = [method mutableCopy];
    _callbackGameObject = [gameObject mutableCopy];
}

- (void) removeCallbackMethod
{
    _callbackMethod = nil;
    _callbackGameObject = nil;
}


- (NSString *) newGuid
{
    NSString *temp = [NSString stringWithFormat:@"%i", arc4random_uniform(UINT16_MAX)];
    while ([self.touches objectForKey:temp])
    {
        temp = [NSString stringWithFormat:@"%i", arc4random_uniform(UINT16_MAX)];
    }
    
    return temp;
}

- (NSString *) guidForTouch:(UITouch *)touch
{
    for (NSString* key in [self.touches allKeys])
    {
        if ([self.touches objectForKey:key] == touch)
            return key;
    }
    return nil;
}
@end

//interface for communication with unity managed code
extern "C"
{
    unsigned const TOUCH_BINARY_SIZE = 36;
    
    //@"Id"
    //@"X"
    //@"Y"
    //@"DeltaX"
    //@"DeltaY"
    //@"Force"
    //@"MaxForce"
    //@"Radius"
    //@"RadiusTolerance"
    int getNativeTouches(long *dataPtr)
    {
        NSMutableDictionary *touches = [[AlternativeInput instance] touches];
        CGRect bounds = [UnityGetGLView() bounds];
        float inputScale = getScaleFactor([[AlternativeInput instance] unityScreenSize]);
        
        int dataLength = (int)[touches count] * TOUCH_BINARY_SIZE;
        if (dataLength > 0)
        {
            unsigned char* data = (unsigned char*)malloc(dataLength);
            int touchIdx = 0;
            
            for (NSString* key in [touches allKeys])
            {
                UITouch *touch = [touches objectForKey:key];
                CGPoint pt = [touch locationInView:UnityGetGLView()];
                CGPoint lastpt =  [touch previousLocationInView:UnityGetGLView()];
                
                //determine force
                float force = 1.0;
                float maxForce = -1.0;
                if ([touch respondsToSelector:@selector(maximumPossibleForce)])
                {
                    if (touch.maximumPossibleForce > 0)
                    {
                        force = touch.force;
                        maxForce = touch.maximumPossibleForce;
                    }
                }
                
                float radius = -1.0;
                float radiusTolerance = 0;
                if ([touch respondsToSelector:@selector(majorRadius)])
                {
                    radius = touch.majorRadius;
                    radiusTolerance = touch.majorRadiusTolerance;
                }
                
                
                WriteInt32(data + (touchIdx * TOUCH_BINARY_SIZE), [key intValue]);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 4, pt.x * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 8, (bounds.size.height - pt.y) * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 12, (pt.x - lastpt.x) * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 16, (lastpt.y - pt.y) * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 20, force);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 24, maxForce);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 28, radius);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 32, radiusTolerance);
                
                touchIdx++;
            }
            
            *dataPtr = (long)data;
            return dataLength;
        }
        else
        {
            return 0;
        }
    }
    
    void startTracking(float unityScreenSize)
    {
        [[AlternativeInput instance] setUnityScreenSize:unityScreenSize];
        [[AlternativeInput instance] setTrackingEnabled:YES];
    }
    
    void stopTracking()
    {
        [[AlternativeInput instance] setTrackingEnabled:NO];
    }
    
    int getForceTouchState()
    {
        return [[AlternativeInput instance] getForceTouchState];
    }
    
    float getScaleFactor(float unityScreenSize)
    {
        CGRect bounds = [UnityGetGLView() bounds];
        float iOSSize = ((bounds.size.height > bounds.size.width) ? bounds.size.height : bounds.size.width);
        return unityScreenSize / iOSSize;
    }
    
    void setCallbackMethod(const char* gameObject, const char* methodName)
    {
        [[AlternativeInput instance] setCallbackMethod:CreateNSString(methodName) forGameObject:CreateNSString(gameObject)];
    }
    
    void removeCallbackMethod()
    {
        [[AlternativeInput instance] removeCallbackMethod];
    }
    
    bool supportsTouchRadius()
    {
        return [[UITouch class] instancesRespondToSelector:@selector(majorRadius)];
    }
}



//helper methods
void	WriteInt32(unsigned char* data, int32_t value)
{
    char* ref = (char*) &value;
    memcpy(data, ref, sizeof(int32_t));
}

void	WriteInt16(unsigned char* data, int16_t value)
{
    char* ref = (char*) &value;
    memcpy(data, ref, sizeof(int16_t));
}

void	WriteFloat(unsigned char* data, float_t value)
{
    char* ref = (char*) &value;
    memcpy(data, ref, sizeof(float_t));
}

char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

NSString* CreateNSString(const char* string)
{
    if (string != NULL)
        return [NSString stringWithUTF8String:string];
    else
        return [NSString stringWithUTF8String:""];
}