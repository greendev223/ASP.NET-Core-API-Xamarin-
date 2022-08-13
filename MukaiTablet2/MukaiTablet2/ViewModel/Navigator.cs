using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MukaiTablet2.Util;
using MukaiTablet2.View;

namespace MukaiTablet2.ViewModel
{
    public class Navigator
    {
        public static Page CurrentPage
        {
            get {
                Page currentPage = (0 < Application.Current.MainPage.Navigation.ModalStack.Count) ?
                    Application.Current.MainPage.Navigation.ModalStack.LastOrDefault() :
                    Application.Current.MainPage;

                return currentPage;
            }
        }

        //画面遷移
        public static async Task PushModalAsync(Page page, bool isAnimate = true)
        {
            await CurrentPage.Navigation.PushModalAsync(page, isAnimate);
        }

        public static async Task PopModalAsync(bool animated = true)
        {
            if (0 < Application.Current.MainPage.Navigation.ModalStack.Count)
            {
                PageBase currentPage = (PageBase)Application.Current.MainPage.Navigation.ModalStack.LastOrDefault();
                
                await currentPage.mVm.OnTerm();

                await currentPage.Navigation.PopModalAsync(animated);
            }
            else
            {
                Logger.Inst.Assert(false);
            }
        }

        public static async Task PopToStartModalAsync(bool animated = true)
        {
            while (0 < Application.Current.MainPage.Navigation.ModalStack.Count)
            {
                await PopModalAsync(animated);
            }
        }

        public static async Task PopAndPush(Page page,bool animated = true)
        {
            await PopModalAsync(animated);
            await PushModalAsync(page, animated);
        }

    }
}
