// MonoMac port of Indragie Karunaratne's awesome INAppStoreWindow Objective-C library.
//
// Copyright Ashok Gelal 2014. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

using AppKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
    public partial class WindowButton : NSButton
    {
        #region Fields

        private NSTrackingArea _mouseTrackingArea;

        #endregion

        #region Properties

        public string GroupIdentifier
        {
            get;
            private set;
        }

        public NSImage ActiveImage
        {
            get;
            set;
        }

        public NSImage ActiveNotKeyWindowImage
        {
            get;
            set;
        }

        public NSImage InactiveImage
        {
            get;
            set;
        }

        public NSImage RolloverImage
        {
            get;
            set;
        }

        public NSImage PressedImage
        {
            get
            {
                return AlternateImage;
            }
            set
            {
                AlternateImage = value;
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                Image = value ? ActiveImage : InactiveImage;
            }
        }

        #endregion

        #region Constructors

        // Called when created from unmanaged code
        public WindowButton(IntPtr handle) : base(handle)
        {
            Initialize();
        }
        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public WindowButton(NSCoder coder) : base(coder)
        {
            Initialize();
        }

		public WindowButton(CGSize size, string groupIdentifier) : base(new CGRect(0, 0, size.Width, size.Height))
        {
            GroupIdentifier = groupIdentifier;
            Initialize();
        }

		[Export("initWithFrame:")]
		public WindowButton (CGRect frame) : base(frame)
		{
			Initialize ();
		}

        #endregion

        #region Methods

        private void Initialize()
        {
            SetButtonType(NSButtonType.MomentaryChange);
            Bordered = false;
            //Title = string.Empty;

			Cell.HighlightsBy = (int)NSCellStyleMask.ContentsCell;
            Cell.ImageDimsWhenDisabled = false;
        }

        public override void UpdateTrackingAreas()
        {
            base.UpdateTrackingAreas();
            if (_mouseTrackingArea != null)
            {
                RemoveTrackingArea(_mouseTrackingArea);
            }

            _mouseTrackingArea = new NSTrackingArea(new CGRect(-4, -4, Bounds.Width, Bounds.Height), 
                NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveAlways,
                this, null
            );

            AddTrackingArea(_mouseTrackingArea);
        }

        public override void ViewDidMoveToWindow()
        {
            if (Window != null)
            {
                UpdateImage();
            }
        }

        public override void ViewWillMoveToWindow(NSWindow newWindow)
        {
            var nc = NSNotificationCenter.DefaultCenter;
            if (Window != null)
            {
                nc.RemoveObserver(this, NSWindow.DidBecomeKeyNotification, Window);
                nc.RemoveObserver(this, NSWindow.DidResignKeyNotification, Window);
                nc.RemoveObserver(this, NSWindow.DidMiniaturizeNotification, Window);
                nc.RemoveObserver(this, NSWindow.WillEnterFullScreenNotification, Window);
                nc.RemoveObserver(this, NSWindow.WillExitFullScreenNotification, Window);
            }

            if (newWindow != null)
            {
                nc.AddObserver(NSWindow.DidBecomeKeyNotification, WindowDidChangeFocusNotificationHandler, newWindow);
                nc.AddObserver(NSWindow.DidResignKeyNotification, WindowDidChangeFocusNotificationHandler, newWindow);

            }
        }

        private void WindowDidChangeFocusNotificationHandler(NSNotification n)
        {
            UpdateImage();
        }
			
        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
            //Group.DidCaptureMousePointer();
            UpdateRollOverImage();
        }

        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            //Group.DidReleaseMousePointer();
            UpdateRollOverImage();
        }

        protected void UpdateRollOverImage()
        {
            if (Enabled)
            {
                Image = RolloverImage;
            }
            else
            {
                UpdateImage();
            }
        }

        protected void UpdateImage()
        {
            if (Window.IsKeyWindow)
            {
                UpdateActiveImage();
            }
            else
            {
                Image = ActiveNotKeyWindowImage;
            }
        }

        protected void UpdateActiveImage()
        {
            Image = Enabled ? ActiveImage : InactiveImage;
        }

        #endregion
    }
}

