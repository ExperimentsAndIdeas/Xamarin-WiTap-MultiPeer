// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace multipeeriOS
{
    [Register ("PickerViewController")]
    partial class PickerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel localServiceNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (localServiceNameLabel != null) {
                localServiceNameLabel.Dispose ();
                localServiceNameLabel = null;
            }
        }
    }
}