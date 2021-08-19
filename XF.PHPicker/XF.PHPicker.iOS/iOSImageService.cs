using System;
using System.Linq;
using System.Threading.Tasks;
using PhotosUI;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(XF.PHPicker.iOS.iOSImageService))]
namespace XF.PHPicker.iOS
{
    public class iOSImageService : IImageService
    {
        public IntPtr Handle { get; }

        public async Task PickPhoto()
        {
            await Task.Run(async() =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var configuration = new PHPickerConfiguration();
                    configuration.SelectionLimit = 10; // Set 0 for unlimited selection, default is 1
                    configuration.PreferredAssetRepresentationMode = PHPickerConfigurationAssetRepresentationMode.Compatible; // ensures we don't have to deal with .heic files
                    configuration.Filter = PHPickerFilter.ImagesFilter; // Only show images(including Live Photos)

                    var picker = new PHPickerViewController(configuration: configuration);
                    var pickerDelegate = new PickerDelegate();
                    picker.Delegate = pickerDelegate;
                    picker.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            
                    var viewController = GetHostViewController();
            
                    //_viewController = topViewControllerWithRootViewController(UIApplication.SharedApplication.Delegate.GetWindow().RootViewController);
                    await viewController.PresentViewControllerAsync(picker, true);

                    await Task.FromResult(0);
                });
            });
        }

        public static UIViewController GetHostViewController()
        {
            UIViewController viewController = null;
            var window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
                throw new InvalidOperationException("There's no current active window");

            if (window.WindowLevel == UIWindowLevel.Normal)
                viewController = window.RootViewController;

            if (viewController == null)
            {
                window = UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel).FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
                if (window == null)
                    throw new InvalidOperationException("Could not find current view controller");
                else
                    viewController = window.RootViewController;
            }

            while (viewController.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            return viewController;
        }
    }

    public class PickerDelegate : PHPickerViewControllerDelegate
    {
        public override void DidFinishPicking(PHPickerViewController picker, PHPickerResult[] results)
        {
            // multiple 'DidFinishPicking' calls can be made while processing selections - stop that nonsense here
            if (picker.IsBeingDismissed)
                return;

            //Dismiss Picker
            picker.DismissViewController(true, null);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}