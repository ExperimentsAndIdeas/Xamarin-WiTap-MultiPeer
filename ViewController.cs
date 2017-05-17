using System;

using UIKit;

namespace multipeeriOS
{
    public class myNetServiceDelegate : Foundation.NSNetServiceDelegate
    {
        
    }

	public class sdaf : Foundation.NSStreamDelegate
	{

	}
	//public class sdaf : PickerDelegate //TapViewControllerDelegate

	//public class sdaf :  //TapViewControllerDelegate


	public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
