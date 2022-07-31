using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Rampastring.XNAUI.Input
{
	// Token: 0x02000010 RID: 16
	public static class KeyboardEventInput
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060000CF RID: 207 RVA: 0x000040D0 File Offset: 0x000022D0
		// (remove) Token: 0x060000D0 RID: 208 RVA: 0x00004104 File Offset: 0x00002304
		public static event KeyboardEventInput.CharEnteredHandler CharEntered;

		// Token: 0x060000D1 RID: 209
		[DllImport("Imm32.dll")]
		private static extern IntPtr ImmGetContext(IntPtr hWnd);

		// Token: 0x060000D2 RID: 210
		[DllImport("Imm32.dll")]
		private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

		// Token: 0x060000D3 RID: 211
		[DllImport("user32.dll")]
		private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x060000D4 RID: 212
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		// Token: 0x060000D5 RID: 213
		[DllImport("imm32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ImmGetCompositionString(IntPtr hIMC, int CompositionStringFlag, IntPtr buffer, int bufferLength);

		// Token: 0x060000D6 RID: 214
		[DllImport("imm32.dll", CharSet = CharSet.Unicode)]
		public static extern uint ImmGetCandidateList(IntPtr hIMC, uint deIndex, IntPtr candidateList, uint dwBufLen);

		// Token: 0x060000D7 RID: 215
		[DllImport("Imm32.dll")]
		private static extern IntPtr ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

		// Token: 0x060000D8 RID: 216
		[DllImport("Imm32.dll")]
		private static extern bool ImmGetCompositionWindow(IntPtr hIMC, ref KeyboardEventInput.COMPOSITIONFORM lpCompForm);

		// Token: 0x060000D9 RID: 217
		[DllImport("Imm32.dll")]
		private static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref KeyboardEventInput.COMPOSITIONFORM lpCompForm);

		// Token: 0x060000DA RID: 218
		[DllImport("Imm32.dll")]
		private static extern bool ImmSetStatusWindowPos(IntPtr hIMC, ref Point lpptPos);

		// Token: 0x060000DB RID: 219
		[DllImport("Imm32.dll")]
		private static extern bool ImmSetCandidateWindow(IntPtr hIMC, ref KeyboardEventInput.CANDIDATEFORM lpCandidate);

		// Token: 0x060000DC RID: 220
		[DllImport("Imm32.dll")]
		private static extern bool ImmGetCandidateWindow(IntPtr hIMC, uint index, ref KeyboardEventInput.CANDIDATEFORM lpCandidate);

		// Token: 0x060000DD RID: 221 RVA: 0x00004138 File Offset: 0x00002338
		public static void Initialize(GameWindow window)
		{
			if (KeyboardEventInput.initialized)
			{
				throw new InvalidOperationException("KeyboardEventInput.Initialize can only be called once!");
			}
			KeyboardEventInput.hookProcDelegate = new KeyboardEventInput.WndProc(KeyboardEventInput.HookProc);
			KeyboardEventInput.prevWndProc = (IntPtr)KeyboardEventInput.SetWindowLong(window.Handle, -4, (int)Marshal.GetFunctionPointerForDelegate(KeyboardEventInput.hookProcDelegate));
			KeyboardEventInput.hIMC = KeyboardEventInput.ImmGetContext(window.Handle);
			KeyboardEventInput.initialized = true;
			KeyboardEventInput.gameWindowHandle = window.Handle;
			KeyboardEventInput.imeActived = false;
			KeyboardEventInput.donotHandle = false;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000041BC File Offset: 0x000023BC
		private static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			IntPtr result = KeyboardEventInput.CallWindowProc(KeyboardEventInput.prevWndProc, hWnd, msg, wParam, lParam);
			IntPtr zero = IntPtr.Zero;
			if (msg == 135U)
			{
				result = (IntPtr)(result.ToInt32() | 4);
			}
			return result;
		}

		// Token: 0x04000055 RID: 85
		private static bool initialized;

		// Token: 0x04000056 RID: 86
		private static IntPtr prevWndProc;

		// Token: 0x04000057 RID: 87
		private static KeyboardEventInput.WndProc hookProcDelegate;

		// Token: 0x04000058 RID: 88
		private static IntPtr hIMC;

		// Token: 0x04000059 RID: 89
		private const int GWL_WNDPROC = -4;

		// Token: 0x0400005A RID: 90
		private const int WM_KEYDOWN = 256;

		// Token: 0x0400005B RID: 91
		private const int WM_KEYUP = 257;

		// Token: 0x0400005C RID: 92
		private const int WM_CHAR = 258;

		// Token: 0x0400005D RID: 93
		private const int WM_IME_SETCONTEXT = 641;

		// Token: 0x0400005E RID: 94
		private const int WM_INPUTLANGCHANGE = 81;

		// Token: 0x0400005F RID: 95
		private const int WM_GETDLGCODE = 135;

		// Token: 0x04000060 RID: 96
		private const int DLGC_WANTALLKEYS = 4;

		// Token: 0x04000061 RID: 97
		private const int WM_IME_NOTIFY = 642;

		// Token: 0x04000062 RID: 98
		private const int IMN_OPENCANDIDATE = 5;

		// Token: 0x04000063 RID: 99
		private const int IMN_CLOSECANDIDATE = 4;

		// Token: 0x04000064 RID: 100
		private const int WM_IME_STARTCOMPOSITION = 269;

		// Token: 0x04000065 RID: 101
		private const int WM_IME_COMPOSITION = 271;

		// Token: 0x04000066 RID: 102
		private const int WM_IME_ENDCOMPOSITION = 270;

		// Token: 0x04000067 RID: 103
		private const int GCS_RESULTSTR = 2048;

		// Token: 0x04000068 RID: 104
		private const int CFS_POINT = 2;

		// Token: 0x04000069 RID: 105
		private const int CFS_CANDIDATEPOS = 64;

		// Token: 0x0400006A RID: 106
		public static IntPtr gameWindowHandle;

		// Token: 0x0400006B RID: 107
		public static IntPtr imeContext;

		// Token: 0x0400006C RID: 108
		public static int immStringLength;

		// Token: 0x0400006D RID: 109
		public static bool imeActived;

		// Token: 0x0400006E RID: 110
		public static bool donotHandle;

		// Token: 0x0400006F RID: 111
		public static Point imeLocation;

		// Token: 0x0200003B RID: 59
		// (Invoke) Token: 0x060003A5 RID: 933
		public delegate void CharEnteredHandler(object sender, KeyboardEventArgs e);

		// Token: 0x0200003C RID: 60
		// (Invoke) Token: 0x060003A9 RID: 937
		private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x0200003D RID: 61
		public struct COMPOSITIONFORM
		{
			// Token: 0x040001AD RID: 429
			public uint dwStyle;

			// Token: 0x040001AE RID: 430
			public Point ptCurrentPos;

			// Token: 0x040001AF RID: 431
			public Rectangle rcArea;
		}

		// Token: 0x0200003E RID: 62
		public struct CANDIDATEFORM
		{
			// Token: 0x040001B0 RID: 432
			public uint dwIndex;

			// Token: 0x040001B1 RID: 433
			public uint dwStyle;

			// Token: 0x040001B2 RID: 434
			public Point ptCurrentPos;

			// Token: 0x040001B3 RID: 435
			public Rectangle rcArea;
		}
	}
}
