using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmsCheckin
{
	public interface KeyboardInterface
	{
		void onKeyboardKeyPress(String key);
		void onBackspaceKeyPress();
	}
}
