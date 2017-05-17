using Foundation;
using System;
using UIKit;
// QuartzCore;

namespace multipeeriOS
{
    public partial class TapView : UIView
    {
        static float kActivationInset = 10.0f;

		bool localTouch; // observable
		bool remoteTouch; // observable

		public TapView (IntPtr handle) : base (handle)
        {
        }

        void CommonInit()
        {
            if (this.MultipleTouchEnabled == false)
            {
                // throw
            }
            this.Layer.BorderColor = UIColor.DarkGray.CGColor;

            // Observe ourself to learn about someone changing the remoteTouch property.
            //this.AddObserver(this,keyPath: @"remoteTouch", options: 0, context: this._remoteTouch); //[self addObserver:self forKeyPath: options:0 context:&self->_remoteTouch];*/
        }

		//		@protocol TapViewDelegate;

		//		@interface TapView : UIView
 

		//@property(nonatomic, weak, readwrite) IBOutlet id<TapViewDelegate> delegate;
 

		/// <summary>
		/// cleans localTouch and remoteTouch, triggering a redraw and KVO, but 
		/// not calling the delegate
		/// </summary>
		internal void resetTouches()
        {
            this.LocalTouchUpNotify (false);  

            if(this.remoteTouch)
            {
                this.remoteTouch = false; 
            }
        }

        void LocalTouchDown()
        {
            if (!this.localTouch)
            {
				id<TapViewDelegate> strongDelegate;

                this.localTouch = true;

                this.UpdateBorderLayer();

//        strongDelegate = self.delegate;
//        if ([strongDelegate respondsToSelector:@selector(tapViewLocalTouchDown:)]) {
//            [strongDelegate tapViewLocalTouchDown:self];
//        }
//}
            }
        }

        void LocalTouchUpNotify(bool notify)
        {
            if (this.localTouch)
            {
                this.localTouch = false;

                this.UpdateBorderLayer();

                if (notify)
                {
					//id<TapViewDelegate> strongDelegate;

					strongDelegate = this.xdelegate;
                    if (true) //strongDelegate.RespondsToSelector: @selector(tapViewLocalTouchUp:)])
                    {
                        //[strongDelegate tapViewLocalTouchUp:self];
                    }
                }
            }
        }

        void TouchesBegan(NSSet touches, UIEvent evnt)
        {
//#pragma unused(touches)
//#pragma unused(event)
			this.LocalTouchDown();
        }

        void TouchesEnded(NSSet touches, UIEvent evnt)
        {
            //#pragma unused(touches)
            //#pragma unused(event)
            this.LocalTouchUpNotify(true);
        }
  
        void TouchesCancelled(NSSet touches, UIEvent evnt)
        {
			//#pragma unused(touches)
			//#pragma unused(event)
			this.LocalTouchUpNotify(true);
        }


		//		@protocol TapViewDelegate<NSObject>

		//@optional

		//- (void) tapViewLocalTouchDown:(TapView*) tapView;
		//- (void) tapViewLocalTouchUp:(TapView*) tapView;

		void Dealloc()
        {
            this.RemoveObserver(this, keyPath: @"remoteTouch", options: 0, context: this._remoteTouch);  //[self removeObserver:self forKeyPath:@"remoteTouch" context:&self->_remoteTouch];
        }

        void UpdateBorderLayer()
        {
            this.Layer.BorderWidth = (this.localTouch || this.remoteTouch) ? kActivationInset: 0.0f; // self.layer.borderWidth = (self.localTouch || self.remoteTouch) ? kActivationInset : 0.0f;
		}
		
 
        void ObserveValueForKeyPath(string keyPath)//   - (void) observeValueForKeyPath:(NSString*) keyPath ofObject:(id) object change:(NSDictionary*) change context:(void*) context
		{
            if (context == this.remoteTouch )
            {
				// If the remoteTouch property changes, redraw.
                this.UpdateBorderLayer();
			}
            else
            {
				//  [super observeValueForKeyPath:keyPath ofObject:object change:change context:context];
				base.ObserveValue(keyPath: null, ofObject: null, change: null, context: null);
            }
        }


	}
}