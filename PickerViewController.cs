using Foundation;
using System;
using UIKit;
//using QuartzCore;
using CoreGraphics;

namespace multipeeriOS
{
    public partial class PickerViewController : UITableViewController
    {
        
        /* Properties you must set before starting and must not change while started */
        //@property(nonatomic, copy, readwrite) NSString* type;

        /* Properties you mustn't set while started */
         //PickerDelegate pickerDelegate;

        /* Properties you can set at anytime */
        /// <summary>
        /// Setting this shows the service name to the user at the top of the picker 
        /// and prevents that service from showing up in the list.
        /// </summary>
       internal NSNetService localService;


        /*DELEGATE PROPERTIES*/
        //        - (void) pickerViewController:(PickerViewController*) controller connectToService:(NSNetService*) service;
        //    // Called when the user selects a service to connect to.  The picker has already put up 
        //    // the connection-in-progress UI.  The delegate should start the connection and, when it's 
        //    // finished, either dismiss the picker (if the connection was a success) or call 
        //    // -cancelConnection (if something went wrong).

        //- (void) pickerViewControllerDidCancelConnect:(PickerViewController*) controller;
        //// Called when the picker itself wants to cancel the connection.  It does this when the user 
        //// taps the Cancel button in the connection-in-progress UI.  At the time this is called the 
        //// connection-in-progress UI has been dismissed; there's no need to call -cancelConnect.



        UIFont localServiceNameLabelFont;      // latched from localServiceNameLabel

        //UILabel connectView = null;
        //UILabel connectLabel = null;

        NSString connectLabelTemplate;  // latched from connectLabel

        NSMutableArray services;  // of NSNetService, sorted by name
        NSNetServiceBrowser browser = null;

        /*
        - (instancetype)init
{
    self = [super init];
    if (self != nil) {
        [self commonInit];
    }
    return self;
}

- (instancetype)initWithCoder:(NSCoder *)coder
{
    self = [super initWithCoder:coder];
    if (self) {
        [self commonInit];
    }
    return self;
}

- (void)commonInit
{
    self->_services = [[NSMutableArray alloc] init];
    // We observe localService so that we can react to the client changing it.
    [self addObserver:self forKeyPath:@"localService" options:0 context:&self->_localService];
}

- (void)dealloc
{
    [self removeObserver:self forKeyPath:@"localService" context:&self->_localService];
}*/

        public PickerViewController(IntPtr handle) : base(handle)
        {
            //this.RegisterForPreviewingWithDelegate();
            //this.TransitioningDelegate
            //this.WeakTransitioningDelegate
            //this.TableView.Delegate
        }

        /// <summary>
        /// Called to set the local service name label in response to a change in the local service. 
        /// It sets the label to either the actual name or "registering" if there's no local service.
        /// </summary>
        void setupLocalServiceNameLabel()
        {
            if (this.localServiceNameLabel == null)
            {
                this.localServiceNameLabel.Font = UIFont.ItalicSystemFontOfSize(this.localServiceNameLabelFont.PointSize * 0.75f);
                this.localServiceNameLabel.Text = "registering...";
            }
            else
            {
                this.localServiceNameLabel.Font = this.localServiceNameLabelFont;
                this.localServiceNameLabel.Text = this.localService.Name; //self.localServiceNameLabel.text = self.localService.name;
            }

        }

        void ObserverValueForKeyPath(string keypath, NSDictionary change, object context)
        //        - (void) observeValueForKeyPath:(NSString*) keyPath ofObject:(id) object change:(NSDictionary*) change context:(void*) context
        {
            if (context == this.localService) //  if (context == &self->_localService)
            {
                if (keypath != @"localService")
                {
                    // todo: throw exception
                }

                if (xobject != this) //      assert(object == self);
                {
                    // todo: throw exception
                }

                // If there's a local service name label (that is, -viewDidLoad has been called), updated it.
                if (this.localServiceNameLabel != null)
                {
                    this.setupLocalServiceNameLabel();
                }

                // There's a chance that the browser saw our service before we heard about its successful 
                // registration, at which point we need to hide the service.  Doing that would be easy, 
                // but there are other edge cases to consider (for example, if the local service changes 
                // name, we would have to unhide the old name and hide the new name).  Rather than attempt 
                // to handle all of those edge cases we just stop and restart when the service name changes.
                if (this.browser != null)
                {
                    this.Stop();
                    this.Start();
                }
            }
            else
            {
               // base.ObserveValue(keypath, object, );//   [super observeValueForKeyPath:keyPath ofObject:object change:change context:context];
            }
        }

        public override void ViewDidLoad()
        {
            //assert(self.localServiceNameLabel != nil);
            //assert(self.connectView != nil);
            //assert(self.connectLabel != nil);

            // Stash the original font for use by -setupLocalServiceNameLabel then call 
            // -setupLocalServiceNameLabel to apply the local service to our header.

            this.localServiceNameLabelFont = this.localServiceNameLabel.Font;

            this.setupLocalServiceNameLabel();

            // Set up the connect view and stash the label text for use as a template.
            this.connectView.Layer.CornerRadius = 10.0f;
            this.connectView.Layer.ShadowColor = UIColor.Black.CGColor;
            this.connectView.Layer.ShadowOffset = new CGSize(3.0f, 3.0f);
            this.connectView.Layer.Opacity = 0.7f;

            this.connectLabelTemplate = this.connectLabel.Text;
        }

        /// <summary>
        ///  Called to start the picker.  It's not legal to start it when it's already started.
        /// </summary>
        internal void Start()
        {
            //assert([self.services count] == 0);

            //assert(self.browser == nil);

            this.browser = new NSNetServiceBrowser();// [[NSNetServiceBrowser alloc] init];
            this.browser.IncludesPeerToPeer = true;

            this.browser.Delegate =this;//[self.browser setDelegate:self];
            this.browser.SearchForServices(type: this, domain: "local"); //  [self.browser searchForServicesOfType:self.type inDomain:@"local"];
        }

        /// <summary>
        /// Called to stop the picker.  It's OK to stop a picker that's not started.
        /// </summary>
        internal void Stop()
        {
            this.browser.Stop();
            this.browser = null;

            this.services.RemoveAllObjects();

            if (this.IsViewLoaded)
            {
                this.TableView.ReloadData();
            }
        }

        /// <summary>
        ///  Call this to tell the picker that you've cancelled a connection attempt.  In 
        /// response it dismisses the connection-in-progress UI.  See the discussion associated 
        ///  with the PickerDelegate protocol .
        /// </summary>
        internal void CancelConnect()
        {
            this.HideConnectViewAndNotify(false);
        }

        void ShowConnectViewForService(NSNetService service)
        {
            CGRect selfViewBounds;

            // Show the connection UI.

            //		assert(self.connectView != nil);               // views should be loaded
            //		assert(self.connectLabel != nil);               // ditto
            //		assert(self.connectView.superview == nil);      // connection view must not be in the view hierarchy

            this.connectLabel.Text = this.connectLabelTemplate + service.Name; // todo fix string with formatting

            selfViewBounds = this.TableView.Bounds;
            this.connectView.Center = new CGPoint(); //self.connectView.center = CGPointMake(CGRectGetMidX(selfViewBounds), CGRectGetMidY(selfViewBounds));

            this.TableView.AddSubview(this.connectView);

            // Disable user interactions on the table view to prevent the user doing 
            // stuff 'behind' our connection-in-progress UI.

            this.TableView.ScrollEnabled = false;
            this.TableView.AllowsSelection = false;

            // Tell the delegate.
            //[self.delegate pickerViewController:self connectToService:service]; 
        }

        /// <summary>
        /// Hide the view we showed in -showConnectViewForService:
        /// </summary>
        /// <param name="notify">If set to <c>true</c> notify.</param>
        void HideConnectViewAndNotify(bool notify)
        {
            if (this.connectView.Superview != null)
            {
                this.connectView.RemoveFromSuperview();

                this.TableView.ScrollEnabled = true;
                this.TableView.AllowsSelection = true;
            }

            if (notify)
            {
                this.delegateX.pickerViewControllerDidCancelConnect.self; // [self.delegate pickerViewControllerDidCancelConnect:self];
            }

        }


        //        - (IBAction) connectCancelAction:(id) sender
        //		// Called when the user taps the Cancel button in the connection UI.  This hides the 
        //		// connection UI and tells the delegate about the cancellation.
        //		{
        //#pragma unused(sender)
        //	[self hideConnectViewAndNotify:YES];
        //}

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            //#pragma unused(tableView)
            //#pragma unused(section)
            return (nint)this.services.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            UITableViewCell cell;
            NSNetService service;

            //#pragma unused(tableView)

            service = this.services[indexPath.Row]; //service = [self.services objectAtIndex: (NSUInteger)indexPath.row];

            cell = this.TableView.DequeueReusableCell("cell");

            cell.TextLabel.Text = service.Name;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            NSNetService service;

            //#pragma unused(tableView)
            //#pragma unused(indexPath)

            this.TableView.DeselectRow(indexPath, animated: true);

            // Find the service associated with the cell and start a connection to that.
            service = this.services[(int)indexPath.Row];
            this.ShowConnectViewForService(service);
        }

        void SortAndReloadTable()
        {
            // Sort the services by name.
            //this.services.Sort()   // [self.services sortUsingComparator:^NSComparisonResult(id obj1, id obj2) {
            //      return [[obj1 name]
            //localizedCaseInsensitiveCompare:[obj2 name]];
            //}];

            // Reload if the view is loaded.
            if (this.IsViewLoaded)
            {
                this.TableView.ReloadData();
            }
        }

        NSNetServiceBrowser DidRemoveService(NSNetService service, bool morecoming)
        {
            //assert(browser == self.browser);
            //#pragma unused(browser)
            //assert(service != nil);

            // Remove the service from our array (assume it's there, of course).
            if (this.localService == null || !this.localService.IsEqual(service))  // todo: test, may not be correct port of  ->> if ((self.localService == nil) || ! [self.localService isEqual: service])
                this.services.RemoveObject(service); //[self.services removeObject:service];


            // Only update the UI once we get the no-more-coming indication.
            if (!morecoming)
                this.SortAndReloadTable();
        }


        NSNetServiceBrowser DidFindService(NSNetService service, bool morecoming)
        {
            //assert(browser == self.browser);
            //#pragma unused(browser)
            //assert(service != nil);

            // Add the service to our array (unless its our own service).

            if ((this.localService == null) || !this.localService.IsEqual(service))
                this.services.AddObject(service);  // [self.services addObject:service];

            // Only update the UI once we get the no-more-coming indication.
            if (!morecoming)
                this.SortAndReloadTable();

        }

        NSNetServiceBrowser DidNotSearch(NSDictionary errorDict)
        {
            if (browser != this.browser) // todo: fix. Original code is assert(browser == self.browser);
            {

            }
            if (errorDict == null)
            {

            }

            //#pragma unused(browser)
            //#pragma unused(errorDict)
            throw new Exception();  // The usual reason for us not searching is a programming error.
        }
    }
}