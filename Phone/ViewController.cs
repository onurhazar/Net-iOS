using System;
using System.Diagnostics;
using Foundation;
using UIKit;
using WatchConnectivity;

namespace Net_iOS
{
    public partial class ViewController : UIViewController, IWCSessionDelegate
    {
        int count = 0;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (WCSession.IsSupported)
            {
                var session = WCSession.DefaultSession;
                session.Delegate = this;
                session.ActivateSession();
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        partial void decClicked(NSObject sender)
        {
            if (count > 0)
            {
                --count;
                UpdateTextDisplay();
                SyncCountToWatch();
            }
        }

        partial void incClicked(NSObject sender)
        {
            ++count;
            UpdateTextDisplay();
            SyncCountToWatch();
        }

        void UpdateTextDisplay()
        {
            InvokeOnMainThread(() =>
            {
                displayLabel.Text = count.ToString();
            });
        }

        void SyncCountToWatch()
        {
            if (WCSession.IsSupported && WCSession.DefaultSession.Reachable)
            {
                var data = new NSDictionary<NSString, NSObject>(new NSString("count"), new NSNumber(count));
                WCSession.DefaultSession.SendMessage(data, null, null);
            }
        }

        #region IWCSessionDelegate
        [Export("session:didReceiveMessage:")]
        public void DidReceiveMessage(WCSession session, NSDictionary<NSString, NSObject> message)
        {
            var key = new NSString("count");
            if (message.ContainsKey(key))
            {
                var countNumber = message[key] as NSNumber;
                count = countNumber.Int32Value;
                UpdateTextDisplay();
            }
        }

        public void ActivationDidComplete(WCSession session, WCSessionActivationState activationState, NSError? error)
        {
            Debug.WriteLine("ActivationDidComplete");
            
            Debug.WriteLine("Session Activation State: " + session.ActivationState);
            Debug.WriteLine("Session Paired: " + session.Paired);
            Debug.WriteLine("Session Reachable: " + session.Reachable);
            Debug.WriteLine("Session WatchAppInstalled: " + session.WatchAppInstalled);
            Debug.WriteLine("Session WatchDirectoryUrl: " + session.WatchDirectoryUrl);
        }

        public void DidBecomeInactive(WCSession session)
        {
            Debug.WriteLine("DidBecomeInactive");
        }

        public void DidDeactivate(WCSession session)
        {
            Debug.WriteLine("DidDeactivate");
        }
        #endregion
    }
}
