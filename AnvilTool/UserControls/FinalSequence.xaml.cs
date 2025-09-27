using AnvilTool.Constants;
using AnvilTool.Entities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

namespace AnvilTool.UserControls
{
    /// <summary>
    /// Logica di interazione per FinalSequence.xaml
    /// </summary>
    public partial class FinalSequence : UserControl
    {
        List<ActionMove> controls = new List<ActionMove>();
        public FinalSequence()
        {
            InitializeComponent();
            controls.Add(first);
            controls.Add(second);
            controls.Add(third);
        }
        #region FinalSeq
        public ObservableCollection<Move> FinalSeq
        {
            get { return (ObservableCollection<Move>)GetValue(FinalSeqProperty); }
            set { SetValue(FinalSeqProperty, value); }
        }

        public static readonly DependencyProperty FinalSeqProperty =
            DependencyProperty.Register(nameof(FinalSeq)
                , typeof(ObservableCollection<Move>)
                , typeof(FinalSequence)
                , new PropertyMetadata(null, new PropertyChangedCallback(OnFinalSequenceChange))); 

        private static void OnFinalSequenceChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinalSequence c = d as FinalSequence;
            if(e.OldValue is ObservableCollection<Move> oldCollection)
                oldCollection.CollectionChanged -= c.OnFinalSequenceCollectionChanged;

            if (e.NewValue is ObservableCollection<Move> newCollection)
                newCollection.CollectionChanged += c.OnFinalSequenceCollectionChanged;

            c.UpdateSequenceUI();
            
        }
        private void OnFinalSequenceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSequenceUI();
        }
        #endregion
        private static void AddMove(ActionMove ctrl, Move move = null)
        {
            if(move == null)
                move = new Move();

            ctrl.Move = move;
        }
        private void UpdateSequenceUI()
        {
            if (FinalSeq == null)
            {
                AddMove(first);
                AddMove(second);
                AddMove(third);
                return;
            }

            for (int i = 0; i < Consts.MAX_FINAL_SEQ_NUM; i++)
            {
                if (FinalSeq.Count > i + 1)
                    AddMove(controls[i], FinalSeq[i]);
                else
                    AddMove(controls[i]);
            }
        }
    }
}
