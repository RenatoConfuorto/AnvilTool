using LIB.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LIB.UserControls
{
    /// <summary>
    /// Logica di interazione per MoveBar.xaml
    /// </summary>
    public partial class MoveBar : UserControl
    {
        public MoveBar()
        {
            InitializeComponent();
            //PositionSliders();
        }

        #region MinPossible
        public int MinPossible
        {
            get { return (int)GetValue(MinPossibleProperty); }
            set { SetValue(MinPossibleProperty, value); }
        }

        public static readonly DependencyProperty MinPossibleProperty =
            DependencyProperty.Register(nameof(MinPossible)
                , typeof(int)
                , typeof(MoveBar)
                , new PropertyMetadata(Consts.MIN_POS, new PropertyChangedCallback(OnPositionsChanged)));
        #endregion

        #region MaxPossible
        public int MaxPossible
        {
            get { return (int)GetValue(MaxPossibleProperty); }
            set { SetValue(MaxPossibleProperty, value); }
        }

        public static readonly DependencyProperty MaxPossibleProperty =
            DependencyProperty.Register(nameof(MaxPossible)
                , typeof(int)
                , typeof(MoveBar)
                , new PropertyMetadata(Consts.MAX_POS, new PropertyChangedCallback(OnPositionsChanged)));
        #endregion


        #region CurrentPos

        public int CurrentPos
        {
            get { return (int)GetValue(CurrentPosProperty); }
            set { SetValue(CurrentPosProperty, value); }
        }

        public static readonly DependencyProperty CurrentPosProperty =
            DependencyProperty.Register(nameof(CurrentPos)
                , typeof(int)
                , typeof(MoveBar)
                , new PropertyMetadata(Consts.MIN_POS, new PropertyChangedCallback(OnPositionsChanged)));
        #endregion

        private static void OnPositionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MoveBar bar = (MoveBar)d;
            bar.PositionSliders();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            PositionSliders();
        }

        private void PositionSliders()
        {
            PositionSlider(sliderMin, MinPossible, 15, true);
            PositionSlider(sliderMax, MaxPossible, 0, true);
            PositionSlider(sliderCurr, CurrentPos, -10, false);
        }

        private void PositionSlider(Grid sliderContainer, int pos, int offset, bool isTop)
        {
            int MIN_OFFSET = 5;
            double sliderWidth = sliderContainer.Width;
            //if (!isTop)
            //    MIN_OFFSET *= -1;
            //if (sliderContainer == sliderMax)
            //    MIN_OFFSET *= -1;

            var barLeng = baCtrl.ActualWidth;
            var barH = baCtrl.ActualHeight;

            var total = barLeng - (2*MIN_OFFSET);

            var movRatio = total / Consts.MAX_POS;

            double left = 0;
            double top = 0;
            if(pos == Consts.MIN_POS)
                left = MIN_OFFSET;
            else if(pos == Consts.MAX_POS)
                left = barLeng - MIN_OFFSET;
            else
            {
                left = MIN_OFFSET +  pos * movRatio;
            }

            left -= sliderWidth / 2;

            if (isTop)
                top = -barH / 2 + 1;
            else
                top = barH / 2 - 1;

            Canvas.SetLeft(sliderContainer, left);
            Canvas.SetTop(sliderContainer, top);
        }
    }
}
