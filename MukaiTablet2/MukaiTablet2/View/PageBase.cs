using MukaiTablet2.Util;
using MukaiTablet2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MukaiTablet2.View
{
    public class PageBase : ContentPage
    {
        public VmBase mVm;
        protected double mWidth = 0;
        protected double mHeight = 0;


        public PageBase(VmBase vm)
        {
            mVm = vm;
            mVm.OnInitDone += OnInitDone;
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await mVm.OnAppear();
        }

        protected override void OnDisappearing()
        {
            mVm.OnDisappear();
            base.OnDisappearing();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            bool isChange = false;

            if (mWidth != width)
            {
                mWidth = width;
                isChange = true;
            }
            if (mHeight != height)
            {
                mHeight = height;
                isChange = true;
            }
            if (isChange && 0 < mHeight && 0 < mHeight)
            {
                mVm.OnPageSizeChanged(width, height);
            }
        }

        protected virtual void OnInitDone(object sender, EventArgs e)
        {

        }


    }
}