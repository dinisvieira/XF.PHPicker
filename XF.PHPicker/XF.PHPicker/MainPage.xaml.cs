using System;
using Xamarin.Forms;

namespace XF.PHPicker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await DependencyService.Get<IImageService>().PickPhoto();
        }
    }
}
