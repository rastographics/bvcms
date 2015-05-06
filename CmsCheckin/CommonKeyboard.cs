using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmsCheckin
{
	public partial class CommonKeyboard : Form
	{
		private KeyboardInterface listener;

		public CommonKeyboard()
		{
			InitializeComponent();

			b1.Click += onKeyboardClick;
			b2.Click += onKeyboardClick;
			b3.Click += onKeyboardClick;
			b4.Click += onKeyboardClick;
			b5.Click += onKeyboardClick;
			b6.Click += onKeyboardClick;
			b7.Click += onKeyboardClick;
			b8.Click += onKeyboardClick;
			b9.Click += onKeyboardClick;
			b0.Click += onKeyboardClick;
			bdash.Click += onKeyboardClick;
			bequal.Click += onKeyboardClick;

			bq.Click += onKeyboardClick;
			bw.Click += onKeyboardClick;
			be.Click += onKeyboardClick;
			br.Click += onKeyboardClick;
			bt.Click += onKeyboardClick;
			by.Click += onKeyboardClick;
			bu.Click += onKeyboardClick;
			bi.Click += onKeyboardClick;
			bo.Click += onKeyboardClick;
			bp.Click += onKeyboardClick;
			blbrace.Click += onKeyboardClick;
			brbrace.Click += onKeyboardClick;

			ba.Click += onKeyboardClick;
			bs.Click += onKeyboardClick;
			bd.Click += onKeyboardClick;
			bf.Click += onKeyboardClick;
			bg.Click += onKeyboardClick;
			bh.Click += onKeyboardClick;
			bj.Click += onKeyboardClick;
			bk.Click += onKeyboardClick;
			bl.Click += onKeyboardClick;

			bz.Click += onKeyboardClick;
			bx.Click += onKeyboardClick;
			bc.Click += onKeyboardClick;
			bv.Click += onKeyboardClick;
			bb.Click += onKeyboardClick;
			bn.Click += onKeyboardClick;
			bm.Click += onKeyboardClick;

			bcomma.Click += onKeyboardClick;
			bdot.Click += onKeyboardClick;
			bslash.Click += onKeyboardClick;

			bshift.Click += onShiftClick;
			brshift.Click += onShiftClick;

			bbs.Click += onBackspaceClick;

			//TopMost = true;
		}

		public CommonKeyboard(KeyboardInterface listener)
			: this()
		{
			this.listener = listener;
		}

		private void onKeyboardClick(object sender, EventArgs e)
		{
			if (listener != null) {
				Button button = sender as Button;
				String letter = button.Text[0].ToString();

				listener.onKeyboardKeyPress(letter);
			}
		}

		private void onBackspaceClick(object sender, EventArgs e)
		{
			if (listener != null) {
				listener.onBackspaceKeyPress();
			}
		}

		private void onShiftClick(object sender, EventArgs e)
		{
			if (ba.Text == "A") {
				ba.Text = "a";
				bb.Text = "b";
				bc.Text = "c";
				bd.Text = "d";
				be.Text = "e";
				bf.Text = "f";
				bg.Text = "g";
				bh.Text = "h";
				bi.Text = "i";
				bj.Text = "j";
				bk.Text = "k";
				bl.Text = "l";
				bm.Text = "m";
				bn.Text = "n";
				bo.Text = "o";
				bp.Text = "p";
				bq.Text = "q";
				br.Text = "r";
				bs.Text = "s";
				bt.Text = "t";
				bu.Text = "u";
				bv.Text = "v";
				bw.Text = "w";
				bx.Text = "x";
				by.Text = "y";
				bz.Text = "z";

				b1.Text = "1";
				b2.Text = "2";
				b3.Text = "3";
				b4.Text = "4";
				b5.Text = "5";
				b6.Text = "6";
				b7.Text = "7";
				b8.Text = "8";
				b9.Text = "9";
				b0.Text = "0";
				bdash.Text = "-";
				bequal.Text = "=";
				blbrace.Text = "[";
				brbrace.Text = "]";
				bcolon.Text = ":";
				bcomma.Text = ",";
				bdot.Text = ".";
				bslash.Text = "/";
			} else {
				ba.Text = "A";
				bb.Text = "B";
				bc.Text = "C";
				bd.Text = "D";
				be.Text = "E";
				bf.Text = "F";
				bg.Text = "G";
				bh.Text = "H";
				bi.Text = "I";
				bj.Text = "J";
				bk.Text = "K";
				bl.Text = "L";
				bm.Text = "M";
				bn.Text = "N";
				bo.Text = "O";
				bp.Text = "P";
				bq.Text = "Q";
				br.Text = "R";
				bs.Text = "S";
				bt.Text = "T";
				bu.Text = "U";
				bv.Text = "V";
				bw.Text = "W";
				bx.Text = "X";
				by.Text = "Y";
				bz.Text = "Z";

				b1.Text = "!";
				b2.Text = "@";
				b3.Text = "#";
				b4.Text = "$";
				b5.Text = "%";
				b6.Text = "^";
				b7.Text = "&&";
				b8.Text = "*";
				b9.Text = "(";
				b0.Text = ")";
				bdash.Text = "_";
				bequal.Text = "+";
				blbrace.Text = "{";
				brbrace.Text = "}";
				bcolon.Text = ";";
				bcomma.Text = "<";
				bdot.Text = ">";
				bslash.Text = "?";
			}
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;

				p.Style |= 0x40000000; // WS_CHILD
				p.ExStyle |= 0x8000000; // WS_EX_NOACTIVATE - requires Win 2000 or higher

				return p;
			}
		}

		protected override void WndProc(ref Message message)
		{
			const int WM_NCHITTEST = 0x0084;

			if (message.Msg == WM_NCHITTEST)
				return;

			base.WndProc(ref message);
		}
	}
}
