using MukaiTablet2.Util;
using MukaiTablet2.View.Common;
using MukaiTablet2.ViewModel;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageIndex : PageBase
    {


        #region ローカル変数
        private VmIndex mVmIndex;
        #endregion

        #region ライフサイクル
        public PageIndex(VmBase vm) : base(vm)
        {
            InitializeComponent();
            mVmIndex = (VmIndex)vm;
            //mVmIndex.NotifyIndexUpdated += onUpdatedIndex;
            //mVmIndex.NotifyProductsUpdated += onUpdatedProducts;
        }

        #region ユーザ操作
        //private void LvBreadcrumb_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    ListView listView = (ListView)sender;
        //    if (listView.SelectedItem == null) return;

        //    mVmIndex.LvBreadcrumb_ItemSelected(sender, e);
        //    listView.SelectedItem = null;
        //}

        #endregion

        #endregion
#if false
        #region イベント処理
        protected override void OnSizeAllocated(double widthArg, double heightArg)
        {
            base.OnSizeAllocated(widthArg, heightArg);

            int width = (int)widthArg;
            int height = (int)heightArg;
            if ((mWitdh != width && 0 < width) || (mHeight != height && 0 < height))
            {
                mWitdh = width;
                mHeight = height;
                if (mIsIndex)
                {
                    if (mIndexUpdated) updateIndexBox();
                }
                else
                {
                    if (mWitdh < mHeight) updateProductsVertical();
                    else updateProductsHorizonal();
                }
            }
        }

        private void onUpdatedIndex(object sender, EventArgs e)
        {
            mIsIndex = true;
            mIndexUpdated = true;
            if (0 < mWitdh) updateIndexBox();  //画面サイズが決まっていたら呼び出し
        }

        private void onUpdatedProducts(object sender, EventArgs e)
        {
            mIsIndex = false;
            if (mWitdh < mHeight) updateProductsVertical();
            else updateProductsHorizonal();
        }
        #endregion




        #region 内部処理
        /// <summary>
        /// インデックスボックスの表示更新
        /// </summary>
        private void updateIndexBox()
        {

            SlBody.Children.Clear();
            SlBody.Orientation = StackOrientation.Vertical;
            ScrollViewItems.Orientation = ScrollOrientation.Vertical;

            int colNum = (int)((mWitdh - 140) / 160);
            if (colNum <= 0)
            {
                Logger.Inst.Assert(false);
                colNum = 1;
            }

            //VMのIndexListをBodyに配置する。
            for (int ii = 0; ii < mVmIndex.IndexList.Count;)
            {

                Grid rowGrid = new Grid();
                for (int col = 0; col < colNum; col++)
                {
                    rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int col = 0; col < colNum; col++)
                {
                    VmIndexItem indexItem = mVmIndex.IndexList[ii];
                    ii++;

                    ViewIndexBox indexBox = new ViewIndexBox();
                    indexBox.MyName = indexItem.IdxName;
                    indexBox.TagColor = indexItem.TagColor;
                    indexBox.Clicked += indexItem.Clicked;
                    Grid.SetColumn(indexBox, col);
                    rowGrid.Children.Add(indexBox);

                    if ((ii < mVmIndex.IndexList.Count) == false) break;
                }
                SlBody.Children.Add(rowGrid);
            }
        }

        /// <summary>
        /// 商品リストの表示更新
        /// </summary>
        private void updateProductsVertical()
        {
            int colNum = (int)((mWitdh - 140) / 200);

            SlBody.Children.Clear();
            SlBody.Orientation = StackOrientation.Vertical;
            ScrollViewItems.Orientation = ScrollOrientation.Vertical;

            if (colNum <= 0)
            {
                Logger.Inst.Assert(false);
                colNum = 1;
            }

            for (int ii = 0; ii < mVmIndex.ProductList.Count;)
            {

                Grid rowGrid = new Grid();
                for (int col = 0; col < colNum; col++)
                {
                    rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int col = 0; col < colNum; col++)
                {
                    VmProduct indexItem = mVmIndex.ProductList[ii];
                    ii++;

                    var viewProduct = new ViewProduct(indexItem);
                    Grid.SetColumn(viewProduct, col);
                    rowGrid.Children.Add(viewProduct);

                    if ((ii < mVmIndex.ProductList.Count) == false) break;
                }
                SlBody.Children.Add(rowGrid);
            }
        }

        private void updateProductsHorizonal()
        {
            int rowNum = (int)((mHeight - 140) / 300);

            SlBody.Children.Clear();
            SlBody.Orientation = StackOrientation.Horizontal;
            ScrollViewItems.Orientation = ScrollOrientation.Horizontal;

            if (rowNum <= 0)
            {
                Logger.Inst.Assert(false);
                ScrollViewItems.Orientation = ScrollOrientation.Both;
                rowNum = 1;
            }

            for (int ii = 0; ii < mVmIndex.ProductList.Count;)
            {

                Grid colGrid = new Grid();
                for (int col = 0; col < rowNum; col++)
                {
                    colGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int row = 0; row < rowNum; row++)
                {
                    VmProduct indexItem = mVmIndex.ProductList[ii];
                    ii++;

                    var viewProduct = new ViewProduct(indexItem);
                    Grid.SetRow(viewProduct, row);
                    colGrid.Children.Add(viewProduct);

                    if ((ii < mVmIndex.ProductList.Count) == false) break;
                }
                SlBody.Children.Add(colGrid);
            }
        }
        #endregion
#endif
    }
}