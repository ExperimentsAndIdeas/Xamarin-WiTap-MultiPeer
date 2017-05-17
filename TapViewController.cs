using Foundation;
using System;
using UIKit;

namespace multipeeriOS
{ 
    public partial class TapViewController : UIViewController
    {
		

        public TapViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            for (int tapViewTag = 1; tapViewTag < (kTapViewControllerTapItemCount + 1); tapViewTag++)
            {
				TapView tapView;
                tapView = (TapView)this.View(tapViewTag: tapViewTag)l;//.view viewWithTag: tapViewTag];

				//assert([tapView isKindOfClass:[TapView class]]);

                tapView.BackgroundColor = UIColor.FromHSBA(
                    hue: (tapViewTag / kTapViewControllerTapItemCount),
                    saturation: 0.75f,
                brightness: 0.75f,
				alpha: 1.0f 
                ); 
			}
        }

        // The view controller mains kTapViewControllerTapItemCount tap views, 
        // each with a local and a remote state.  The local state is whether 
        // the user is currently tapping on the view; the view controller 
        // tells clients about this via delegate callbacks.  The remote state 
        // is whether a remote user is currently tapping on the view; the client 
        // must tell the view controller about this via the various 'remote touch' 
        // methods below.

        //enum {
        int kTapViewControllerTapItemCount = 9;
		//}

        internal void TapViewLocalTouchDown(int tapItemIndex)
        {
			//id<TapViewControllerDelegate> strongDelegate;

			strongDelegate = this.delegate;
			if ([strongDelegate.RespondsToSelector: @selector(tapViewController: localTouchDownOnItem:)])
			{
				//assert(tapView.tag != 0);
				//assert(tapView.tag <= kTapViewControllerTapItemCount);

	        	[strongDelegate tapViewController:self localTouchDownOnItem:(NSUInteger) ([tapView tag] - 1)];
             }
        }


		internal void remoteTouchDownOnItem(int tapItemIndex)
		{
			//assert(tapItemIndex < kTapViewControllerTapItemCount);
            if (this.IsViewLoaded )
			{
                ((TapView)this.View.ViewWithTag(tapItemIndex +1).RemoteTouch = true;
			}
		}
		internal void remoteTouchUpOnItem(int tapItemIndex)
		{
			//assert(tapItemIndex < kTapViewControllerTapItemCount);
			if (this.IsViewLoaded)
			{
                ((TapView)this.View.ViewWithTag(tapItemIndex + 1).RemoteTouch = false;
			}
		}
		internal void resetTouches()
		{
			for (int tag = 1; tag <= kTapViewControllerTapItemCount; tag++)
			{
				TapView tapView;

                tapView = (TapView)this.View.ViewWithTag(tag);// .view viewWithTag: tag]);
                  //assert([tapView isKindOfClass:[TapView class]]);
                tapView.resetTouches(); 
            }
		}
        internal IBAction CloseButtonAction(var sender)
        {
			//id <TapViewControllerDelegate>  strongDelegate;

//#pragma unused(sender)
			//strongDelegate = this.delegate;
            //if (strongDelegate.RespondsToSelector(selector(tapViewControllerDidClose:)))
            //{
            //    [strongDelegate tapViewControllerDidClose:self];
                
            //}
        }


	}
}