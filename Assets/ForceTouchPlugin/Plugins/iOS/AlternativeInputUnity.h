#import <Foundation/Foundation.h>

@interface AlternativeInput : NSObject
    + (id) instance;

    - (void) addTouches:(NSSet*)touches;
    - (void) updateTouches:(NSSet*)touches;
    - (void) removeTouches:(NSSet*)touches;

    - (int) getForceTouchState;
    - (void) setCallbackMethod:(NSString *)method forGameObject:(NSString *)gameObject;
    - (void) removeCallbackMethod;

    @property (strong, nonatomic) NSMutableDictionary *touches;
    @property (nonatomic, assign) BOOL trackingEnabled;
    @property (strong, nonatomic) NSMutableString *callbackMethod;
    @property (strong, nonatomic) NSMutableString *callbackGameObject;
    @property (nonatomic, assign) float unityScreenSize;
@end



#ifdef __cplusplus
extern "C" {
#endif
    
    void startTracking(float unityScreenSize);
    void stopTracking();
    int getForceTouchState();
    float getScaleFactor(float unityScreenSize);
    int getNativeTouches(long *dataPtr);
    void setCallbackMethod(const char* gameObject, const char* methodName);
    void removeCallbackMethod();
    bool supportsTouchRadius();
    
#ifdef __cplusplus
}
#endif
    
#ifdef __cplusplus
    void	WriteInt32(unsigned char* data, int32_t value);
    void	WriteInt16(unsigned char* data, int16_t value);
    void	WriteFloat(unsigned char* data, float_t value);
    char* cStringCopy(const char* string);
    static NSString* CreateNSString(const char* string);
#endif