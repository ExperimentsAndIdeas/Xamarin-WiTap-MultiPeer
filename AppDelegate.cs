using Foundation;
using UIKit;

using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace multipeeriOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, INSNetServiceDelegate, INSStreamDelegate
	{
		// The Bonjour service type consists of an IANA service name (see RFC 6335) 
		// prefixed by an underscore (as per RFC 2782).
		//
		// <http://www.ietf.org/rfc/rfc6335.txt>
		// 
		// <http://www.ietf.org/rfc/rfc2782.txt>
		// 
		// See Section 5.1 of RFC 6335 for the specifics requirements.
		// 
		// To avoid conflicts, you must register your service type with IANA before 
		// shipping.
		// 
		// To help network administrators indentify your service, you should choose a 
		// service name that's reasonably human readable.

		static NSString kWiTapBonjourType = (NSString)@"_witap2._tcp.";

		NSNetService server = null;
		bool isServerStarted = false;
		NSInputStream inputStream = null;
		NSOutputStream outputStream = null;
		NSString registeredName = null;
		int streamOpenCount = 0;

		PickerViewController picker = null;
		TapViewController tapViewController = null;

		myNetServiceDelegate MyNetServiceDelegate = null;

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Get the root view controller (set up by the storyboard)
			this.tapViewController = (TapViewController)this.Window.RootViewController;
			if (this.tapViewController.GetType() == typeof(TapViewController))
			{
				// throw
			}
			this.tapViewController.delesdff = null;      //  self.tapViewController.delegate = self;

			MyNetServiceDelegate = new myNetServiceDelegate();

			// Show our window
			this.Window.RootViewController = this.tapViewController;
			this.Window.MakeKeyAndVisible();

			// Create and advertise our server.  We only want the service to be registered on 
			// local networks so we pass in the "local." domain.
			this.server = new NSNetService(domain: "local.", type: kWiTapBonjourType, name: UIDevice.CurrentDevice.Name, port: 0);
			this.server.IncludesPeerToPeer = true;
			this.server.Delegate = this;  // should this be a weak delegage? [self.server setDelegate:self];
			this.server.Publish(NSNetServiceOptions.ListenForConnections);
			this.isServerStarted = true;

			// Set up for a new game, which presents a Bonjour browser that displays other 
			// available games.

			SetupForNewGame();

			return true;
		}

		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication uiApplication)
		{
			// If there's a game playing, shut it down.  Whether this is the right thing to do 
			// depends on your app.  In some cases it might be more sensible to leave the connection 
			// in place for a short while to see if the user comes back to the app.  This issue is 
			// discussed in more depth in Technote 2277 "Networking and Multitasking".
			//
			// <https://developer.apple.com/library/ios/#technotes/tn2277/_index.html>

			if (this.inputStream != null)
			{
				this.SetupForNewGame();
			}

			// Quiesce the server and service browser, if any.
			this.server.Stop();
			this.isServerStarted = false;
			this.registeredName = null;
			if (this.picker != null)
			{
				//[self.picker stop];
			}
		}

		public override void WillEnterForeground(UIApplication uiApplication)
		{
			// Quicken the server.  Once this is done it will quicken the picker, if there's one up.
			if (this.isServerStarted == false)
			{
				// todo: breakpoint
			}

			this.server.Publish(NSNetServiceOptions.ListenForConnections);
			this.isServerStarted = true;
			if (this.registeredName != null)
			{
				this.StartPicker();
			}
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}


		#region Picker Management
		void StartPicker()
		{
			if (this.registeredName == null)
			{
				// todo: throw
			}

			// Tell the picker about our registration.  It uses this to a) filter out our game 
			// from the results, and b) display our game name in its table view header.
			this.picker.localService = this.server;

			// Start it up.
			this.picker.Start();
		}


		void PresentPicker()
		{
			if (this.picker != null)
			{
				// If the picker is already on screen then we're here because of a connection failure. 
				// In that case we just cancel the picker's connection UI and the user can choose another 
				// service.

				this.picker.CancelConnect();
			}
			else
			{
				// Create the service picker and put it up on screen.  We only start the picker 
				// if our server has completed its registration (the picker needs to know our 
				// service name so that it can exclude us from the list).  If that's not the 
				// case then the picker remains stopped until -serverDidStart: runs.

				//self.picker = [self.tapViewController.storyboard instantiateViewControllerWithIdentifier: @"picker"];
				//assert([self.picker isKindOfClass:[PickerViewController class]]);
				//self.picker.type = kWiTapBonjourType;
				//self.picker.delegate = self;
				if (this.registeredName != null)
					StartPicker();

				//[self.tapViewController presentViewController:self.picker animated:NO completion:nil];

			}
		}

		void DismissPicker()
		{
			if (this.picker == null)
			{
				// throw todo:
			}

			//this.tapViewController.DismissViewControllerAnimated:NO completion:nil];';'
			this.picker.Stop();
			this.picker = null;
		}

		/// <summary>
		/// Called by the picker when the user has chosen a service for us to connect to. 
		/// The picker is already displaying its connection-in-progress UI.
		/// </summary>
		// - (void)pickerViewController:(PickerViewController *)controller connectToService:(NSNetService *)service
		void ConnectToService(PickerViewController controller, NSNetService service)
		{
			bool success;
			NSInputStream inStream;
			NSOutputStream outStream;

			if (service == null)
			{
				// todo: throw
			}
			if (this.inputStream != null)
			{
				// todo: throw
			}
			if (this.outputStream != null)
			{
				// todo: throw
			}

			if (controller == this.picker)   //assert(controller == self.picker);
			{
				// todo: throw
			}


			// Create and open streams for the service.
			// 
			// -getInputStream:outputStream: just creates the streams, it doesn't hit the 
			// network, and thus it shouldn't fail under normal circumstances (in fact, its 
			// CFNetService equivalent, CFStreamCreatePairWithSocketToNetService, returns no status 
			// at all).  So, I didn't spend too much time worrying about the error case here.  If 
			// we do get an error, you end up staying in the picker.  OTOH, actual connection errors 
			// get handled via the NSStreamEventErrorOccurred event.

			success = service.GetStreams(inputStream: out inStream, outputStream: out outStream);
			if (!success)
			{
				this.SetupForNewGame();
			}
			else
			{
				this.inputStream = inStream;
				this.outputStream = outStream;

				this.OpenStreams();
			}
		}

		/// <summary>
		///  Called by the picker when the user taps the Cancel button in its 
		/// connection-in-progress UI.  We respond by closing our in-progress connection.
		/// </summary>
		void PickerViewControllerDidCancellConnect()
		{
			this.CloseStreams();
		}

		#endregion
		void SetupForNewGame()
		{
			// Reset our tap view state to avoid old taps appearing in the new game.
			//[self.tapViewController resetTouches];

			// If there's a connection, shut it down.
			CloseStreams();

			// If our server is deregistered, reregister it.
			if (!this.isServerStarted)
			{
				this.server.Publish(); // [self.server publishWithOptions:NSNetServiceListenForConnections];
				this.isServerStarted = true;
			}

			// And show the service picker.
			// todo: [self presentPicker];
		}

		#region ConnectionManagement
		/*
		  * 
		  * 
 - (void)stream:(NSStream *)stream handleEvent:(NSStreamEvent)eventCode
 {
	 #pragma unused(stream)

	 switch(eventCode) {

		 case NSStreamEventOpenCompleted: {
			 self.streamOpenCount += 1;
			 assert(self.streamOpenCount <= 2);

			 // Once both streams are open we hide the picker and the game is on.

			 if (self.streamOpenCount == 2) {
				 [self dismissPicker];

				 [self.server stop];
				 self.isServerStarted = NO;
				 self.registeredName = nil;
			 }
		 } break;

		 case NSStreamEventHasSpaceAvailable: {
			 assert(stream == self.outputStream);
			 // do nothing
		 } break;

		 case NSStreamEventHasBytesAvailable: {
			 uint8_t     b;
			 NSInteger   bytesRead;

			 assert(stream == self.inputStream);

			 bytesRead = [self.inputStream read:&b maxLength:sizeof(uint8_t)];
			 if (bytesRead <= 0) {
				 // Do nothing; we'll handle EOF and error in the 
				 // NSStreamEventEndEncountered and NSStreamEventErrorOccurred case, 
				 // respectively.
			 } else {
				 // We received a remote tap update, forward it to the appropriate view
				 if ( (b >= 'A') && (b < ('A' + kTapViewControllerTapItemCount))) {
					 [self.tapViewController remoteTouchDownOnItem:b - 'A'];
				 } else if ( (b >= 'a') && (b < ('a' + kTapViewControllerTapItemCount))) {
					 [self.tapViewController remoteTouchUpOnItem:b - 'a'];
				 } else {
					 // Ignore the bogus input.  This is important because it allows us 
					 // to telnet in to the app in order to test its behaviour.  telnet 
					 // sends all sorts of odd characters, so ignoring them is a good thing.
				 }
			 }
		 } break;

		 default:
			 assert(NO);
			 // fall through
		 case NSStreamEventErrorOccurred:
			 // fall through
		 case NSStreamEventEndEncountered: {
			 [self setupForNewGame];
		 } break;
	 }
 }
		 */
		#endregion


		void OpenStreams()
		{
			if (this.inputStream == null)
			{
				// todo throw. // streams must exist but aren't open
			}
			if (this.outputStream == null)
			{

			}
			if (this.streamOpenCount != 0)
			{

			}
 


			this.inputStream.Delegate = this;  //[self.inputStream  setDelegate:self];
            this.inputStream.Schedule(aRunLoop: NSRunLoop.Current, mode: NSRunLoopMode.Default);// [self.inputStream  scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
			this.inputStream.Open();

			this.outputStream.Delegate = this;  //[self.outputStream  setDelegate:self];
			this.outputStream.Schedule(aRunLoop: NSRunLoop.Current , mode: NSRunLoopMode.Default);// [self.inputStream  scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
			this.outputStream.Open();
		}

		void CloseStreams()
		{
			if ((this.inputStream != null) != (this.outputStream != null))// should either have both or neither
			{
				// Throw!! todo:
			}

			if (this.inputStream != null)
			{
				//todo:   [self.inputStream removeFromRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
				this.inputStream.Close();
				this.inputStream = null;

				//todo:   [self.outputStream removeFromRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
				this.outputStream.Close();
				this.outputStream = null;
			}
			streamOpenCount = 0;
		}

	}
}



